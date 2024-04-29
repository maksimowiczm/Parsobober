using Parsobober.Pkb.Ast.Abstractions;

namespace Parsobober.Pkb.Ast.AstTraverser.Strategies;

public class BfsStrategy : IAstTraversalStrategy
{
    public IEnumerable<(TreeNode node, int depth)> Traverse(TreeNode root)
    {
        var queue = new Queue<(TreeNode node, int depth)>();
        queue.Enqueue((root, 0));

        while (queue.Count > 0)
        {
            var (node, depth) = queue.Dequeue();

            if (node != root)
            {
                yield return (node, depth);
            }

            foreach (var child in node.Children.Where(IsNodeValid))
            {
                queue.Enqueue((child, depth + 1));
            }
        }
    }

    protected virtual bool IsNodeValid(TreeNode? node) => node is not null;
}

public class BfsStatementStrategy : BfsStrategy
{
    protected override bool IsNodeValid(TreeNode? node) =>
        node is not null &&
        (node.Type.IsStatement() ||
         node.Type == EntityType.StatementsList ||
         node.Type == EntityType.Procedure);
}