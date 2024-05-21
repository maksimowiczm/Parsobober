using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Tree.Node;

internal interface IQueryNode
{
    IDeclaration Select { get; }
    ILazyQuery Query { get; }
}