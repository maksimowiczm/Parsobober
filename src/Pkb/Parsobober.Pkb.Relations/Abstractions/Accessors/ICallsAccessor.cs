using Parsobober.Pkb.Relations.Dto;

namespace Parsobober.Pkb.Relations.Abstractions.Accessors;

public interface ICallsAccessor
{
    /// <summary>
    /// Returns procedures that are called by given procedure
    /// => Calls(provided, returned)
    /// </summary>
    /// <param name="callerName">Caller procedure name</param>
    IEnumerable<Procedure> GetCalled(string callerName);

    /// <summary>
    /// Returns procedures that are calling given procedure
    /// => Calls(returned, Provided)
    /// </summary>
    /// <param name="calledName">Called procedure name</param>
    IEnumerable<Procedure> GetCallers(string calledName);

    /// <summary>
    /// Returns procedures that are called by any procedure (can be used as transitive)
    /// </summary>
    IEnumerable<Procedure> GetCalled();

    /// <summary>
    /// Returns procedures that are calling anu procedure (can be used as transitive)
    /// </summary>
    IEnumerable<Procedure> GetCallers();

    /// <summary>
    /// Returns procedures that are transitive called by given procedure
    /// => Calls*(provided, returned)
    /// </summary>
    /// /// <param name="callerName">Caller procedure name</param>
    IEnumerable<Procedure> GetCalledTransitive(string callerName);

    /// <summary>
    /// Returns procedures that are transitive calling given procedure
    /// => Calls*(returned, provided)
    /// </summary>
    /// <param name="calledName">Called procedure name</param>
    IEnumerable<Procedure> GetCallersTransitive(string calledName);

    /// <summary>
    /// Returns true if caller calls called, false otherwise.
    /// </summary>
    /// <param name="callerName">Caller procedure name</param>
    /// <param name="calledName">Called procedure name</param>
    bool IsCalled(string callerName, string calledName);

    /// <summary>
    /// Returns true if caller calls transitively called, false otherwise.
    /// </summary>
    /// <param name="callerName">Caller procedure name</param>
    /// <param name="calledName">Called procedure name</param>
    bool IsCalledTransitive(string callerName, string calledName);
}