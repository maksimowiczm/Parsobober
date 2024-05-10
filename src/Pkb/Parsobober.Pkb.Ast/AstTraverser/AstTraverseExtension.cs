using Parsobober.Pkb.Ast.Abstractions;

namespace Parsobober.Pkb.Ast.AstTraverser;

public static class AstTraverseExtension
{
    public static IEnumerable<(TreeNode node, int depth)> Traverse(this TreeNode root, IAstTraversalStrategy strategy)
    {
        return strategy.Traverse(root);
    }
}