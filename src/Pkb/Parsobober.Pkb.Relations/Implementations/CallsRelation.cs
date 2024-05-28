using Microsoft.Extensions.Logging;
using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Abstractions.Creators;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pkb.Relations.Utilities;

namespace Parsobober.Pkb.Relations.Implementations;

public class CallsRelation(
    ILogger<CallsRelation> logger,
    IProgramContextAccessor programContext
) : ICallsCreator, ICallsAccessor
{
    /// <summary>
    /// (caller, list of called)
    /// </summary>
    private readonly Dictionary<string, List<string>> _callersDictionary = new();

    /// <summary>
    /// (called, list of callers)
    /// </summary>
    private readonly Dictionary<string, List<string>> _calledDictionary = new();

    public void SetCalls(TreeNode callerProcedure, TreeNode callStatement)
    {
        if (!callerProcedure.Type.IsProcedure() || callStatement.Type != EntityType.Call)
        {
            logger.LogError(
                "Calls relation can only be established between two procedure nodes. ({caller} => {called})",
                callerProcedure, callStatement);

            throw new ArgumentException(
                $"At least one of provided nodes type: {callStatement.Type}, {callerProcedure.Type} is different than required: {EntityType.Procedure} type.");
        }

        // Check required attributes
        if (callerProcedure.Attribute is null)
        {
            logger.LogError("Caller procedure must have an attribute. ({caller})", callerProcedure);

            throw new ArgumentNullException(callerProcedure.Attribute);
        }

        if (callStatement.Attribute is null)
        {
            logger.LogError("Called procedure must have an attribute. ({called})", callStatement);

            throw new ArgumentNullException(callStatement.Attribute);
        }

        AddCallsToDictionaries(callerProcedure.Attribute, callStatement.Attribute);
    }

    private void AddCallsToDictionaries(string caller, string called)
    {
        if (_callersDictionary.TryGetValue(caller, out var calledList))
        {
            if (!calledList.Contains(called))
            {
                calledList.Add(called);
            }
        }
        else
        {
            _callersDictionary.Add(caller, [called]);
        }

        if (_calledDictionary.TryGetValue(called, out var callerList))
        {
            if (!callerList.Contains(caller))
            {
                callerList.Add(caller);
            }
        }
        else
        {
            _calledDictionary.Add(called, [caller]);
        }
    }

    public IEnumerable<Procedure> GetCalled(string callerName)
    {
        return _callersDictionary.TryGetValue(callerName, out var calledList)
            ? calledList.Select(calledName => programContext.ProceduresDictionary[calledName].ToProcedure())
            : Enumerable.Empty<Procedure>();
    }

    public IEnumerable<Procedure> GetCallers(string calledName)
    {
        return _calledDictionary.TryGetValue(calledName, out var callerList)
            ? callerList.Select(callerName => programContext.ProceduresDictionary[callerName].ToProcedure())
            : Enumerable.Empty<Procedure>();
    }

    public IEnumerable<Procedure> GetCalledTransitive(string callerName)
    {
        var visited = new HashSet<string>();
        var stack = new Stack<string>();
        stack.Push(callerName);

        while (stack.Count > 0)
        {
            var currentProcedure = stack.Pop();

            // Skip if already visited
            if (!visited.Add(currentProcedure))
            {
                continue;
            }

            if (_callersDictionary.TryGetValue(currentProcedure, out var calledList))
            {
                // Add all called procedures to the stack
                foreach (var calledProcedure in calledList)
                {
                    stack.Push(calledProcedure);
                }
            }
        }

        visited.Remove(callerName);
        return visited.Select(calledName => programContext.ProceduresDictionary[calledName].ToProcedure());
    }

    public IEnumerable<Procedure> GetCallersTransitive(string calledName)
    {
        var visited = new HashSet<string>();
        var stack = new Stack<string>();
        stack.Push(calledName);

        while (stack.Count > 0)
        {
            var currentProcedure = stack.Pop();

            // Skip if already visited
            if (!visited.Add(currentProcedure))
            {
                continue;
            }

            if (_calledDictionary.TryGetValue(currentProcedure, out var callerList))
            {
                // Add all caller procedures to the stack
                foreach (var callerProcedure in callerList)
                {
                    stack.Push(callerProcedure);
                }
            }
        }

        visited.Remove(calledName);
        return visited.Select(callerName => programContext.ProceduresDictionary[callerName].ToProcedure());
    }
}