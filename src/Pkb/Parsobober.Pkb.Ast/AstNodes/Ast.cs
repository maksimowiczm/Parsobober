namespace Parsobober.Pkb.Ast.AstNodes;

public class Ast : IAst
{
    public Ast()
    {
        Root = new TreeNode(0, EntityType.Program);
    }

    public TreeNode Root { get; }

    public TreeNode CreateTNode(int lineNumber, EntityType type)
    {
        TreeNode newNode = new TreeNode(lineNumber, type);
        return newNode;
    }

    public void SetAttr(TreeNode node, string attr)
    {
        node.Attribute = attr;
    }

    public void SetSibling(TreeNode left, TreeNode right)
    {
        left.RightSibling = right;
        right.LeftSibling = left;
    }

    public int SetParenthood(TreeNode parent, TreeNode child)
    {
        child.Parent = parent;
        parent.Children.Add(child);
        return parent.Children.Count;
    }

    public TreeNode GetChildN(TreeNode node, int n)
    {
        return node.Children[n];
    }
}