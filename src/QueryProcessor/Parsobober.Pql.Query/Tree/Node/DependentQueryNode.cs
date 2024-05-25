using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Tree.Node;

internal class DependentQueryNode : IQueryNode
{
    private readonly IQueryDeclaration _conditionQuery;
    private readonly IDeclaration _conditionSelect;
    private readonly IQueryNode _dependent;

    public DependentQueryNode(IQueryDeclaration conditionQuery, IDeclaration conditionSelect, IQueryNode dependent)
    {
        _conditionQuery = conditionQuery;
        _conditionSelect = conditionSelect;
        _dependent = dependent;
    }

    public IEnumerable<IComparable> Do()
    {
        // god forgive me
        // I am doing this only to pass my studies

        var dependentResult = _dependent.Do().Select(r =>
            // 💀💀💀💀💀💀💀💀💀
            IArgument.Parse(new Dictionary<string, IDeclaration>(), r.ToString()!)
        );

        var result = dependentResult
            .SelectMany(arg =>
                _conditionQuery
                    .ReplaceArgument(_conditionSelect, arg)
                    .Do(_conditionSelect)
            )
            .Distinct();

        return result;
    }

#if DEBUG
    private List<IComparable> Result => Do().ToList();
#endif
}