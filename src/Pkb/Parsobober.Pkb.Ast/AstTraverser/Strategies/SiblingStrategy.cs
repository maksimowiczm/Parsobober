using Parsobober.Pkb.Ast.Abstractions;

namespace Parsobober.Pkb.Ast.AstTraverser.Strategies;

/// <summary>
/// Traversal strategy that traverses all right siblings of a node.
/// Depth means sibling distance from the root node.
/// (1 - first right sibling, 2 - second to first right sibling ...)
/// </summary>
public class RightSiblingStrategy : IAstTraversalStrategy
{
    public IEnumerable<(TreeNode node, int depth)> Traverse(TreeNode root)
    {
        for (var (node, depth) = (root.RightSibling, 1); node is not null; node = node.RightSibling, depth++)
        {
            yield return (node, depth);
        }
    }
}

/// <summary>
/// Traversal strategy that traverses all left siblings of a node.
/// Depth means sibling distance from the root node.
/// (1 - first left sibling, 2 - second to first left sibling ...)
/// </summary>
public class LeftSiblingStrategy : IAstTraversalStrategy
{
    public IEnumerable<(TreeNode node, int depth)> Traverse(TreeNode root)
    {
        for (var (node, depth) = (root.LeftSibling, 1); node is not null; node = node.LeftSibling, depth++)
        {
            yield return (node, depth);
        }
    }
}