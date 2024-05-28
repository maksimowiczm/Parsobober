namespace Parsobober.Pql.Query.Tree.Abstraction;

internal interface IQueryNode
{
    IEnumerable<IComparable> Do();
}