using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query.Queries;

internal static class FollowsTransitive
{
    #region Builder

    public class Builder(IFollowsAccessor accessor)
    {
        private readonly List<(string left, string right)> _followsRelations = [];

        public void Add(string left, string right)
        {
            _followsRelations.Add((left, right));
        }

        public IEnumerable<Statement>? Build(string select, IReadOnlyDictionary<string, IDeclaration> declarations)
        {
            return null;
        }
    }

    #endregion
}