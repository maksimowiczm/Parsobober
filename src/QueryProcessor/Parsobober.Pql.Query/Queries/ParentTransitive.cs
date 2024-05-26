using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Queries.Core;

namespace Parsobober.Pql.Query.Queries;

internal static class ParentTransitive
{
    public class QueryDeclaration(IArgument parent, IArgument child, IParentAccessor accessor) :
        ReplaceableArgumentQueryDeclaration<QueryDeclaration>, IQueryDeclaration
    {
        public override IArgument Left { get; } = parent;
        public override IArgument Right { get; } = child;

        public override IEnumerable<IComparable> Do(IDeclaration select)
        {
            // pattern matching argumentów
            var query = (Left, Right) switch
            {
                // Parent*(stmt, 1)
                (IStatementDeclaration declaration, IArgument.Line child) =>
                    new GetTransitiveParentByLineNumber(accessor, child.Value).Build(declaration),

                // Parent*(1, stmt)
                (IArgument.Line parent, IStatementDeclaration child) =>
                    new GetTransitiveChildrenByLineNumber(accessor, parent.Value).Build(child),

                // Parent*(stmt, stmt)
                (IStatementDeclaration parent, IStatementDeclaration child) => BuildParentWithSelect(parent, child),

                // Parent*(1, 2) nie wspierane w tej wersji
                _ => throw new InvalidOperationException("Invalid query")
            };

            return query;

            IEnumerable<Statement> BuildParentWithSelect(IStatementDeclaration parent, IStatementDeclaration child)
            {
                // tu nastąpi samowywrotka przy zapytaniach, w których nie ma wartości z selecta
                // przykład: Select x such that Parent(a, b)

                if (parent == select)
                {
                    return new GetTransitiveParentsByChildType(accessor).Create(child).Build(parent);
                }

                if (child == select)
                {
                    return new GetTransitiveChildrenByParentType(accessor).Create(parent).Build(child);
                }

                throw new Exception("No chyba coś ci się pomyliło kolego, taka sytuacja nigdy nie mogla zajść");
            }
        }

        protected override QueryDeclaration CloneSelf(IArgument left, IArgument right) => new(left, right, accessor);
    }

    #region Queries

    private class GetTransitiveParentsByChildType(IParentAccessor parentAccessor)
    {
        public ParentQuery Create(IStatementDeclaration childStatementDeclaration) =>
            childStatementDeclaration switch
            {
                IStatementDeclaration.Statement => new GetTransitiveParentsByChildType<Statement>(parentAccessor),
                IStatementDeclaration.Assign => new GetTransitiveParentsByChildType<Assign>(parentAccessor),
                IStatementDeclaration.While => new GetTransitiveParentsByChildType<While>(parentAccessor),
                IStatementDeclaration.If => new GetTransitiveParentsByChildType<If>(parentAccessor),
                IStatementDeclaration.Call => new GetTransitiveParentsByChildType<Call>(parentAccessor),
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
                IStatementDeclaration.If => parentAccessor.GetParentsTransitive<TChild>().OfType<If>(),
                IStatementDeclaration.Call => parentAccessor.GetParentsTransitive<TChild>().OfType<Call>(),
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
                IStatementDeclaration.If => new GetTransitiveChildrenByParentType<If>(parentAccessor),
                IStatementDeclaration.Call => new GetTransitiveChildrenByParentType<Call>(parentAccessor),
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
                IStatementDeclaration.If => parentAccessor.GetChildrenTransitive<TParent>().OfType<If>(),
                IStatementDeclaration.Call => parentAccessor.GetChildrenTransitive<TParent>().OfType<Call>(),
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
                IStatementDeclaration.If => parentAccessor.GetParentsTransitive(line).OfType<If>(),
                IStatementDeclaration.Call => parentAccessor.GetParentsTransitive(line).OfType<Call>(),
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
                IStatementDeclaration.If => parentAccessor.GetChildrenTransitive(line).OfType<If>(),
                IStatementDeclaration.Call => parentAccessor.GetChildrenTransitive(line).OfType<Call>(),
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