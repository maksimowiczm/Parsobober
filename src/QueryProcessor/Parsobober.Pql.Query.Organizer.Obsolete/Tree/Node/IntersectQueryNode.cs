using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Organizer.Obsolete.Tree.Abstraction;

namespace Parsobober.Pql.Query.Organizer.Obsolete.Tree.Node;

[Obsolete("Obsolete query organizer. Use Parsobober.Pql.Query.Organizer instead.")]
internal class IntersectQueryNode(IQueryNode a, IQueryNode b) : IQueryNode
{
    public IEnumerable<IPkbDto> Do() => a.Do().Intersect(b.Do());
}