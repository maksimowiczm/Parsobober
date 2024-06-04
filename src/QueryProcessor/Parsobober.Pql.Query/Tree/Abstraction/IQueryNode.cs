namespace Parsobober.Pql.Query.Tree.Abstraction;

public interface IQueryNode
{
    IEnumerable<IComparable> Do();
}