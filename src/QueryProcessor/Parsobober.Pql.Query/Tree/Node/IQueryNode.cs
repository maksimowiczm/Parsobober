namespace Parsobober.Pql.Query.Tree.Node;

internal interface IQueryNode
{
    IEnumerable<IComparable> Do();
}