using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries;
using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Tree.Node;

internal class BooleanQueryNode(
    IDeclaration select,
    IQueryDeclaration predicate,
    Func<IEnumerable<IComparable>> query
) : IQueryNode
{
    public IDeclaration Select { get; } = select;

    public ILazyQuery Query =>
        predicate switch
        {
            { Left: IDeclaration left, Right: not IDeclaration } when predicate.Do(left).Any() =>
                new RawQuery(query),
            { Left: not IDeclaration, Right: IDeclaration right } when predicate.Do(right).Any() =>
                new RawQuery(query),
            _ => throw new Exception("Unambiguous query")
        };
}