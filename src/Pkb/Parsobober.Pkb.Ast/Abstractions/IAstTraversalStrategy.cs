namespace Parsobober.Pkb.Ast.Abstractions;

public interface IAstTraversalStrategy
{
    IEnumerable<(TreeNode node, int depth)> Traverse(TreeNode root);
}