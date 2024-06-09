using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Queries.Core;
using Parsobober.Pql.Query.Queries.Exceptions;

namespace Parsobober.Pql.Query.Queries;

internal static class FollowsTransitive
{
    public class QueryDeclaration(IArgument followed, IArgument follows, IFollowsAccessor accessor)
        : ReplaceableArgumentQueryDeclaration<QueryDeclaration>, IQueryDeclaration
    {
        public override IArgument Left { get; } = followed;
        public override IArgument Right { get; } = follows;

        public override IEnumerable<IPkbDto> Do()
        {
            var query = (Left, Right) switch
            {
                (Line left, Line right) => new BooleanFollowsQuery(accessor, left.Value, right.Value).Build(),
                (Any, _) or (_, Any) => HandleAny(),
                _ => DoDeclaration()
            };

            return query;
        }

        private IEnumerable<Statement> HandleAny() => (Left, Right) switch
        {
            (Line predecessor, Any) => accessor.GetFollowersTransitive(predecessor.Value),
            (Any, Line follower) => accessor.GetPrecedingTransitive(follower.Value),
            (Any, Any) => accessor.GetFollowersTransitive<Statement>(),
            _ => Enumerable.Empty<Statement>()
        };

        public override IEnumerable<IPkbDto> Do(IDeclaration select)
        {
            // pattern matching argumentów
            var query = (Left, Right) switch
            {
                // followed*(stmt, 1)
                (IStatementDeclaration preceding, Line follower) =>
                    new GetTransitivePrecedingByLineNumber(accessor, follower.Value).Build(preceding),

                // followed*(1, stmt)
                (Line preceding, IStatementDeclaration follower) =>
                    new GetTransitiveFollowerByLineNumber(accessor, preceding.Value).Build(follower),

                // followed*(stmt, stmt)
                (IStatementDeclaration preceding, IStatementDeclaration follower) =>
                    BuildFollowedWithSelect(preceding, follower),

                _ => throw new QueryNotSupported(this, $"Followed*({Left}, {Right}) is not supported.")
            };

            return query;

            IEnumerable<Statement> BuildFollowedWithSelect(
                IStatementDeclaration preceding,
                IStatementDeclaration follows
            )
            {
                if (preceding == select)
                {
                    return new GetTransitivePrecedingByFollowsType(accessor).Create(follows).Build(preceding);
                }

                if (follows == select)
                {
                    return new GetTransitiveFollowerByFollowedType(accessor).Create(preceding).Build(follows);
                }

                throw new DeclarationNotFoundException(select, this);
            }
        }

        protected override QueryDeclaration CloneSelf(IArgument left, IArgument right) => new(left, right, accessor);
    }

    #region Queries

    private class GetTransitivePrecedingByFollowsType(IFollowsAccessor followedAccessor)
    {
        public FollowedQuery Create(IStatementDeclaration followsStatementDeclaration) =>
            followsStatementDeclaration switch
            {
                IStatementDeclaration.Statement => new GetTransitivePrecedingByFollowsType<Statement>(followedAccessor),
                IStatementDeclaration.Assign => new GetTransitivePrecedingByFollowsType<Assign>(followedAccessor),
                IStatementDeclaration.While => new GetTransitivePrecedingByFollowsType<While>(followedAccessor),
                IStatementDeclaration.If => new GetTransitivePrecedingByFollowsType<If>(followedAccessor),
                IStatementDeclaration.Call => new GetTransitivePrecedingByFollowsType<Call>(followedAccessor),
                _ => throw new ArgumentOutOfRangeException(nameof(followsStatementDeclaration))
            };
    }

    private class GetTransitivePrecedingByFollowsType<TFollows>(IFollowsAccessor followedAccessor) : FollowedQuery
        where TFollows : Statement
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration precedingStatementDeclaration) =>
            precedingStatementDeclaration switch
            {
                IStatementDeclaration.Statement => followedAccessor.GetPrecedingTransitive<TFollows>(),
                IStatementDeclaration.Assign => followedAccessor.GetPrecedingTransitive<TFollows>().OfType<Assign>(),
                IStatementDeclaration.While => followedAccessor.GetPrecedingTransitive<TFollows>().OfType<While>(),
                IStatementDeclaration.If => followedAccessor.GetPrecedingTransitive<TFollows>().OfType<If>(),
                IStatementDeclaration.Call => followedAccessor.GetPrecedingTransitive<TFollows>().OfType<Call>(),
                _ => throw new ArgumentOutOfRangeException(nameof(precedingStatementDeclaration))
            };
    }

    private class GetTransitiveFollowerByFollowedType(IFollowsAccessor followedAccessor)
    {
        public FollowedQuery Create(IStatementDeclaration precedingStatementDeclaration) =>
            precedingStatementDeclaration switch
            {
                IStatementDeclaration.Statement => new GetTransitiveFollowerByFollowedType<Statement>(followedAccessor),
                IStatementDeclaration.Assign => new GetTransitiveFollowerByFollowedType<Assign>(followedAccessor),
                IStatementDeclaration.While => new GetTransitiveFollowerByFollowedType<While>(followedAccessor),
                IStatementDeclaration.If => new GetTransitiveFollowerByFollowedType<If>(followedAccessor),
                IStatementDeclaration.Call => new GetTransitiveFollowerByFollowedType<Call>(followedAccessor),
                _ => throw new ArgumentOutOfRangeException(nameof(precedingStatementDeclaration))
            };
    }

    private class GetTransitiveFollowerByFollowedType<TFollowed>(IFollowsAccessor followedAccessor) : FollowedQuery
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

    private class GetTransitivePrecedingByLineNumber(IFollowsAccessor followedAccessor, int line) : FollowedQuery
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration followsStatementDeclaration) =>
            followsStatementDeclaration switch
            {
                IStatementDeclaration.Statement => followedAccessor.GetPrecedingTransitive(line),
                IStatementDeclaration.Assign => followedAccessor.GetPrecedingTransitive(line).OfType<Assign>(),
                IStatementDeclaration.While => followedAccessor.GetPrecedingTransitive(line).OfType<While>(),
                IStatementDeclaration.If => followedAccessor.GetPrecedingTransitive(line).OfType<If>(),
                IStatementDeclaration.Call => followedAccessor.GetPrecedingTransitive(line).OfType<Call>(),
                _ => throw new ArgumentOutOfRangeException(nameof(followsStatementDeclaration))
            };
    }

    private class GetTransitiveFollowerByLineNumber(IFollowsAccessor followedAccessor, int line) : FollowedQuery
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

    private abstract class FollowedQuery
    {
        public abstract IEnumerable<Statement> Build(IStatementDeclaration followsStatementDeclaration);
    }

    private class BooleanFollowsQuery(IFollowsAccessor accessor, int left, int right)
    {
        public IEnumerable<IPkbDto> Build() => IPkbDto.Boolean(accessor.IsFollowsTransitive(left, right));
    }

    #endregion
}