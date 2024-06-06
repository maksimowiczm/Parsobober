using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Organizer.Obsolete.Tree.Abstraction;

namespace Parsobober.Pql.Query.Organizer.Obsolete.Tree.Node;

/// <summary>
/// Query node that returns raw enumerable. The most simple query node.
/// </summary>
[Obsolete("Obsolete query organizer. Use Parsobober.Pql.Query.Organizer instead.")]
public class EnumerableQueryNode : IQueryNode
{
    private readonly IEnumerable<IPkbDto> _queryResult;

    public EnumerableQueryNode(IEnumerable<IPkbDto> queryResult)
    {
        _queryResult = queryResult;
    }

    public IEnumerable<IPkbDto> Do() => _queryResult;

#if DEBUG
    private List<IPkbDto> Result => Do().ToList();
#endif
}