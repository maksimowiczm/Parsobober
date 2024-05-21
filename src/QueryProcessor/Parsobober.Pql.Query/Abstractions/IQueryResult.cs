namespace Parsobober.Pql.Query.Abstractions;

/// <summary>
/// The IQuery interface defines the contract for a query that can be executed.
/// </summary>
public interface IQueryResult
{
    /// <summary>
    /// Executes the query and returns the result as a string.
    /// </summary>
    /// <returns>The result of the query execution.</returns>
    string Execute();
}