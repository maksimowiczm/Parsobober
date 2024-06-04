using Parsobober.Pql.Query.Tree.Abstraction;

namespace Parsobober.Pql.Query.Tree.Node;

/// <summary>
/// Returns result if condition is met.
/// </summary>
[Obsolete("XD")]
internal class ConditionalQueryNode : IQueryNode
{
    private readonly IQueryNode _condition;
    private readonly IQueryNode _result;

    public ConditionalQueryNode(IQueryNode condition, IQueryNode result)
    {
        _condition = condition;
        _result = result;
    }

    public IEnumerable<IComparable> Do()
    {
        if (_condition.Do().Any())
        {
            return _result.Do();
        }

        return [];
    }

#if DEBUG
    private List<IComparable> Result => Do().ToList();
#endif
}