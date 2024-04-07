namespace Parsobober.Pkb.Ast.AstNodes;

public class TreeNode(int lineNumber, EntityType type)
{
    private readonly int _lineNumber = lineNumber;
    private string? _attribute;
    private TreeNode? _leftSibling;
    private TreeNode? _rightSibling;
    private TreeNode? _parent;
    private readonly IList<TreeNode> _children = new List<TreeNode>();

    public int LineNumber => _lineNumber;

    public string? Attribute
    {
        get => _attribute;
        set => _attribute = value;
    }

    public TreeNode? LeftSibling
    {
        get => _leftSibling;
        set => _leftSibling = value ?? throw new ArgumentNullException(nameof(value));
    }

    public TreeNode? RightSibling
    {
        get => _rightSibling;
        set => _rightSibling = value ?? throw new ArgumentNullException(nameof(value));
    }

    public TreeNode? Parent
    {
        get => _parent;
        set => _parent = value ?? throw new ArgumentNullException(nameof(value));
    }

    public EntityType Type
    {
        get => type;
        set => type = value;
    }

    public IList<TreeNode> Children => _children;

    public TreeNode GetChild(int idx)
    {
        return _children[idx];
    }
}