using Parsobober.Pkb.Ast;
using static Parsobober.Pkb.Relations.Dto.Statements;

namespace Parsobober.Pkb.Relations.Abstractions.Accessors;

public interface IParentAccessor
{
    /// <summary>
    /// Returns parent of the given statement => Parent(returned, provided)
    /// </summary>
    TreeNode GetParent(Statement statement);

    /// <summary>
    /// Returns transitive parents of the given statement => Parent*(returned, provided)
    /// </summary>
    IEnumerable<TreeNode> GetParentTransitive(Statement statement);

    /// <summary>
    /// Returns children of the given statement => Parent(provided, returned)
    /// </summary>
    IEnumerable<TreeNode> GetParentedBy(Statement statement);

    /// <summary>
    /// Returns transitive children of the given statement => Parent*(provided, returned)
    /// </summary>
    IEnumerable<TreeNode> GetParentedByTransitive(Statement statement);

    bool IsParent(Statement parent, Statement child);

    bool IsParentTransitive(Statement parent, Statement child);
}