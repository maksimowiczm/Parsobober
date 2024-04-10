using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Dto;
using static Parsobober.Pkb.Relations.Dto.Refs;
using static Parsobober.Pkb.Relations.Dto.Statements;

namespace Parsobober.Pkb.Relations.Abstractions.Accessors;

public interface IUsesAccessor
{
    /// <summary>
    /// Returns variables that are used by the given statement => Uses(provided(stmt), returned)
    /// </summary>
    IEnumerable<TreeNode> GetVariables(Statement statement);

    /// <summary>
    /// Returns variables that are used by the given procedure => Uses(provided(proc), returned)
    /// </summary>
    IEnumerable<TreeNode> GetVariables(Procedure procedure);

    /// <summary>
    /// Returns statements that use given variable => Uses(returned, provided(stmt))
    /// </summary>
    IEnumerable<TreeNode> GetStatements(Variable variable);

    /// <summary>
    /// Returns procedures that use given variable => Uses(returned, provided(proc))
    /// </summary>
    IEnumerable<TreeNode> GetProcedures(Variable variable);

    bool IsModified(Variable variable, Statement statement);

    bool IsModified(Variable variable, Procedure procedure);
}