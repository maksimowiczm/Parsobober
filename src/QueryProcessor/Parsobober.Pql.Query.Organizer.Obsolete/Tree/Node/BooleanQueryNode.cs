using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Organizer.Obsolete.Tree.Abstraction;

namespace Parsobober.Pql.Query.Organizer.Obsolete.Tree.Node;

[Obsolete("Obsolete query organizer. Use Parsobober.Pql.Query.Organizer instead.")]
public class BooleanQueryNode(bool result) : IQueryNode
{
    public IEnumerable<IPkbDto> Do() => IPkbDto.Boolean(result);
}