using Parsobober.Pkb.Ast;

namespace Parsobober.Pkb.Relations.Abstractions.Creators;

public interface IModifiesCreator
{
    /// <summary>
    /// Sets the modifies relationship between the modifier(stmt or proc) and variable.
    /// </summary>
    /// <param name="modifier">Statement or procedure that modifies variable.</param>
    /// <param name="variable">Variable that is modified.</param>
    void SetModifies(TreeNode modifier, TreeNode variable);
}