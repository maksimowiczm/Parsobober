using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query.Queries;

internal static class FollowsTransitive
{
    #region Builder

    public class Builder(IFollowsAccessor accessor)
    {
        private readonly List<(string followed, string follows)> _followsRelations = [];

        public void Add(string followed, string follows)
        {
            _followsRelations.Add((followed, follows));
        }

        public IEnumerable<Statement>? Build(string select, IReadOnlyDictionary<string, IDeclaration> declarations)
        {
            if (_followsRelations.Count == 0)
            {
                return null;
            }


            if (_followsRelations.Count > 1)
            {
                throw new InvalidOperationException("Invalid query");
            }

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
                // followed*(stmt, 1)
                (IStatementDeclaration declaration, IArgument.Line follows) =>
                    new GetTransitiveFollowedByLineNumber(accessor, follows.Value).Build(declaration),

                // followed*(1, stmt)
                (IArgument.Line followed, IStatementDeclaration follows) =>
                    new GetTransitiveFollowsByLineNumber(accessor, followed.Value).Build(follows),

                // followed*(stmt, stmt)
                (IStatementDeclaration followed, IStatementDeclaration follows) =>
                    BuildfollowedWithSelect((followedStr, followed), (followsStr, follows)),

                // followed*(1, 2) nie wspierane w tej wersji
                _ => throw new InvalidOperationException("Invalid query")
            };

            return query;
        }

        private IEnumerable<Statement> BuildfollowedWithSelect(
            (string key, IStatementDeclaration type) followed,
            (string key, IStatementDeclaration type) follows
        )
        {
            // tu nastąpi samowywrotka przy zapytaniach, w których nie ma wartości z selecta
            // przykład: Select x such that followed(a, b)

            if (followed.key == select)
            {
                return new GetTransitiveFollowedByFollowsType(accessor).Create(follows.type).Build(followed.type);
            }

            if (follows.key == select)
            {
                return new GetTransitiveFollowsByFollowedType(accessor).Create(followed.type).Build(follows.type);
            }

            throw new InvalidOperationException("Invalid query");
        }
    }

    #endregion

    #region Queries

    private class GetTransitiveFollowedByFollowsType(IFollowsAccessor followedAccessor)
    {
        public followedQuery Create(IStatementDeclaration followsStatementDeclaration) =>
            followsStatementDeclaration switch
            {
                IStatementDeclaration.Statement => new GetTransitiveFollowedByFollowsType<Statement>(followedAccessor),
                IStatementDeclaration.Assign => new GetTransitiveFollowedByFollowsType<Assign>(followedAccessor),
                IStatementDeclaration.While => new GetTransitiveFollowedByFollowsType<While>(followedAccessor),
                _ => throw new ArgumentOutOfRangeException(nameof(followsStatementDeclaration))
            };
    }

    /// <summary>
    /// Gets transitive followeds of given type by follows type.
    /// </summary>
    /// <param name="followedAccessor">followed accessor.</param>
    /// <typeparam name="Tfollows">follows type.</typeparam>
    private class GetTransitiveFollowedByFollowsType<Tfollows>(IFollowsAccessor followedAccessor) : followedQuery
        where Tfollows : Statement
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration followsStatementDeclaration) =>
            followsStatementDeclaration switch
            {
                IStatementDeclaration.Statement => followedAccessor.GetFollowedTransitive<Tfollows>(),
                IStatementDeclaration.Assign => followedAccessor.GetFollowedTransitive<Tfollows>().OfType<Assign>(),
                IStatementDeclaration.While => followedAccessor.GetFollowedTransitive<Tfollows>().OfType<While>(),
                _ => throw new ArgumentOutOfRangeException(nameof(followsStatementDeclaration))
            };
    }

    private class GetTransitiveFollowsByFollowedType(IFollowsAccessor followedAccessor)
    {
        public followedQuery Create(IStatementDeclaration followedStatementDeclaration) =>
            followedStatementDeclaration switch
            {
                IStatementDeclaration.Statement => new GetTransitiveFollowsByFollowedType<Statement>(
                    followedAccessor),
                IStatementDeclaration.Assign => new GetTransitiveFollowsByFollowedType<Assign>(followedAccessor),
                IStatementDeclaration.While => new GetTransitiveFollowsByFollowedType<While>(followedAccessor),
                _ => throw new ArgumentOutOfRangeException(nameof(followedStatementDeclaration))
            };
    }

    /// <summary>
    /// Gets transitive followsren of given type by followed type.
    /// </summary>
    /// <param name="followedAccessor">followed accessor.</param>
    /// <typeparam name="Tfollowed">followed type.</typeparam>
    private class GetTransitiveFollowsByFollowedType<Tfollowed>(IFollowsAccessor followedAccessor) : followedQuery
        where Tfollowed : Statement
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration followsStatementDeclaration) =>
            followsStatementDeclaration switch
            {
                IStatementDeclaration.Statement => followedAccessor.GetFollowersTransitive<Tfollowed>(),
                IStatementDeclaration.Assign => followedAccessor.GetFollowersTransitive<Tfollowed>().OfType<Assign>(),
                IStatementDeclaration.While => followedAccessor.GetFollowersTransitive<Tfollowed>().OfType<While>(),
                _ => throw new ArgumentOutOfRangeException(nameof(followsStatementDeclaration))
            };
    }

    /// <summary>
    /// Get transitive followed of given type by follows line number.
    /// </summary>
    /// <param name="followedAccessor">followed accessor.</param>
    /// <param name="line">Line number.</param>
    private class GetTransitiveFollowedByLineNumber(IFollowsAccessor followedAccessor, int line) : followedQuery
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration followsStatementDeclaration) =>
            followsStatementDeclaration switch
            {
                IStatementDeclaration.Statement => followedAccessor.GetFollowedTransitive(line),
                IStatementDeclaration.Assign => followedAccessor.GetFollowedTransitive(line).OfType<Assign>(),
                IStatementDeclaration.While => followedAccessor.GetFollowedTransitive(line).OfType<While>(),
                _ => throw new ArgumentOutOfRangeException(nameof(followsStatementDeclaration))
            };
    }

    /// <summary>
    /// Gets transitive followsren of given type by followed line number.
    /// </summary>
    /// <param name="followedAccessor">followed accessor.</param>
    /// <param name="line">Line number.</param>
    private class GetTransitiveFollowsByLineNumber(IFollowsAccessor followedAccessor, int line) : followedQuery
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration followsStatementDeclaration) =>
            followsStatementDeclaration switch
            {
                IStatementDeclaration.Statement => followedAccessor.GetFollowersTransitive(line),
                IStatementDeclaration.Assign => followedAccessor.GetFollowersTransitive(line).OfType<Assign>(),
                IStatementDeclaration.While => followedAccessor.GetFollowersTransitive(line).OfType<While>(),
                _ => throw new ArgumentOutOfRangeException(nameof(followsStatementDeclaration))
            };
    }

    /// <summary>
    /// Represents a followed query.
    /// </summary>
    private abstract class followedQuery
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