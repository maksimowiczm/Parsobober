namespace Parsobober.Pkb.Ast.AstNodes;
using System.Collections.Generic;

public class TreeNode
{
    public int LineNumber { get; }
    public EntityType Type { get;}
    public string? Attribute { get; set; }
    public TreeNode? LeftSibling { get; set; }
    public TreeNode? RightSibling { get; set; }
    public TreeNode? Parent { get; set; }
    public IList<TreeNode> Children { get; } = new List<TreeNode>();

    public TreeNode(int lineNumber, EntityType type)
    {
        LineNumber = lineNumber;
        Type = type;
    }

    public override string ToString()
    {
        return Type.ToString() + ':' + Attribute;
    }
}