using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Tree.Node;

/// <summary>
/// Returns result if condition is met.
/// Ambiguous because it doesn't know if it should return left or right side.
/// We go with left side. Might change in the future. Clueless.
/// </summary>
internal class AmbiguousConditionalQueryNode : IQueryNode
{
    private readonly IQueryDeclaration _condition;
    private readonly IEnumerable<IComparable> _result;

    public AmbiguousConditionalQueryNode(
        IDeclaration select,
        IQueryDeclaration condition,
        IDtoProgramContextAccessor context
    )
    {
        _condition = condition;
        _result = select switch
        {
            IStatementDeclaration.Statement => context.Statements,
            IStatementDeclaration.Assign => context.Assigns,
            IStatementDeclaration.While => context.Whiles,
            IVariableDeclaration.Variable => context.Variables,
            _ => throw new Exception("idk")
        };
    }

    public IEnumerable<IComparable> Do()
    {
        if (_condition.Do().Any())
        {
            return _result;
        }

        return [];
    }
}