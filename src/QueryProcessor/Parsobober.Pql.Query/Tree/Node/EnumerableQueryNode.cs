namespace Parsobober.Pql.Query.Tree.Node;

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