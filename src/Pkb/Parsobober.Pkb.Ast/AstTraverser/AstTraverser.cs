using Parsobober.Pkb.Ast.Abstractions;

namespace Parsobober.Pkb.Ast.AstTraverser;

public static class AstTraverser
{
    public static IEnumerable<(TreeNode node, int depth)> Traverse(TreeNode root, IAstTraversalStrategy strategy)
    {
        return strategy.Traverse(root);
    }
}