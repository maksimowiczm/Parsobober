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
    /// => Uses(returned, provided)
    /// </summary>
    IEnumerable<Statement> GetStatements();

    /// <summary>
    /// Returns statements that uses variable with given variableName
    /// => Uses(returned, provided)
    /// </summary>
    /// <param name="variableName">Variable name</param>
    IEnumerable<Statement> GetStatements(string variableName);

    /// <summary>
    /// Boxed uses request.
    /// </summary>
    interface IRequest
    {
    }
}