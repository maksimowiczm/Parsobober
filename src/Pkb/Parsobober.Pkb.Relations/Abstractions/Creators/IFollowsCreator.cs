using Parsobober.Pkb.Ast;

namespace Parsobober.Pkb.Relations.Abstractions.Creators;

/// <summary>
/// Interface for creating a follows relationship between two nodes in a tree.
/// </summary>
public interface IFollowsCreator
{
    /// <summary>
    /// Sets the follows relationship between the given nodes.
    /// </summary>
    void SetFollows(TreeNode preceding, TreeNode following);
}