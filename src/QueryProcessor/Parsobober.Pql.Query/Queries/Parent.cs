using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Queries.Core;
using Parsobober.Pql.Query.Queries.Exceptions;

namespace Parsobober.Pql.Query.Queries;

internal static class Parent
{
    public class QueryDeclaration(IArgument parent, IArgument child, IParentAccessor accessor)
        : ReplaceableArgumentQueryDeclaration<QueryDeclaration>, IQueryDeclaration
    {
        public override IArgument Left { get; } = parent;
        public override IArgument Right { get; } = child;

        public override IEnumerable<IPkbDto> Do()
        {
            var query = (Left, Right) switch
            {
                (Line parent, Line child) => new BooleanParentQuery(accessor, parent.Value, child.Value).Build(),
                (Any, _) or (_, Any) => HandleAny(),
                _ => DoDeclaration()
            };

            return query;
        }

        private IEnumerable<IPkbDto> HandleAny() => (Left, Right) switch
        {
            (Line parent, Any) => accessor.GetChildren(parent.Value),
            (Any, Line child) when accessor.GetParent(child.Value) is not null =>
                Enumerable.Repeat(accessor.GetParent(child.Value)!, 1),
            (Any, Any) => accessor.GetParents<Statement>(),
            (IStatementDeclaration parent, Any) => new GetParentsByChildType(accessor).Create().Build(parent),
            (Any, IStatementDeclaration child) => new GetChildrenByParentType(accessor).Create().Build(child),
            _ => Enumerable.Empty<Statement>()
        };

        public override IEnumerable<IPkbDto> Do(IDeclaration select)
        {
            // pattern matching argumentów
            var query = (Left, Right) switch
            {
                // Parent(stmt, 1)
                (IStatementDeclaration declaration, Line child) =>
                    new GetParentByLineNumber(accessor, child.Value).Build(declaration),

                // Parent(1, stmt)
                (Line parent, IStatementDeclaration child) =>
                    new GetChildrenByLineNumber(accessor, parent.Value).Build(child),

                // Parent(stmt, stmt)
                (IStatementDeclaration parent, IStatementDeclaration child) => BuildParentWithSelect(parent, child),

                _ => throw new QueryNotSupported(this, $"Parent({Left}, {Right}) is not supported.")
            };

            return query;

            IEnumerable<IPkbDto> BuildParentWithSelect(IStatementDeclaration parent, IStatementDeclaration child)
            {
                if (parent == select)
                {
                    return new GetParentsByChildType(accessor).Create(child).Build(parent);
                }

                if (child == select)
                {
                    return new GetChildrenByParentType(accessor).Create(parent).Build(child);
                }

                throw new DeclarationNotFoundException(select, this);
            }
        }

        protected override QueryDeclaration CloneSelf(IArgument left, IArgument right) => new(left, right, accessor);
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
                IStatementDeclaration.If => new GetParentsByChildType<If>(parentAccessor),
                IStatementDeclaration.Call => new GetParentsByChildType<Call>(parentAccessor),
                _ => throw new ArgumentOutOfRangeException(nameof(childStatementDeclaration))
            };

        public ParentQuery Create() => new GetParentsByChildType<Statement>(parentAccessor);
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
                IStatementDeclaration.If => parentAccessor.GetParents<TChild>().OfType<If>(),
                IStatementDeclaration.Call => parentAccessor.GetParents<TChild>().OfType<Call>(),
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
                IStatementDeclaration.If => new GetChildrenByParentType<If>(parentAccessor),
                IStatementDeclaration.Call => new GetChildrenByParentType<Call>(parentAccessor),
                _ => throw new ArgumentOutOfRangeException(nameof(parentStatementDeclaration))
            };

        public ParentQuery Create() => new GetChildrenByParentType<Statement>(parentAccessor);
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
                IStatementDeclaration.If => parentAccessor.GetChildren<TParent>().OfType<If>(),
                IStatementDeclaration.Call => parentAccessor.GetChildren<TParent>().OfType<Call>(),
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
                IStatementDeclaration.If => parentStatement as If,
                IStatementDeclaration.Call => parentStatement as Call,
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
        public override IEnumerable<IPkbDto> Build(IStatementDeclaration child) =>
            child switch
            {
                IStatementDeclaration.Statement => parentAccessor.GetChildren(line),
                IStatementDeclaration.Assign => parentAccessor.GetChildren(line).OfType<Assign>(),
                IStatementDeclaration.While => parentAccessor.GetChildren(line).OfType<While>(),
                IStatementDeclaration.If => parentAccessor.GetChildren(line).OfType<If>(),
                IStatementDeclaration.Call => parentAccessor.GetChildren(line).OfType<Call>(),
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
        public abstract IEnumerable<IPkbDto> Build(IStatementDeclaration declaration);
    }

    private class BooleanParentQuery(IParentAccessor accessor, int parent, int child)
    {
        public IEnumerable<IPkbDto> Build() => IPkbDto.Boolean(accessor.IsParent(parent, child));
    }

    #endregion
}