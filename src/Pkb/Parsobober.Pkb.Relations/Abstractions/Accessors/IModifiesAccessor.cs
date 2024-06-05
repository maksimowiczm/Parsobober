using Parsobober.Pkb.Relations.Dto;

namespace Parsobober.Pkb.Relations.Abstractions.Accessors;

public interface IModifiesAccessor
{
    /// <summary>
    /// Returns variables that are modified by <typeparamref name="T"/>
    /// => Modifies(provided, returned)
    /// </summary>
    /// <typeparam name="T"><see cref="IRequest"/></typeparam>
    IEnumerable<Variable> GetVariables<T>() where T : IRequest;

    /// <summary>
    /// Returns variables that are modified by statement with given <paramref name="lineNumber"/>
    /// => Modifies(provided, returned)
    /// </summary>
    /// <param name="lineNumber">Line number</param>
    IEnumerable<Variable> GetVariables(int lineNumber);

    /// <summary>
    /// Returns statements that modify variable
    /// => Modifies(returned, provided)
    /// </summary>
    IEnumerable<Statement> GetStatements();

    /// <summary>
    /// Returns statements that modify variable with given variableName
    /// => Modifies(returned, provided)
    /// </summary>
    /// <param name="variableName">Variable name</param>
    IEnumerable<Statement> GetStatements(string variableName);

    /// <summary>
    /// Returns procedures that modify variable with given variableName
    /// => Modifies(returned, provided)
    /// </summary>
    /// <param name="variableName">Variable name</param>
    IEnumerable<Procedure> GetProcedures(string variableName);

    /// <summary>
    /// Returns variables that are modified by procedure with given procedureName
    /// </summary>
    /// <param name="procedureName">Procedure name</param>
    IEnumerable<Variable> GetVariables(string procedureName);

    /// <summary>
    /// Returns true if statement with given line number modifies variable with given variableName
    /// </summary>
    bool IsModified(int lineNumber, string variableName);

    /// <summary>
    /// Returns true if procedure with given name modifies variable with given variableName
    /// </summary>
    bool IsModified(string procedureName, string variableName);

    /// <summary>
    /// Boxed modifies request.
    /// </summary>
    interface IRequest;
}