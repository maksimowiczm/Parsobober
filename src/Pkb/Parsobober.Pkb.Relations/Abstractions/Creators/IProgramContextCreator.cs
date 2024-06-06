using Parsobober.Pkb.Ast;

namespace Parsobober.Pkb.Relations.Abstractions.Creators;

public interface IProgramContextCreator
{
    public bool TryAddVariable(TreeNode variable);
    public bool TryAddStatement(TreeNode statement);
    public bool TryAddProcedure(TreeNode procedure);
    public bool TryAddStatementList(TreeNode statementList);
    public bool TryAddConstant(TreeNode constant);
}