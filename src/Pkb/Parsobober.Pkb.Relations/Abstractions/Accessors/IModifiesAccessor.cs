using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Dto;
using static Parsobober.Pkb.Relations.Dto.Refs;
using static Parsobober.Pkb.Relations.Dto.Statements;

namespace Parsobober.Pkb.Relations.Abstractions.Accessors;

public interface IModifiesAccessor
{
    /// <summary>
    /// Returns variables that are modified by the given statement => Modifies(provided(stmt), returned)
    /// </summary>
    IEnumerable<TreeNode> GetVariables(Statement statement);

    /// <summary>
    /// Returns variables that are modified by the given procedure => Modifies(provided(proc), returned)
    /// </summary>
    IEnumerable<TreeNode> GetVariables(Procedure procedure);

    /// <summary>
    /// Returns statements that modify given variable => Modifies(returned, provided(stmt))
    /// </summary>
    IEnumerable<TreeNode> GetStatements(Variable variable);

    /// <summary>
    /// Returns procedures that modify given variable => Modifies(returned, provided(proc))
    /// </summary>
    IEnumerable<TreeNode> GetProcedures(Variable variable);

    bool IsModified(Statement statement, Variable variable);

    bool IsModified(Procedure procedure, Variable variable);
}