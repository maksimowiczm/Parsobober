namespace Parsobober.Pkb.Ast.Abstractions;

/// <summary>
/// Represents an interface for a variable table.
/// </summary>
public interface IVariableTable
{
    /// <summary>
    /// Gets the size of the variable table.
    /// </summary>
    int Size { get; }

    /// <summary>
    /// Inserts a variable with the specified name into the variable table.
    /// </summary>
    /// <param name="variableName">The name of the variable to insert.</param>
    /// <returns>The index of the inserted variable even if variable exists.</returns>
    int InsertVariable(string variableName);

    /// <summary>
    /// Gets the index of the variable with the specified name from the variable table.
    /// </summary>
    /// <param name="variableName">The name of the variable to get the index for.</param>
    /// <returns>The index of the variable.</returns>
    int GetVariableIndex(string variableName);

    /// <summary>
    /// Gets the name of the variable at the specified index from the variable table.
    /// </summary>
    /// <param name="index">The index of the variable to get the name for.</param>
    /// <returns>
    /// The name of the variable at the specified index,
    /// otherwise an empty string if the index is out of bounds.
    /// </returns>
    string GetVariableName(int index);
}