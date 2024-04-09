namespace Parsobober.Pkb.Ast.Abstractions;

/// <summary>
/// Represents an interface for a procedure table.
/// </summary>
public interface IProcedureTable
{
    /// <summary>
    /// Gets the size of the procedure table.
    /// </summary>
    int Size { get; }

    /// <summary>
    /// Inserts a new procedure into the procedure table.
    /// </summary>
    /// <param name="procedureName">The name of the procedure to be inserted.</param>
    /// <returns>The index of the newly inserted procedure or -1 if procedure with given name exists.</returns>
    int InsertProcedure(string procedureName);

    /// <summary>
    /// Gets the index of a procedure in the procedure table.
    /// </summary>
    /// <param name="procedureName">The name of the procedure to find.</param>
    /// <returns>The index of the procedure if found, otherwise -1.</returns>
    int GetProcedureIndex(string procedureName);

    /// <summary>
    /// Gets the name of a procedure at a given index in the procedure table.
    /// </summary>
    /// <param name="index">The index of the procedure to retrieve name for.</param>
    /// <returns>
    /// The name of the procedure at the specified index,
    /// otherwise an empty string if the index is out of bounds.
    /// </returns>
    string GetProcedureName(int index);
}