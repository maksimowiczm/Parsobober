namespace Parsobober.Pkb.Ast;

public class TreeNode(int lineNumber, EntityType type)
{
    public int LineNumber { get; set; } = lineNumber;
    public EntityType Type { get; } = type;
    public string? Attribute { get; internal set; }
    public TreeNode? LeftSibling { get; internal set; }
    public TreeNode? RightSibling { get; internal set; }
    public TreeNode? Parent { get; internal set; }
    private readonly List<TreeNode> _children = [];
    public IReadOnlyList<TreeNode> Children => _children.AsReadOnly();

    internal void AddChild(TreeNode child)
    {
        _children.Add(child);
    }

    public override string ToString()
    {
        return Type.ToString() + ':' + Attribute;
    }

    public bool _Equals_(object? obj)
    {

        if (Equals(obj))
        {
            return true;
        }

        if (Children.Count > 0 && Children[0]._Equals_(obj))
        {
            return true;
        }

        if (Children.Count > 1 && Children[1]._Equals_(obj))
        {
            return true;
        }
        return false;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not TreeNode node)
        {
            return false;
        }

        if (Children.Count != node.Children.Count || Type != node.Type || Attribute != node.Attribute)
        {
            return false;
        }

        for (int i = 0; i < Children.Count; i++)
        {
            if (!Children[i].Equals(node.Children[i]))
            {
                return false;
            }
        }

        return true;
    }
}