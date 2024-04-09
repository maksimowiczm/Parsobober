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
    TreeNode CreateTreeNode(int lineNumber, EntityType type);

    /// <summary>
    /// Sets an attribute on the specified tree node.
    /// </summary>
    /// <param name="node">The tree node to set the attribute on.</param>
    /// <param name="attribute">The attribute to set on the tree node.</param>
    void SetAttribute(TreeNode node, string attribute);

    /// <summary>
    /// Sets the specified tree nodes as siblings.
    /// </summary>
    /// <param name="left">The left sibling tree node.</param>
    /// <param name="right">The right sibling tree node.</param>
    void SetSiblings(TreeNode left, TreeNode right);

    /// <summary>
    /// Sets the parent-child relationship between the specified tree nodes.
    /// </summary>
    /// <param name="parent">The parent tree node.</param>
    /// <param name="child">The child tree node.</param>
    /// <returns>
    /// The number of children.
    /// </returns>
    int SetParenthood(TreeNode parent, TreeNode child);
}