using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pql.Query.Arguments;

namespace Parsobober.Pql.Query.Tree.Node;

/// <summary>
/// 
/// </summary>
internal class AmbiguousQueryNode(IDeclaration select, IDtoProgramContextAccessor context) : IQueryNode
{
    public IEnumerable<IComparable> Do() => select switch
    {
        IStatementDeclaration.Statement => context.Statements,
        IStatementDeclaration.Assign => context.Assigns,
        IStatementDeclaration.While => context.Whiles,
        IVariableDeclaration.Variable => context.Variables,
        _ => throw new Exception("idk")
    };

#if DEBUG
    private List<IComparable> Result => Do().ToList();
#endif
}