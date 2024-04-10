using Parsobober.Pkb.Ast;

namespace Parsobober.Pkb.Relations.Abstractions.Creators;

public interface IUsesCreator
{
    /// <summary>
    /// Sets the uses relationship between the user(stmt or proc) and variable.
    /// </summary>
    /// <param name="user">Statement or procedure that uses variable.</param>
    /// <param name="variable">Variable that is used.</param>
    void SetUses(TreeNode user, TreeNode variable);
}