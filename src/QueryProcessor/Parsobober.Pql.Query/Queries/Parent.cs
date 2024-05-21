using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Abstractions;
using Parsobober.Pql.Query.Arguments;

namespace Parsobober.Pql.Query.Queries;

internal static class Parent
{
    #region Builder

    public class Builder(IParentAccessor accessor)
    {
        private readonly List<(string parent, string child)> _parentRelations = [];

        public void Add(string parent, string child)
        {
            _parentRelations.Add((parent, child));
        }

        public IEnumerable<Statement>? Build(string select, IReadOnlyDictionary<string, IDeclaration> declarations)
        {
            if (_parentRelations.Count == 0)
            {
                return null;
            }

            // todo aktualnie działa tylko dla jednego parenta
            // na pierwszą iterację wystarczy

            if (_parentRelations.Count > 1)
            {
                throw new InvalidOperationException("Invalid query");
            }

            var parent = _parentRelations.First().parent;
            var child = _parentRelations.First().child;

            var query = new InnerBuilder(accessor, select, declarations).Build(parent, child);

            return query;
        }
    }

    private class InnerBuilder(
        IParentAccessor accessor,
        string select,
        IReadOnlyDictionary<string, IDeclaration> declarations
    )
    {
        public IEnumerable<Statement> Build(string parentStr, string childStr)
        {
            // parsowanie argumentów
            var parentArgument = IArgument.Parse(declarations, parentStr);
            var childArgument = IArgument.Parse(declarations, childStr);

            // pattern matching argumentów
            var query = (parentArgument, childArgument) switch
            {
                // Parent(stmt, 1)
                (IStatementDeclaration declaration, IArgument.Line child) =>
                    new GetParentByLineNumber(accessor, child.Value).Build(declaration),

                // Parent(1, stmt)
                (IArgument.Line parent, IStatementDeclaration child) =>
                    new GetChildrenByLineNumber(accessor, parent.Value).Build(child),

                // Parent(stmt, stmt)
                (IStatementDeclaration parent, IStatementDeclaration child) =>
                    BuildParentWithSelect((parentStr, parent), (childStr, child)),

                // Parent(1, 2) nie wspierane w tej wersji
                _ => throw new InvalidOperationException("Invalid query")
            };

            return query;
        }

        private IEnumerable<Statement> BuildParentWithSelect(
            (string key, IStatementDeclaration type) parent,
            (string key, IStatementDeclaration type) child
        )
        {
            // tu nastąpi samowywrotka przy zapytaniach, w których nie ma wartości z selecta
            // przykład: Select x such that Parent(a, b)

            if (parent.key == select)
            {
                return new GetParentsByChildType(accessor).Create(child.type).Build(parent.type);
            }

            if (child.key == select)
            {
                return new GetChildrenByParentType(accessor).Create(parent.type).Build(child.type);
            }

            throw new InvalidOperationException("Invalid query");
        }
    }

    #endregion

    #region Queries

    private class GetParentsByChildType(IParentAccessor parentAccessor)
    {
        public ParentQuery Create(IStatementDeclaration childStatementDeclaration) =>
            childStatementDeclaration switch
            {
                IStatementDeclaration.Statement => new GetParentsByChildType<Statement>(parentAccessor),
                IStatementDeclaration.Assign => new GetParentsByChildType<Assign>(parentAccessor),
                IStatementDeclaration.While => new GetParentsByChildType<While>(parentAccessor),
                _ => throw new ArgumentOutOfRangeException(nameof(childStatementDeclaration))
            };
    }

    /// <summary>
    /// Gets parents of given type by child type.
    /// </summary>
    /// <param name="parentAccessor">Parent accessor.</param>
    /// <typeparam name="TChild">Child type.</typeparam>
    private class GetParentsByChildType<TChild>(IParentAccessor parentAccessor) : ParentQuery
        where TChild : Statement
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration parentStatementDeclaration) =>
            parentStatementDeclaration switch
            {
                IStatementDeclaration.Statement => parentAccessor.GetParents<TChild>(),
                IStatementDeclaration.Assign => parentAccessor.GetParents<TChild>().OfType<Assign>(),
                IStatementDeclaration.While => parentAccessor.GetParents<TChild>().OfType<While>(),
                _ => throw new ArgumentOutOfRangeException(nameof(parentStatementDeclaration))
            };
    }

    private class GetChildrenByParentType(IParentAccessor parentAccessor)
    {
        public ParentQuery Create(IStatementDeclaration parentStatementDeclaration) =>
            parentStatementDeclaration switch
            {
                IStatementDeclaration.Statement => new GetChildrenByParentType<Statement>(parentAccessor),
                IStatementDeclaration.Assign => new GetChildrenByParentType<Assign>(parentAccessor),
                IStatementDeclaration.While => new GetChildrenByParentType<While>(parentAccessor),
                _ => throw new ArgumentOutOfRangeException(nameof(parentStatementDeclaration))
            };
    }

    /// <summary>
    /// Gets children of given type by parent type.
    /// </summary>
    /// <param name="parentAccessor">Parent accessor.</param>
    /// <typeparam name="TParent">Parent type.</typeparam>
    private class GetChildrenByParentType<TParent>(IParentAccessor parentAccessor) : ParentQuery
        where TParent : Statement
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration childStatementDeclaration) =>
            childStatementDeclaration switch
            {
                IStatementDeclaration.Statement => parentAccessor.GetChildren<TParent>(),
                IStatementDeclaration.Assign => parentAccessor.GetChildren<TParent>().OfType<Assign>(),
                IStatementDeclaration.While => parentAccessor.GetChildren<TParent>().OfType<While>(),
                _ => throw new ArgumentOutOfRangeException(nameof(childStatementDeclaration))
            };
    }

    /// <summary>
    /// Get parent of given type by child line number.
    /// </summary>
    /// <param name="parentAccessor">Parent accessor.</param>
    /// <param name="line">Line number.</param>
    private class GetParentByLineNumber(IParentAccessor parentAccessor, int line) : ParentQuery
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration parent)
        {
            var parentStatement = parentAccessor.GetParent(line);

            var result = parent switch
            {
                IStatementDeclaration.Statement => parentStatement,
                IStatementDeclaration.Assign => parentStatement as Assign,
                IStatementDeclaration.While => parentStatement as While,
                _ => throw new ArgumentOutOfRangeException(nameof(parent))
            };

            if (result is null)
            {
                return Enumerable.Empty<Statement>();
            }

            return Enumerable.Repeat(result, 1);
        }
    }

    /// <summary>
    /// Gets children of given type by parent line number.
    /// </summary>
    /// <param name="parentAccessor">Parent accessor.</param>
    /// <param name="line">Line number.</param>
    private class GetChildrenByLineNumber(IParentAccessor parentAccessor, int line) : ParentQuery
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration child) =>
            child switch
            {
                IStatementDeclaration.Statement => parentAccessor.GetChildren(line),
                IStatementDeclaration.Assign => parentAccessor.GetChildren(line).OfType<Assign>(),
                IStatementDeclaration.While => parentAccessor.GetChildren(line).OfType<While>(),
                _ => throw new ArgumentOutOfRangeException(nameof(child))
            };
    }

    /// <summary>
    /// Represents a parent query.
    /// </summary>
    private abstract class ParentQuery
    {
        /// <summary>
        /// Builds a query.
        /// </summary>
        /// <param name="declaration"> The declaration to build the query for. </param>
        /// <returns> The query. </returns>
        public abstract IEnumerable<Statement> Build(IStatementDeclaration declaration);
    }

    #endregion
}