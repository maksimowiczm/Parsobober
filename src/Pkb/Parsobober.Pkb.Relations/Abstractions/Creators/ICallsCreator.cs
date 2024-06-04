using Parsobober.Pkb.Ast;

namespace Parsobober.Pkb.Relations.Abstractions.Creators;

public interface ICallsCreator
{
    /// <summary>
    /// Sets the calls relationship between the two procedures: caller and called, provided by call statement argument.
    /// </summary>
    /// <param name="callerProcedure">Calling procedure.</param>
    /// <param name="callStatement">Call statement.</param>
    void SetCalls(TreeNode callerProcedure, TreeNode callStatement);

    /// <summary>
    /// Sets the relationship between the container statement and the call statement.
    /// </summary>
    /// <param name="containerStatement">Container statement that contains call statement.</param>
    /// <param name="callStatement">Call statement.</param>
    void SetContainerCalls(TreeNode containerStatement, TreeNode callStatement);
}