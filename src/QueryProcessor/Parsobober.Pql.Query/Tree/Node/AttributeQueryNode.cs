using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Tree.Node;

internal class AttributeQueryNode(
    IAttributeQuery attributeQuery,
    IQueryNode queryNode
) : IQueryNode
{
    public IEnumerable<IComparable> Do()
    {
        var queryResult = queryNode.Do();
        var attributeResult = attributeQuery.Do();
        var intersection = queryResult.Intersect(attributeResult);
        
        return intersection; 
    }
}