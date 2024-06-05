using Parsobober.Pql.Query.Organizer.Obsolete.Tree.Abstraction;

namespace Parsobober.Pql.Query.Organizer.Obsolete.Tree.Node;

[Obsolete("Obsolete query organizer. Use Parsobober.Pql.Query.Organizer instead.")]
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