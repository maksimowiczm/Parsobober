using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Tree.Abstraction;

namespace Parsobober.Pql.Query.Tree.Node;

/// <summary>
/// Returns intersection of <see cref="IQueryNode"/> result and <see cref="IAttributeQuery"/>.
/// </summary>
[Obsolete("XD")]
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
        // todo might have to refactor so that attributeQuery use queryNode result as input instead of doing it's own pkb query
        var queryResult = _queryNode.Do();
        var attributeResult = _attributeQuery.Do();
        var intersection = queryResult.Intersect(attributeResult);

        return intersection;
    }

#if DEBUG
    private List<IComparable> Result => Do().ToList();
#endif
}