using Parsobober.Pql.Query.Tree.Abstraction;

namespace Parsobober.Pql.Query.Tree.Node;

/// <summary>
/// Query node that returns raw enumerable. The most simple query node.
/// </summary>
internal class EnumerableQueryNode : IQueryNode
{
    private readonly IEnumerable<IComparable> _queryResult;

    public EnumerableQueryNode(IEnumerable<IComparable> queryResult)
    {
        _queryResult = queryResult;
    }

    public IEnumerable<IComparable> Do() => _queryResult;

#if DEBUG
    private List<IComparable> Result => Do().ToList();
#endif
}