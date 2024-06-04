using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Tree.Abstraction;

namespace Parsobober.Pql.Query.Tree;

[Obsolete("XD")]
internal class QueryOrganizerBuilder(IDtoProgramContextAccessor context) : IQueryOrganizerBuilder
{
    private readonly List<IQueryDeclaration> _queries = [];

    private readonly List<IAttributeQuery> _attributes = [];

    public void AddQuery(IQueryDeclaration query) => _queries.Add(query);

    public void AddAttribute(IAttributeQuery attributeQuery) => _attributes.Add(attributeQuery);

    public IQueryOrganizer Build()
    {
        var queries = _queries
            .Select(q =>
            {
                var attribute = _attributes.Where(a => a.Declaration == q.Left || a.Declaration == q.Right);
                return attribute.Aggregate(q, (current, a) => a.ApplyAttribute(current));
            });

        var container = new QueryContainer(queries.ToList(), _attributes);

        return new QueryOrganizer(container, context);
    }
}