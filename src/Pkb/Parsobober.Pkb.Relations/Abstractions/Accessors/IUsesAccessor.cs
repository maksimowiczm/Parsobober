using Parsobober.Pkb.Relations.Dto;

namespace Parsobober.Pkb.Relations.Abstractions.Accessors;

public interface IUsesAccessor
{
    /// <summary>
    /// Returns variables that are used by <typeparamref name="T"/>
    /// => Uses(provided, returned)
    /// </summary>
    /// <typeparam name="T"><see cref="IRequest"/></typeparam>
    IEnumerable<Variable> GetVariables<T>() where T : IRequest;

    /// <summary>
    /// Returns variables that are used by statement with given <paramref name="lineNumber"/>
    /// => Uses(provided, returned)
    /// </summary>
    /// <param name="lineNumber">Line number</param>
    IEnumerable<Variable> GetVariables(int lineNumber);

    /// <summary>
    /// Returns statements that uses variable
    /// => Uses(returned, _)
    /// </summary>
    IEnumerable<Statement> GetStatements();

    /// <summary>
    /// Returns statements that uses variable with given variableName
    /// => Uses(returned, provided)
    /// </summary>
    /// <param name="variableName">Variable name</param>
    IEnumerable<Statement> GetStatements(string variableName);

    /// <summary>
    /// Returns procedures that use variable with given variableName
    /// => Uses(returned, provided)
    /// </summary>
    /// <param name="variableName">Variable name</param>
    IEnumerable<Procedure> GetProcedures(string variableName);

    /// <summary>
    /// Returns procedures that use variables
    /// => Uses(returned, _)
    /// </summary>
    /// <returns></returns>
    IEnumerable<Procedure> GetProcedures();

    /// <summary>
    /// Returns variables that are used by procedure with given procedureName
    /// </summary>
    /// <param name="procedureName">Procedure name</param>
    IEnumerable<Variable> GetVariables(string procedureName);

    /// <summary>
    /// Returns true if statement with given line number uses variable with given variableName
    /// </summary>
    bool IsUsed(int lineNumber, string variableName);

    /// <summary>
    /// Returns true if procedure with given name uses variable with given variableName
    /// </summary>
    bool IsUsed(string procedureName, string variableName);

    /// <summary>
    /// Boxed uses request.
    /// </summary>
    interface IRequest;
}