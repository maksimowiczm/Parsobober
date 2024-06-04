namespace Parsobober.Pql.Query.Organizer.Obsolete.Tree.Abstraction;

[Obsolete("Obsolete query organizer. Use Parsobober.Pql.Query.Organizer instead.")]
public interface IQueryNode
{
    IEnumerable<IComparable> Do();
}