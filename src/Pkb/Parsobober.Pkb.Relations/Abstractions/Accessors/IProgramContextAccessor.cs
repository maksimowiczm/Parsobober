using Parsobober.Pkb.Ast;

namespace Parsobober.Pkb.Relations.Abstractions.Accessors;

public interface IProgramContextAccessor
{
    /// <summary>
    /// Returns dictionary that uses variable name as key and its first TreeNode occurence as value
    /// </summary>
    public IReadOnlyDictionary<string, TreeNode> VariablesDictionary { get; }

    /// <summary>
    /// Returns dictionary that uses line number as key and statement TreeNode as value
    /// </summary>
    public IReadOnlyDictionary<int, TreeNode> StatementsDictionary { get; }

    /// <summary>
    /// Returns dictionary that uses procedure name as key and TreeNode as value
    /// </summary>
    public IReadOnlyDictionary<string, TreeNode> ProceduresDictionary { get; }
}