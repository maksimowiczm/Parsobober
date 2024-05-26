using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Queries.Core;

namespace Parsobober.Pql.Query.Queries;

internal static class FollowsTransitive
{
    public class QueryDeclaration(IArgument followed, IArgument follows, IFollowsAccessor accessor)
        : ReplaceableArgumentQueryDeclaration<QueryDeclaration>, IQueryDeclaration
    {
        public override IArgument Left { get; } = followed;
        public override IArgument Right { get; } = follows;

        public override IEnumerable<IComparable> Do(IDeclaration select)
        {
            // pattern matching argumentów
            var query = (Left, Right) switch
            {
                // followed*(stmt, 1)
                (IStatementDeclaration declaration, IArgument.Line follows) =>
                    new GetTransitiveFollowedByLineNumber(accessor, follows.Value).Build(declaration),

                // followed*(1, stmt)
                (IArgument.Line followed, IStatementDeclaration follows) =>
                    new GetTransitiveFollowsByLineNumber(accessor, followed.Value).Build(follows),

                // followed*(stmt, stmt)
                (IStatementDeclaration followed, IStatementDeclaration follows) =>
                    BuildFollowedWithSelect(followed, follows),

                // followed*(1, 2) niewspierane w tej wersji
                _ => throw new InvalidOperationException("Invalid query")
            };

            return query;

            IEnumerable<Statement> BuildFollowedWithSelect(
                IStatementDeclaration followed,
                IStatementDeclaration follows
            )
            {
                // tu nastąpi samowywrotka przy zapytaniach, w których nie ma wartości z selecta
                // przykład: Select x such that followed(a, b)

                if (followed == select)
                {
                    return new GetTransitiveFollowedByFollowsType(accessor).Create(follows).Build(followed);
                }

                if (follows == select)
                {
                    return new GetTransitiveFollowsByFollowedType(accessor).Create(followed).Build(follows);
                }

                throw new InvalidOperationException("Invalid query");
            }
        }

        protected override QueryDeclaration CloneSelf(IArgument left, IArgument right) => new(left, right, accessor);
    }

    #region Queries

    private class GetTransitiveFollowedByFollowsType(IFollowsAccessor followedAccessor)
    {
        public FollowedQuery Create(IStatementDeclaration followsStatementDeclaration) =>
            followsStatementDeclaration switch
            {
                IStatementDeclaration.Statement => new GetTransitiveFollowedByFollowsType<Statement>(followedAccessor),
                IStatementDeclaration.Assign => new GetTransitiveFollowedByFollowsType<Assign>(followedAccessor),
                IStatementDeclaration.While => new GetTransitiveFollowedByFollowsType<While>(followedAccessor),
                IStatementDeclaration.If => new GetTransitiveFollowedByFollowsType<If>(followedAccessor),
                IStatementDeclaration.Call => new GetTransitiveFollowedByFollowsType<Call>(followedAccessor),
                _ => throw new ArgumentOutOfRangeException(nameof(followsStatementDeclaration))
            };
    }

    /// <summary>
    /// Gets transitive followeds of given type by follows type.
    /// </summary>
    /// <param name="followedAccessor">followed accessor.</param>
    /// <typeparam name="TFollows">follows type.</typeparam>
    private class GetTransitiveFollowedByFollowsType<TFollows>(IFollowsAccessor followedAccessor) : FollowedQuery
        where TFollows : Statement
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration followsStatementDeclaration) =>
            followsStatementDeclaration switch
            {
                IStatementDeclaration.Statement => followedAccessor.GetFollowedTransitive<TFollows>(),
                IStatementDeclaration.Assign => followedAccessor.GetFollowedTransitive<TFollows>().OfType<Assign>(),
                IStatementDeclaration.While => followedAccessor.GetFollowedTransitive<TFollows>().OfType<While>(),
                IStatementDeclaration.If => followedAccessor.GetFollowedTransitive<TFollows>().OfType<If>(),
                IStatementDeclaration.Call => followedAccessor.GetFollowedTransitive<TFollows>().OfType<Call>(),
                _ => throw new ArgumentOutOfRangeException(nameof(followsStatementDeclaration))
            };
    }

    private class GetTransitiveFollowsByFollowedType(IFollowsAccessor followedAccessor)
    {
        public FollowedQuery Create(IStatementDeclaration followedStatementDeclaration) =>
            followedStatementDeclaration switch
            {
                IStatementDeclaration.Statement => new GetTransitiveFollowsByFollowedType<Statement>(followedAccessor),
                IStatementDeclaration.Assign => new GetTransitiveFollowsByFollowedType<Assign>(followedAccessor),
                IStatementDeclaration.While => new GetTransitiveFollowsByFollowedType<While>(followedAccessor),
                IStatementDeclaration.If => new GetTransitiveFollowsByFollowedType<If>(followedAccessor),
                IStatementDeclaration.Call => new GetTransitiveFollowsByFollowedType<Call>(followedAccessor),
                _ => throw new ArgumentOutOfRangeException(nameof(followedStatementDeclaration))
            };
    }

    /// <summary>
    /// Gets transitive followsren of given type by followed type.
    /// </summary>
    /// <param name="followedAccessor">followed accessor.</param>
    /// <typeparam name="TFollowed">followed type.</typeparam>
    private class GetTransitiveFollowsByFollowedType<TFollowed>(IFollowsAccessor followedAccessor) : FollowedQuery
        where TFollowed : Statement
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration followsStatementDeclaration) =>
            followsStatementDeclaration switch
            {
                IStatementDeclaration.Statement => followedAccessor.GetFollowersTransitive<TFollowed>(),
                IStatementDeclaration.Assign => followedAccessor.GetFollowersTransitive<TFollowed>().OfType<Assign>(),
                IStatementDeclaration.While => followedAccessor.GetFollowersTransitive<TFollowed>().OfType<While>(),
                IStatementDeclaration.If => followedAccessor.GetFollowersTransitive<TFollowed>().OfType<If>(),
                IStatementDeclaration.Call => followedAccessor.GetFollowersTransitive<TFollowed>().OfType<Call>(),
                _ => throw new ArgumentOutOfRangeException(nameof(followsStatementDeclaration))
            };
    }

    /// <summary>
    /// Get transitive followed of given type by follows line number.
    /// </summary>
    /// <param name="followedAccessor">followed accessor.</param>
    /// <param name="line">Line number.</param>
    private class GetTransitiveFollowedByLineNumber(IFollowsAccessor followedAccessor, int line) : FollowedQuery
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration followsStatementDeclaration) =>
            followsStatementDeclaration switch
            {
                IStatementDeclaration.Statement => followedAccessor.GetFollowedTransitive(line),
                IStatementDeclaration.Assign => followedAccessor.GetFollowedTransitive(line).OfType<Assign>(),
                IStatementDeclaration.While => followedAccessor.GetFollowedTransitive(line).OfType<While>(),
                IStatementDeclaration.If => followedAccessor.GetFollowedTransitive(line).OfType<If>(),
                IStatementDeclaration.Call => followedAccessor.GetFollowedTransitive(line).OfType<Call>(),
                _ => throw new ArgumentOutOfRangeException(nameof(followsStatementDeclaration))
            };
    }

    /// <summary>
    /// Gets transitive followsren of given type by followed line number.
    /// </summary>
    /// <param name="followedAccessor">followed accessor.</param>
    /// <param name="line">Line number.</param>
    private class GetTransitiveFollowsByLineNumber(IFollowsAccessor followedAccessor, int line) : FollowedQuery
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration followsStatementDeclaration) =>
            followsStatementDeclaration switch
            {
                IStatementDeclaration.Statement => followedAccessor.GetFollowersTransitive(line),
                IStatementDeclaration.Assign => followedAccessor.GetFollowersTransitive(line).OfType<Assign>(),
                IStatementDeclaration.While => followedAccessor.GetFollowersTransitive(line).OfType<While>(),
                IStatementDeclaration.If => followedAccessor.GetFollowersTransitive(line).OfType<If>(),
                IStatementDeclaration.Call => followedAccessor.GetFollowersTransitive(line).OfType<Call>(),
                _ => throw new ArgumentOutOfRangeException(nameof(followsStatementDeclaration))
            };
    }

    /// <summary>
    /// Represents a followed query.
    /// </summary>
    private abstract class FollowedQuery
    {
        /// <summary>
        /// Builds a query.
        /// </summary>
        /// <param name="followsStatementDeclaration"> The declaration to build the query for. </param>
        /// <returns> The query. </returns>
        public abstract IEnumerable<Statement> Build(IStatementDeclaration followsStatementDeclaration);
    }

    #endregion
}