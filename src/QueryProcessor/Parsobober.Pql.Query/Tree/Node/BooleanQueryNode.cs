using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Tree.Node;

internal class BooleanQueryNode(IQueryDeclaration predicate, IEnumerable<IComparable> query) : IQueryNode
{
    public IEnumerable<IComparable> Do() =>
        predicate switch
        {
            { Left: IDeclaration left, Right: not IDeclaration } when predicate.Do(left).Any() => query,
            { Left: not IDeclaration, Right: IDeclaration right } when predicate.Do(right).Any() => query,
            // todo think about resolving this case
            { Left: IDeclaration left, Right: IDeclaration right } when predicate.Do(left).Any() &&
                                                                        predicate.Do(right).Any() => query,
            _ => []
        };
}