using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Organizer.Obsolete.Tree.Abstraction;

namespace Parsobober.Pql.Query.Organizer.Obsolete.Tree.Node;

/// <summary>
/// Query that returns all elements of select type. 
/// </summary>
[Obsolete("Obsolete query organizer. Use Parsobober.Pql.Query.Organizer instead.")]
public class PkbQueryNode(IDeclaration select, IDtoProgramContextAccessor context) : IQueryNode
{
    public IEnumerable<IPkbDto> Do() => select.ExtractFromContext(context);

#if DEBUG
    private List<IPkbDto> Result => Do().ToList();
#endif
}