namespace Parsobober.Pkb.Ast.AstNodes;

public class Ast : IAst
{
    private TreeNode? _root;

    private TreeNode? Root
    {
        get => _root;
        set => _root = value ?? throw new ArgumentNullException(nameof(value));
    }

    public TreeNode CreateTNode(int lineNumber, EntityType type)
    {
        TreeNode newNode = new TreeNode(lineNumber, type);

        if (_root == null)
            SetRoot(newNode);

        return newNode;
    }

    public void SetRoot(TreeNode node)
    {
        Root = node;
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

    public TreeNode? GetRoot()
    {
        return Root;
    }

    public TreeNode GetChildN(TreeNode node, int n)
    {
        return node.GetChild(n);
    }
}