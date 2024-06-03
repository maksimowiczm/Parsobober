using Parsobober.Pql.Query.Tree.Abstraction;

namespace Parsobober.Pql.Query.Tree.Node;

internal class IntersectQueryNode(IQueryNode a, IQueryNode b) : IQueryNode
{
    public IEnumerable<IComparable> Do() => a.Do().Intersect(b.Do());
}