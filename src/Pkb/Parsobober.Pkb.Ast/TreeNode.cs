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
}