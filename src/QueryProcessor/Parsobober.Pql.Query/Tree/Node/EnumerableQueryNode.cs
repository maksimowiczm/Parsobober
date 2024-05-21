namespace Parsobober.Pql.Query.Tree.Node;

internal record EnumerableQueryNode(IEnumerable<IComparable> QueryResult) : IQueryNode
{
    public IEnumerable<IComparable> Do() => QueryResult;
}