using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Tree.Node;

internal class BooleanQueryNode : IQueryNode
{
    private readonly IQueryDeclaration? _predicate;
    private readonly IQueryNode? _predicateNode;
    private readonly IEnumerable<IComparable> _query;

    public BooleanQueryNode(IQueryDeclaration predicate, IEnumerable<IComparable> query)
    {
        _predicate = predicate;
        _query = query;
    }

    public BooleanQueryNode(IQueryNode node, IEnumerable<IComparable> query)
    {
        _predicateNode = node;
        _query = query;
    }

    public IEnumerable<IComparable> Do()
    {
        if (_predicate is not null)
        {
            return _predicate switch
            {
                { Left: IDeclaration left, Right: not IDeclaration } when _predicate.Do(left).Any() => _query,
                { Left: not IDeclaration, Right: IDeclaration right } when _predicate.Do(right).Any() => _query,
                // todo think about resolving this case
                { Left: IDeclaration left, Right: IDeclaration right } when _predicate.Do(left).Any() &&
                                                                            _predicate.Do(right).Any() => _query,
                _ => []
            };
        }

        if (_predicateNode!.Do().Any())
        {
            return _query;
        }

        return [];
    }
}