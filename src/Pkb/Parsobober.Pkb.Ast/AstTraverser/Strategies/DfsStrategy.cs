using Parsobober.Pkb.Ast.Abstractions;

namespace Parsobober.Pkb.Ast.AstTraverser.Strategies;

public class DfsStrategy : IAstTraversalStrategy
{
    public IEnumerable<(TreeNode node, int depth)> Traverse(TreeNode root)
    {
        var stack = new Stack<(TreeNode node, int depth)>();

        stack.Push((root, 0));

        while (stack.Count > 0)
        {
            var (node, depth) = stack.Pop();

            if (node != root)
            {
                yield return (node, depth);
            }

            foreach (var child in node.Children.Reverse().Where(IsNodeValid))
            {
                stack.Push((child, depth + 1));
            }
        }
    }

    protected virtual bool IsNodeValid(TreeNode? node) => node is not null;
}

public class DfsStatementStrategy : DfsStrategy
{
    protected override bool IsNodeValid(TreeNode? node) =>
        node is not null &&
        (node.Type.IsStatement() ||
         node.Type == EntityType.StatementsList ||
         node.Type == EntityType.Procedure);
}