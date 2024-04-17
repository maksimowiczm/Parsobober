using Parsobober.Pkb.Ast;

namespace Parsobober.Simple.Parser.Abstractions
{
    internal interface ISimpleExtractor : ISimpleParser
    {
        TreeNode Assign();
        TreeNode Expr();
        TreeNode Factor();
        TreeNode Procedure();
        TreeNode Stmt();
        TreeNode StmtLst();
        TreeNode While();
    }
}