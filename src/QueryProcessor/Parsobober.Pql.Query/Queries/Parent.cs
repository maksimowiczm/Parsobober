using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Queries;

internal static class Parent
{
    public class QueryDeclaration(IArgument parent, IArgument child, IParentAccessor accessor) : IQueryDeclaration
    {
        public IArgument Left { get; } = parent;
        public IArgument Right { get; } = child;

        public IEnumerable<IComparable> Do(IDeclaration select)
        {
            // pattern matching argumentów
            var query = (Left, Right) switch
            {
                // Parent(stmt, 1)
                (IStatementDeclaration declaration, IArgument.Line child) =>
                    new GetParentByLineNumber(accessor, child.Value).Build(declaration),

                // Parent(1, stmt)
                (IArgument.Line parent, IStatementDeclaration child) =>
                    new GetChildrenByLineNumber(accessor, parent.Value).Build(child),

                // Parent(stmt, stmt)
                (IStatementDeclaration parent, IStatementDeclaration child) => BuildParentWithSelect(parent, child),

                // Parent(1, 2) nie wspierane w tej wersji
                _ => throw new InvalidOperationException("Invalid query")
            };

            return query;

            IEnumerable<Statement> BuildParentWithSelect(IStatementDeclaration parent, IStatementDeclaration child)
            {
                if (parent== select)
                {
                    return new GetParentsByChildType(accessor).Create(child).Build(parent);
                }

                if (child== select)
                {
                    return new GetChildrenByParentType(accessor).Create(parent).Build(child);
                }

                throw new Exception("No chyba coś ci się pomyliło kolego, taka sytuacja nigdy nie mogla zajść");
            }
        }
    }

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