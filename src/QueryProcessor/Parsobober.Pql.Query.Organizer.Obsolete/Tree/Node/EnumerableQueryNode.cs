using Parsobober.Pql.Query.Organizer.Obsolete.Tree.Abstraction;

namespace Parsobober.Pql.Query.Organizer.Obsolete.Tree.Node;

/// <summary>
/// Query node that returns raw enumerable. The most simple query node.
/// </summary>
[Obsolete("Obsolete query organizer. Use Parsobober.Pql.Query.Organizer instead.")]
public class EnumerableQueryNode : IQueryNode
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