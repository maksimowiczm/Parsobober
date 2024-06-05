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
    private readonly Dictionary<string, HashSet<string>> _callersDictionary = new();

    /// <summary>
    /// (called, list of callers)
    /// </summary>
    private readonly Dictionary<string, HashSet<string>> _calledDictionary = new();

    /// <summary>
    /// (container, list of called procedures)
    /// </summary>
    private readonly Dictionary<int, HashSet<string>> _containerCallsDictionary = new();

    #region ICallsCreator

    public void SetCalls(TreeNode callerProcedure, TreeNode callStatement)
    {
        if (!callerProcedure.Type.IsProcedure())
        {
            logger.LogError(
                "Calls relation can only be established between procedure and call statement. ({caller} should be procedure)",
                callerProcedure);

            throw new ArgumentException(
                $"Type {callerProcedure.Type} is different than required: {EntityType.Procedure} type.");
        }

        if (!callStatement.Type.IsCallStatement())
        {
            logger.LogError(
                "Calls relation can only be established between procedure and call statement. ({called} should be call statement)",
                callStatement);

            throw new ArgumentException(
                $"Type {callStatement.Type} is different than required: {EntityType.Call} type.");
        }

        // Check required attributes
        if (callerProcedure.Attribute is null)
        {
            logger.LogError("Caller procedure must have an attribute. ({caller})", callerProcedure);

            throw new ArgumentNullException(callerProcedure.Attribute);
        }

        if (callStatement.Attribute is null)
        {
            logger.LogError("Call statement must have an attribute. ({called})", callStatement);

            throw new ArgumentNullException(callStatement.Attribute);
        }

        AddCallsToDictionaries(callerProcedure.Attribute, callStatement.Attribute);
    }

    public void SetContainerCalls(TreeNode containerStatement, TreeNode callStatement)
    {
        if (!containerStatement.Type.IsContainerStatement())
        {
            logger.LogError(
                "Container calls relation can only be established between container statement and call statement. ({container} should be container statement)",
                containerStatement);

            throw new ArgumentException(
                $"Type {containerStatement.Type} is different than any of required: container {EntityType.Statement}.");
        }

        if (!callStatement.Type.IsCallStatement())
        {
            logger.LogError(
                "Container calls relation can only be established between container statement  and call statement. ({called} should be call statement)",
                callStatement);

            throw new ArgumentException(
                $"Type {callStatement.Type} is different than required: {EntityType.Call} type.");
        }


        if (callStatement.Attribute is null)
        {
            logger.LogError("Called procedure must have an attribute. ({called})", callStatement);

            throw new ArgumentNullException(callStatement.Attribute);
        }

        if (_containerCallsDictionary.TryGetValue(containerStatement.LineNumber, out var calledList))
        {
            calledList.Add(callStatement.Attribute);
        }
        else
        {
            _containerCallsDictionary.Add(containerStatement.LineNumber, [callStatement.Attribute]);
        }
    }

    private void AddCallsToDictionaries(string caller, string called)
    {
        if (_callersDictionary.TryGetValue(caller, out var calledList))
        {
            calledList.Add(called);
        }
        else
        {
            _callersDictionary.Add(caller, [called]);
        }

        if (_calledDictionary.TryGetValue(called, out var callerList))
        {
            callerList.Add(caller);
        }
        else
        {
            _calledDictionary.Add(called, [caller]);
        }
    }

    #endregion

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

    public IEnumerable<Procedure> GetCalled()
    {
        return _calledDictionary.Keys.Select(
            calledName => programContext.ProceduresDictionary[calledName].ToProcedure());
    }

    public IEnumerable<Procedure> GetCallers()
    {
        return _callersDictionary.Keys.Select(
            callerName => programContext.ProceduresDictionary[callerName].ToProcedure());
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

    public bool IsCalled(string callerName, string calledName)
    {
        return GetCalled(callerName).Any(called => called.ProcName == calledName);
    }

    public bool IsCalledTransitive(string callerName, string calledName)
    {
        return GetCalledTransitive(callerName).Any(called => called.ProcName == calledName);
    }

    internal List<string> GetAllCalledProcedures() => _calledDictionary.Keys.ToList();

    internal IReadOnlyDictionary<int, HashSet<string>> GetAllContainerCalls() => _containerCallsDictionary.AsReadOnly();
}