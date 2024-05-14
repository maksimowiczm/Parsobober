using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query.Queries;

internal static class ParentTransitive
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
                // Parent*(stmt, 1)
                (IStatementDeclaration declaration, IArgument.Line child) =>
                    new GetTransitiveParentByLineNumber(accessor, child.Value).Build(declaration),

                // Parent*(1, stmt)
                (IArgument.Line parent, IStatementDeclaration child) =>
                    new GetTransitiveChildrenByLineNumber(accessor, parent.Value).Build(child),

                // Parent*(stmt, stmt)
                (IStatementDeclaration parent, IStatementDeclaration child) =>
                    BuildParentWithSelect((parentStr, parent), (childStr, child)),

                // Parent*(1, 2) nie wspierane w tej wersji
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
                return new GetTransitiveParentsByChildType(accessor).Create(child.type).Build(parent.type);
            }

            if (child.key == select)
            {
                return new GetTransitiveChildrenByParentType(accessor).Create(parent.type).Build(child.type);
            }

            throw new InvalidOperationException("Invalid query");
        }
    }

    #endregion

    #region Queries

    private class GetTransitiveParentsByChildType(IParentAccessor parentAccessor)
    {
        public ParentQuery Create(IStatementDeclaration childStatementDeclaration) =>
            childStatementDeclaration switch
            {
                IStatementDeclaration.Statement => new GetTransitiveParentsByChildType<Statement>(parentAccessor),
                IStatementDeclaration.Assign => new GetTransitiveParentsByChildType<Assign>(parentAccessor),
                IStatementDeclaration.While => new GetTransitiveParentsByChildType<While>(parentAccessor),
                _ => throw new ArgumentOutOfRangeException(nameof(childStatementDeclaration))
            };
    }

    /// <summary>
    /// Gets transitive parents of given type by child type.
    /// </summary>
    /// <param name="parentAccessor">Parent accessor.</param>
    /// <typeparam name="TChild">Child type.</typeparam>
    private class GetTransitiveParentsByChildType<TChild>(IParentAccessor parentAccessor) : ParentQuery
        where TChild : Statement
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration childStatementDeclaration) =>
            childStatementDeclaration switch
            {
                IStatementDeclaration.Statement => parentAccessor.GetParentsTransitive<TChild>(),
                IStatementDeclaration.Assign => parentAccessor.GetParentsTransitive<TChild>().OfType<Assign>(),
                IStatementDeclaration.While => parentAccessor.GetParentsTransitive<TChild>().OfType<While>(),
                _ => throw new ArgumentOutOfRangeException(nameof(childStatementDeclaration))
            };
    }

    private class GetTransitiveChildrenByParentType(IParentAccessor parentAccessor)
    {
        public ParentQuery Create(IStatementDeclaration parentStatementDeclaration) =>
            parentStatementDeclaration switch
            {
                IStatementDeclaration.Statement => new GetTransitiveChildrenByParentType<Statement>(parentAccessor),
                IStatementDeclaration.Assign => new GetTransitiveChildrenByParentType<Assign>(parentAccessor),
                IStatementDeclaration.While => new GetTransitiveChildrenByParentType<While>(parentAccessor),
                _ => throw new ArgumentOutOfRangeException(nameof(parentStatementDeclaration))
            };
    }

    /// <summary>
    /// Gets transitive children of given type by parent type.
    /// </summary>
    /// <param name="parentAccessor">Parent accessor.</param>
    /// <typeparam name="TParent">Parent type.</typeparam>
    private class GetTransitiveChildrenByParentType<TParent>(IParentAccessor parentAccessor) : ParentQuery
        where TParent : Statement
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration childStatementDeclaration) =>
            childStatementDeclaration switch
            {
                IStatementDeclaration.Statement => parentAccessor.GetChildrenTransitive<TParent>(),
                IStatementDeclaration.Assign => parentAccessor.GetChildrenTransitive<TParent>().OfType<Assign>(),
                IStatementDeclaration.While => parentAccessor.GetChildrenTransitive<TParent>().OfType<While>(),
                _ => throw new ArgumentOutOfRangeException(nameof(childStatementDeclaration))
            };
    }

    /// <summary>
    /// Get transitive parent of given type by child line number.
    /// </summary>
    /// <param name="parentAccessor">Parent accessor.</param>
    /// <param name="line">Line number.</param>
    private class GetTransitiveParentByLineNumber(IParentAccessor parentAccessor, int line) : ParentQuery
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration childStatementDeclaration) =>
            childStatementDeclaration switch
            {
                IStatementDeclaration.Statement => parentAccessor.GetParentsTransitive(line),
                IStatementDeclaration.Assign => parentAccessor.GetParentsTransitive(line).OfType<Assign>(),
                IStatementDeclaration.While => parentAccessor.GetParentsTransitive(line).OfType<While>(),
                _ => throw new ArgumentOutOfRangeException(nameof(childStatementDeclaration))
            };
    }

    /// <summary>
    /// Gets transitive children of given type by parent line number.
    /// </summary>
    /// <param name="parentAccessor">Parent accessor.</param>
    /// <param name="line">Line number.</param>
    private class GetTransitiveChildrenByLineNumber(IParentAccessor parentAccessor, int line) : ParentQuery
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration childStatementDeclaration) =>
            childStatementDeclaration switch
            {
                IStatementDeclaration.Statement => parentAccessor.GetChildrenTransitive(line),
                IStatementDeclaration.Assign => parentAccessor.GetChildrenTransitive(line).OfType<Assign>(),
                IStatementDeclaration.While => parentAccessor.GetChildrenTransitive(line).OfType<While>(),
                _ => throw new ArgumentOutOfRangeException(nameof(childStatementDeclaration))
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
        /// <param name="childStatementDeclaration"> The declaration to build the query for. </param>
        /// <returns> The query. </returns>
        public abstract IEnumerable<Statement> Build(IStatementDeclaration childStatementDeclaration);
    }

    #endregion
}