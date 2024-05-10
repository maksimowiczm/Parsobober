using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query.Queries;

internal static class ParentTransitive
{
    #region Builder

    public class Builder(IParentAccessor accessor)
    {
        private readonly List<(string parent, string child)> _parentRelations = [];

        public void Add(string parent, string child)
        {
            _parentRelations.Add((parent, child));
        }

        public IEnumerable<Statement>? Build(string select, IReadOnlyDictionary<string, IDeclaration> declarations)
        {
            return null;
        }
    }

    #endregion
}