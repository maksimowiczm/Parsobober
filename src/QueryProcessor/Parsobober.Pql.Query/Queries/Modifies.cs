using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query.Queries;

internal static class Modifies
{
    #region Builder

    public class Builder(IModifiesAccessor accessor)
    {
        private readonly List<(string left, string right)> _modifiesRelations = [];

        public void Add(string left, string right)
        {
            _modifiesRelations.Add((left, right));
        }

        public IEnumerable<Statement>? Build(string select, IReadOnlyDictionary<string, IDeclaration> declarations)
        {
            return null;
        }
    }

    #endregion
}