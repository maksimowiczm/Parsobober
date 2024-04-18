using Parsobober.Pkb.Ast;

namespace Parsobober.Simple.Parser.Abstractions;

public interface ISimpleExtractor
{
    void Assign(TreeNode node);
    void Expr(TreeNode node);
    void Factor(TreeNode node);
    void Procedure(TreeNode node);
    void Stmt(TreeNode node);
    void StmtLst(TreeNode node);
    void While(TreeNode node);
    void Variable(TreeNode node);
}
