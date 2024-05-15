using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query.Queries;

internal static class Uses
{
    #region Builder

    public class Builder(IUsesAccessor accessor)
    {
        private readonly List<(string left, string right)> _usesRelations = [];

        public void Add(string left, string right)
        {
            _usesRelations.Add((left, right));
        }

        public IEnumerable<Statement>? Build(string select, IReadOnlyDictionary<string, IDeclaration> declarations)
        {
            return null;
        }
    }

    #endregion
}