using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Queries.Core;
using Parsobober.Pql.Query.Queries.Exceptions;

namespace Parsobober.Pql.Query.Queries;

internal static class Follows
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
            (Line predecessor, Any) when accessor.GetFollower(predecessor.Value) is not null =>
                Enumerable.Repeat(accessor.GetFollower(predecessor.Value)!, 1),
            (Any, Line follower) when accessor.GetPreceding(follower.Value) is not null =>
                Enumerable.Repeat(accessor.GetPreceding(follower.Value)!, 1),
            (Any, Any) => accessor.GetFollowersTransitive<Statement>(),
            _ => Enumerable.Empty<Statement>()
        };

        public override IEnumerable<IPkbDto> Do(IDeclaration select)
        {
            // pattern matching argumentów
            var query = (Left, Right) switch
            {
                // Follows(stmt, 1)
                (IStatementDeclaration preceding, Line follows) =>
                    new GetPrecedingByLineNumber(accessor, follows.Value).Build(preceding),

                //Follows(1, stmt) 
                (Line preceding, IStatementDeclaration follows) =>
                    new GetFollowerByLineNumber(accessor, preceding.Value).Build(follows),

                // Follows(stmt, stmt)
                (IStatementDeclaration preceding, IStatementDeclaration follows) =>
                    BuildFollowsWithSelect(preceding, follows),

                _ => throw new QueryNotSupported(this, $"Follows({Left}, {Right}) is not supported.")
            };

            return query;

            IEnumerable<IPkbDto> BuildFollowsWithSelect(
                IStatementDeclaration preceding,
                IStatementDeclaration follows
            )
            {
                if (preceding == select)
                {
                    return new GetPrecedingByFollowsType(accessor).Create(follows).Build(preceding);
                }

                if (follows == select)
                {
                    var xd = new GetFollowerByFollowedType(accessor).Create(preceding).Build(follows);
                    var xp = xd.ToList();
                    return xp;
                }

                throw new DeclarationNotFoundException(select, this);
            }
        }

        protected override QueryDeclaration CloneSelf(IArgument left, IArgument right) => new(left, right, accessor);
    }

    #region Queries

    private class GetPrecedingByFollowsType(IFollowsAccessor followsAccessor)
    {
        public FollowsQuery Create(IStatementDeclaration followsStatementDeclaration) =>
            followsStatementDeclaration switch
            {
                IStatementDeclaration.Statement => new GetPrecedingByFollowsType<Statement>(followsAccessor),
                IStatementDeclaration.Assign => new GetPrecedingByFollowsType<Assign>(followsAccessor),
                IStatementDeclaration.While => new GetPrecedingByFollowsType<While>(followsAccessor),
                IStatementDeclaration.If => new GetPrecedingByFollowsType<If>(followsAccessor),
                IStatementDeclaration.Call => new GetPrecedingByFollowsType<Call>(followsAccessor),
                _ => throw new ArgumentOutOfRangeException(nameof(followsStatementDeclaration))
            };
    }

    private class GetPrecedingByFollowsType<TFollows>(IFollowsAccessor followsAccessor) : FollowsQuery
        where TFollows : Statement
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration precedingStatementDeclaration) =>
            precedingStatementDeclaration switch
            {
                IStatementDeclaration.Statement => followsAccessor.GetPreceding<TFollows>(),
                IStatementDeclaration.Assign => followsAccessor.GetPreceding<TFollows>().OfType<Assign>(),
                IStatementDeclaration.While => followsAccessor.GetPreceding<TFollows>().OfType<While>(),
                IStatementDeclaration.If => followsAccessor.GetPreceding<TFollows>().OfType<If>(),
                IStatementDeclaration.Call => followsAccessor.GetPreceding<TFollows>().OfType<Call>(),
                _ => throw new ArgumentOutOfRangeException(nameof(precedingStatementDeclaration))
            };
    }

    private class GetFollowerByFollowedType(IFollowsAccessor followsAccessor)
    {
        public FollowsQuery Create(IStatementDeclaration precedingDeclaration) =>
            precedingDeclaration switch
            {
                IStatementDeclaration.Statement => new GetFollowerByFollowedType<Statement>(followsAccessor),
                IStatementDeclaration.Assign => new GetFollowerByFollowedType<Assign>(followsAccessor),
                IStatementDeclaration.While => new GetFollowerByFollowedType<While>(followsAccessor),
                IStatementDeclaration.If => new GetFollowerByFollowedType<If>(followsAccessor),
                IStatementDeclaration.Call => new GetFollowerByFollowedType<Call>(followsAccessor),
                _ => throw new ArgumentOutOfRangeException(nameof(precedingDeclaration))
            };
    }

    private class GetFollowerByFollowedType<TFollower>(IFollowsAccessor followsAccessor) : FollowsQuery
        where TFollower : Statement
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration followsDeclaration) =>
            followsDeclaration switch
            {
                IStatementDeclaration.Statement => followsAccessor.GetFollowers<TFollower>(),
                IStatementDeclaration.Assign => followsAccessor.GetFollowers<TFollower>().OfType<Assign>(),
                IStatementDeclaration.While => followsAccessor.GetFollowers<TFollower>().OfType<While>(),
                IStatementDeclaration.If => followsAccessor.GetFollowers<TFollower>().OfType<If>(),
                IStatementDeclaration.Call => followsAccessor.GetFollowers<TFollower>().OfType<Call>(),
                _ => throw new ArgumentOutOfRangeException(nameof(followsDeclaration))
            };
    }

    private class GetFollowerByLineNumber(IFollowsAccessor followsAccessor, int line) : FollowsQuery
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration follows)
        {
            var precedingStatement = followsAccessor.GetFollower(line);

            var result = follows switch
            {
                IStatementDeclaration.Statement => precedingStatement,
                IStatementDeclaration.Assign => precedingStatement as Assign,
                IStatementDeclaration.While => precedingStatement as While,
                IStatementDeclaration.If => precedingStatement as If,
                IStatementDeclaration.Call => precedingStatement as Call,
                _ => throw new ArgumentOutOfRangeException(nameof(follows))
            };

            if (result is null)
            {
                return Enumerable.Empty<Statement>();
            }

            return Enumerable.Repeat(result, 1);
        }
    }

    private class GetPrecedingByLineNumber(IFollowsAccessor followsAccessor, int line) : FollowsQuery
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration follows)
        {
            var followsStatement = followsAccessor.GetPreceding(line);

            var result = follows switch
            {
                IStatementDeclaration.Statement => followsStatement,
                IStatementDeclaration.Assign => followsStatement as Assign,
                IStatementDeclaration.While => followsStatement as While,
                IStatementDeclaration.If => followsStatement as If,
                IStatementDeclaration.Call => followsStatement as Call,
                _ => throw new ArgumentOutOfRangeException(nameof(follows))
            };

            if (result is null)
            {
                return Enumerable.Empty<Statement>();
            }

            return Enumerable.Repeat(result, 1);
        }
    }

    private class BooleanFollowsQuery(IFollowsAccessor accessor, int left, int right)
    {
        public IEnumerable<IPkbDto> Build() => IPkbDto.Boolean(accessor.IsFollows(left, right));
    }

    private abstract class FollowsQuery
    {
        public abstract IEnumerable<IPkbDto> Build(IStatementDeclaration declaration);
    }

    #endregion
}