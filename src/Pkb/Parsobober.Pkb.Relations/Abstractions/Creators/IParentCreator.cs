using Parsobober.Pkb.Ast;

namespace Parsobober.Pkb.Relations.Abstractions.Creators;

/// <summary>
/// Interface for creating parent relation pairs in pkb.
/// </summary>
public interface IParentCreator
{
    /// <summary>
    /// Sets the parent of a child node.
    /// </summary>
    /// <param name="parentNode">The parent node to which the child node will be attached.</param>
    /// <param name="childNode">The child node that will be attached to the parent node.</param>
    void SetParent(TreeNode parentNode, TreeNode childNode);
}