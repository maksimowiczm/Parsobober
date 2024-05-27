using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Tree.Exceptions;

namespace Parsobober.Pql.Query.Tree.Node;

/// <summary>
/// Query that returns all elements of select type. 
/// </summary>
internal class PkbQueryNode(IDeclaration select, IDtoProgramContextAccessor context) : IQueryNode
{
    public IEnumerable<IComparable> Do() => select switch
    {
        IStatementDeclaration.Statement => context.Statements,
        IStatementDeclaration.Assign => context.Assigns,
        IStatementDeclaration.While => context.Whiles,
        IStatementDeclaration.If => context.Ifs,
        IStatementDeclaration.Call => context.Calls,
        IVariableDeclaration.Variable => context.Variables,
        _ => throw new DeclarationNotSupported(select, "Given declaration is not supported.")
    };

#if DEBUG
    private List<IComparable> Result => Do().ToList();
#endif
}