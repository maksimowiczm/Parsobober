namespace Parsobober.Pkb.Ast.Abstractions;

/// <summary>
/// Represents the abstract syntax tree (AST) for the parser.
/// </summary>
public interface IAst
{
    /// <summary>
    /// Gets the root node of the abstract syntax tree.
    /// </summary>
    TreeNode Root { get; }

    /// <summary>
    /// Creates a new tree node with the specified line number and entity type.
    /// </summary>
    /// <param name="lineNumber">The line number of the new tree node.</param>
    /// <param name="type">The entity type of the new tree node.</param>
    /// <returns>
    /// A new tree node with the specified line number and entity type.
    /// </returns>
    public static TreeNode CreateTreeNode(int lineNumber, EntityType type)
    {
        var newNode = new TreeNode(lineNumber, type);

        return newNode;
    }

    /// <summary>
    /// Sets an attribute on the specified tree node.
    /// </summary>
    /// <param name="node">The tree node to set the attribute on.</param>
    /// <param name="attribute">The attribute to set on the tree node.</param>
    public static void SetAttribute(TreeNode node, string attr)
    {
        node.Attribute = attr;
    }

    /// <summary>
    /// Sets the specified tree nodes as siblings.
    /// </summary>
    /// <param name="left">The left sibling tree node.</param>
    /// <param name="right">The right sibling tree node.</param>
    public static void SetSiblings(TreeNode left, TreeNode right)
    {
        left.RightSibling = right;
        right.LeftSibling = left;
    }

    /// <summary>
    /// Sets the parent-child relationship between the specified tree nodes.
    /// </summary>
    /// <param name="parent">The parent tree node.</param>
    /// <param name="child">The child tree node.</param>
    /// <returns>
    /// The number of children.
    /// </returns>
    public static int SetParenthood(TreeNode parent, TreeNode child)
    {
        child.Parent = parent;
        parent.AddChild(child);

        return parent.Children.Count;
    }
}