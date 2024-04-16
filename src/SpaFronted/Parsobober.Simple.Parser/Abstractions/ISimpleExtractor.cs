using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Ast.Abstractions;

namespace Parsobober.Simple.Parser.Abstractions
{
    internal interface ISimpleExtractor
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