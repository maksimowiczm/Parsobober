using Parsobober.Pql.Query.Tree.Abstraction;

namespace Parsobober.Pql.Query.Tree.Node;

public class BooleanQueryNode(bool result) : IQueryNode
{
    public IEnumerable<IComparable> Do() =>
        result switch
        {
            // xd
            true => Enumerable.Repeat<IComparable>(true, 1),
            _ => Enumerable.Empty<IComparable>()
        };
}