using Parsobober.Pkb.Ast.Abstractions;

namespace Parsobober.Pkb.Ast.AstTraverser.Strategies;

/// <summary>
/// Ast traversal strategy that traverses only parent nodes (straight up).
/// As a depth it returns the distance from the given node.
/// </summary>
public class OnlyParentStrategy : IAstTraversalStrategy
{
    public IEnumerable<(TreeNode node, int depth)> Traverse(TreeNode root)
    {
        for (var (node, depth) = (root.Parent, 1); node is not null; node = node.Parent, depth++)
        {
            yield return (node, depth);
        }
    }
}