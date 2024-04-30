using Parsobober.Pkb.Ast.Abstractions;

namespace Parsobober.Pkb.Ast.AstTraverser;

public class AstTraverser
{
    public IEnumerable<(TreeNode node, int depth)> Traverse(TreeNode root, IAstTraversalStrategy strategy)
    {
        return strategy.Traverse(root);
    }
}