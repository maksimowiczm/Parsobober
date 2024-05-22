using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Tree.Node;

/// <summary>
/// Returns intersection of query and attribute.
/// </summary>
internal class AttributeQueryNode : IQueryNode
{
    private readonly IAttributeQuery _attributeQuery;
    private readonly IQueryNode _queryNode;

    public AttributeQueryNode(IAttributeQuery attributeQuery, IQueryNode queryNode)
    {
        _attributeQuery = attributeQuery;
        _queryNode = queryNode;
    }

    public IEnumerable<IComparable> Do()
    {
        // intersection of query and attribute
        var queryResult = _queryNode.Do();
        var attributeResult = _attributeQuery.Do();
        var intersection = queryResult.Intersect(attributeResult);

        return intersection;
    }
}