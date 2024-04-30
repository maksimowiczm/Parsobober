using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query.Queries;

internal static class Follows
{
    #region Builder

    public class Builder(IFollowsAccessor accessor)
    {
        private readonly List<(string followed, string follows)> _followsRelations = [];

        public void Add(string followed, string follows)
        {
            _followsRelations.Add((followed, follows));
        }

        public IEnumerable<Statement> Build(string select, IReadOnlyDictionary<string, IDeclaration> declarations)
        {
            var followed = _followsRelations.First().followed;
            var follows = _followsRelations.First().follows;

            var query = new InnerBuilder(accessor, select, declarations).Build(followed, follows);

            return query;
        }
    }

    private class InnerBuilder(
        IFollowsAccessor accessor,
        string select,
        IReadOnlyDictionary<string, IDeclaration> declarations
    )
    {
        public IEnumerable<Statement> Build(string followedStr, string followsStr)
        {
            // parsowanie argumentów
            var followedArgument = IArgument.Parse(declarations, followedStr);
            var followsArgument = IArgument.Parse(declarations, followsStr);

            // pattern matching argumentów
            var query = (followedArgument, followsArgument) switch
            {
                // Follows(stmt, 1)
                (IStatementDeclaration declaration, IArgument.Line follows) =>
                  new GetFollowsByLineNumber(accessor, follows.Value).Build(declaration),

                //Follows(1, stmt) 
                (IArgument.Line followed, IStatementDeclaration follows) =>
                new GetFollowedByLineNumber(accessor, followed.Value).Build(follows),

                // Follows(stmt, stmt)
                (IStatementDeclaration followed, IStatementDeclaration follows) =>
                    BuildParentWithSelect((followedStr, followed), (followsStr, follows)),

                // Follows(1, 2) nie wspierane w tej wersji
                _ => throw new InvalidOperationException("Invalid query")
            };

            return query;
        }

        private IEnumerable<Statement> BuildParentWithSelect(
            (string key, IStatementDeclaration type) followed,
            (string key, IStatementDeclaration type) follows
        )
        {
            // tu nastąpi samowywrotka przy zapytaniach, w których nie ma wartości z selecta
            // przykład: Select x such that Parent(a, b)

            if (followed.key == select)
            {
                return new GetFollowedByFollowsType(accessor).Create(followed.type).Build(follows.type);
            }

            if (follows.key == select)
            {
                return new GetFollowsByFollowedType(accessor).Create(follows.type).Build(followed.type);
            }

            throw new InvalidOperationException("Invalid query");
        }
    }

    #endregion

    #region Queries

    private class GetFollowedByFollowsType(IFollowsAccessor followsAccessor)
    {
        public FollowsQuery Create(IStatementDeclaration parentStatementDeclaration) =>
            parentStatementDeclaration switch
            {
                IStatementDeclaration.Statement => new GetFollowedByFollowsType<Statement>(followsAccessor),
                IStatementDeclaration.Assign => new GetFollowedByFollowsType<Assign>(followsAccessor),
                IStatementDeclaration.While => new GetFollowedByFollowsType<While>(followsAccessor),
                _ => throw new ArgumentOutOfRangeException(nameof(parentStatementDeclaration))
            };
    }

    /// <summary>
    /// Gets followed of given type by follows type.
    /// </summary>
    /// <param name="followsAccessor">Follows accessor.</param>
    /// <typeparam name="TFollowed">Followed type.</typeparam>
    private class GetFollowedByFollowsType<TFollowed>(IFollowsAccessor followsAccessor) : FollowsQuery
        where TFollowed : Statement
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration childStatementDeclaration) =>
            childStatementDeclaration switch
            {
                IStatementDeclaration.Statement => followsAccessor.GetFollowed<TFollowed>(),
                IStatementDeclaration.Assign => followsAccessor.GetFollowed<TFollowed>().OfType<Assign>(),
                IStatementDeclaration.While => followsAccessor.GetFollowed<TFollowed>().OfType<While>(),
                _ => throw new ArgumentOutOfRangeException(nameof(childStatementDeclaration))
            };
    }

    private class GetFollowsByFollowedType(IFollowsAccessor followsAccessor)
    {
        public FollowsQuery Create(IStatementDeclaration declaration) =>
            declaration switch
            {
                IStatementDeclaration.Statement => new GetFollowedByParentType<Statement>(followsAccessor),
                IStatementDeclaration.Assign => new GetFollowedByParentType<Assign>(followsAccessor),
                IStatementDeclaration.While => new GetFollowedByParentType<While>(followsAccessor),
                _ => throw new ArgumentOutOfRangeException(nameof(declaration))
            };
    }

    /// <summary>
    /// Gets follows of given type by followed type.
    /// </summary>
    /// <param name="followsAccessor">Followed accessor.</param>
    /// <typeparam name="TFollows">Follows type.</typeparam>
    private class GetFollowedByParentType<TFollows>(IFollowsAccessor followsAccessor) : FollowsQuery
        where TFollows : Statement
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration declaration) =>
            declaration switch
            {
                IStatementDeclaration.Statement => followsAccessor.GetFollowed<TFollows>(),
                IStatementDeclaration.Assign => followsAccessor.GetFollowed<TFollows>().OfType<Assign>(),
                IStatementDeclaration.While => followsAccessor.GetFollowed<TFollows>().OfType<While>(),
                _ => throw new ArgumentOutOfRangeException(nameof(declaration))
            };
    }

    /// <summary>
    /// Get Followed of given type by follows line number.
    /// </summary>
    /// <param name="followsAccessor">Followed accessor.</param>
    /// <param name="line">Line number.</param>
    private class GetFollowedByLineNumber(IFollowsAccessor followsAccessor, int line) : FollowsQuery
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration follows)
        {
            var followsStatement = followsAccessor.GetFollowed(line);

            var result = follows switch
            {
                IStatementDeclaration.Statement => followsStatement,
                IStatementDeclaration.Assign => followsStatement as Assign,
                IStatementDeclaration.While => followsStatement as While,
                _ => throw new ArgumentOutOfRangeException(nameof(follows))
            };

            if (result is null)
            {
                return Enumerable.Empty<Statement>();
            }

            return Enumerable.Repeat(result, 1);
        }
    }

    /// <summary>
    /// Gets follows of given type by followed line number.
    /// </summary>
    /// <param name="followsAccessor">Followed accessor.</param>
    /// <param name="line">Line number.</param>
    private class GetFollowsByLineNumber(IFollowsAccessor followsAccessor, int line) : FollowsQuery
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration follows)
        {
            var followsStatement = followsAccessor.GetFollower(line);

            var result = follows switch
            {
                IStatementDeclaration.Statement => followsStatement,
                IStatementDeclaration.Assign => followsStatement as Assign,
                IStatementDeclaration.While => followsStatement as While,
                _ => throw new ArgumentOutOfRangeException(nameof(follows))
            };

            if (result is null)
            {
                return Enumerable.Empty<Statement>();
            }

            return Enumerable.Repeat(result, 1);
        }
    }

    /// <summary>
    /// Represents a follows query.
    /// </summary>
    private abstract class FollowsQuery
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