using Parsobober.Pkb.Ast.Abstractions;

namespace Parsobober.Pkb.Ast;

public class Ast : IAst
{
    public TreeNode Root { get; } = new(0, EntityType.Program);

    public TreeNode CreateTreeNode(int lineNumber, EntityType type)
    {
        var newNode = new TreeNode(lineNumber, type);

        return newNode;
    }

    public void SetAttribute(TreeNode node, string attr)
    {
        node.Attribute = attr;
    }

    public void SetSiblings(TreeNode left, TreeNode right)
    {
        left.RightSibling = right;
        right.LeftSibling = left;
    }

    public int SetParenthood(TreeNode parent, TreeNode child)
    {
        child.Parent = parent;
        parent.AddChild(child);

        return parent.Children.Count;
    }
}