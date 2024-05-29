using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Tree.Abstraction;

namespace Parsobober.Pql.Query.Tree;

internal class QueryContainerBuilder : IQueryContainer.IQueryContainerBuilder
{
    private readonly List<IQueryDeclaration> _queries = [];

    private readonly List<IAttributeQuery> _attributes = [];

    public void AddQuery(IQueryDeclaration query) => _queries.Add(query);

    public void AddAttribute(IAttributeQuery attributeQuery) => _attributes.Add(attributeQuery);

    public IQueryContainer Build()
    {
        var queries = _queries
            .Select(q =>
            {
                var attribute = _attributes.Where(a => a.Declaration == q.Left || a.Declaration == q.Right);
                return attribute.Aggregate(q, (current, a) => a.ApplyAttribute(current));
            });

        return new QueryContainer(queries.ToList(), _attributes);
    }
}