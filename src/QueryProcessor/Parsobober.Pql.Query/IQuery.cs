namespace Parsobober.Pql.Query;

/// <summary>
/// The IQuery interface defines the contract for a query that can be executed.
/// </summary>
public interface IQuery
{
    /// <summary>
    /// Executes the query and returns the result as a string.
    /// </summary>
    /// <returns>The result of the query execution.</returns>
    string Execute();
}