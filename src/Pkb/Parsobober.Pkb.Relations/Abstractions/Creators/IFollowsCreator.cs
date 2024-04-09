using Parsobober.Pkb.Ast;

namespace Parsobober.Pkb.Relations.Abstractions.Creators;

/// <summary>
/// Interface for creating a follows relationship between two nodes in a tree.
/// </summary>
public interface IFollowsCreator
{
    /// <summary>
    /// Sets the follows relationship between the specified node and its parent.
    /// </summary>
    /// <param name="node">The node to establish the follows relationship with.</param>
    /// <param name="parent">The parent node of the specified node.</param>
    void SetFollows(TreeNode node, TreeNode parent);
}