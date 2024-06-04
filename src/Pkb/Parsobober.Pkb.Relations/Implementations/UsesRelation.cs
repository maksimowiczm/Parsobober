using Microsoft.Extensions.Logging;
using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Abstractions.Creators;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pkb.Relations.Utilities;
using static Parsobober.Pkb.Relations.Abstractions.Accessors.IUsesAccessor;

namespace Parsobober.Pkb.Relations.Implementations;

public class UsesRelation(
    ILogger<ParentRelation> logger,
    IProgramContextAccessor programContext,
    CallsRelation callsRelation
) : IUsesCreator, IUsesAccessor
{
    /// <summary>
    /// Stores uses relation between statement and variables it uses.
    /// </summary>
    /// <remarks>[statement line number,  list of variable names].</remarks>
    private readonly Dictionary<int, List<string>> _usesStatementDictionary = new();

    /// <summary>
    /// Stores usues relation between procedure and variable it uses.
    /// </summary>
    /// <remarks>[procedure name, list of variable names].</remarks>
    private readonly Dictionary<string, List<string>> _usesProcedureDictionary = new();

    public void SetUses(TreeNode user, TreeNode variable)
    {
        if (!variable.Type.IsVariable())
        {
            logger.LogError(
                "Uses relation can only be established between statement and variable node. ({variable} must be variable)",
                variable);

            throw new ArgumentException(
                $"Variable node type {variable.Type} is different than required: {EntityType.Variable}.");
        }

        // Check required attribute
        if (variable.Attribute is null)
        {
            logger.LogError("Variable must have an attribute. ({node})", variable);

            throw new ArgumentNullException(variable.Attribute);
        }

        if (user.Type.IsStatement())
        {
            AddToStatementDictionary(user.LineNumber, variable.Attribute);
        }
        else if (user.Type.IsProcedure())
        {
            // Check required procedure attribute
            if (user.Attribute is null)
            {
                logger.LogError("Procedure must have an attribute. ({node})", user);

                throw new ArgumentNullException(user.Attribute);
            }

            AddToProcedureDictionary(user.Attribute, variable.Attribute);
        }
        else
        {
            logger.LogError(
                "Uses relation can only be established between statement/procedure and variable node. ({user} must be statement/procedure)",
                user);

            throw new ArgumentException(
                $"User node type {user.Type} is different than any of required: {EntityType.Statement} or {EntityType.Procedure} types.");
        }
    }

    private void AddToStatementDictionary(int lineNumber, string variableName)
    {
        if (_usesStatementDictionary.TryGetValue(lineNumber, out var variableList))
        {
            if (variableList.Contains(variableName))
            {
                return;
            }

            variableList.Add(variableName);
            return;
        }

        _usesStatementDictionary.Add(lineNumber, [variableName]);
    }

    private void AddToProcedureDictionary(string procedureName, string variableName)
    {
        if (_usesProcedureDictionary.TryGetValue(procedureName, out var variableList))
        {
            if (variableList.Contains(variableName))
            {
                return;
            }

            variableList.Add(variableName);
            return;
        }

        _usesProcedureDictionary.Add(procedureName, [variableName]);
    }

    public IEnumerable<Variable> GetVariables<T>() where T : IRequest
    {
        return _usesStatementDictionary
            .Where(statement => programContext.StatementsDictionary[statement.Key].IsType<T>())
            .SelectMany(statement => statement.Value)
            .Distinct()
            .Select(variable => programContext.VariablesDictionary[variable].ToVariable());
    }

    public IEnumerable<Variable> GetVariables(int lineNumber)
    {
        return _usesStatementDictionary.TryGetValue(lineNumber, out var variableList)
            ? variableList.Select(variableName => programContext.VariablesDictionary[variableName].ToVariable())
            : Enumerable.Empty<Variable>();
    }

    public IEnumerable<Statement> GetStatements()
    {
        return _usesStatementDictionary.Select(entry =>
            programContext.StatementsDictionary[entry.Key].ToStatement()
        );
    }

    public IEnumerable<Statement> GetStatements(string variableName)
    {
        return _usesStatementDictionary
            .Where(stmt => stmt.Value.Contains(variableName))
            .Select(stmt => programContext.StatementsDictionary[stmt.Key].ToStatement());
    }

    public IEnumerable<Procedure> GetProcedures(string variableName)
    {
        var visited = new HashSet<string>();
        var proceduresToVisit = new Stack<string>();

        foreach (var procedure in _usesProcedureDictionary.Where(proc => proc.Value.Contains(variableName)))
        {
            proceduresToVisit.Push(procedure.Key);
        }

        while (proceduresToVisit.Count > 0)
        {
            var currentProcedure = proceduresToVisit.Pop();

            if (!visited.Add(currentProcedure))
            {
                continue;
            }

            foreach (var calledProcedure in callsRelation.GetCallers(currentProcedure))
            {
                proceduresToVisit.Push(calledProcedure.ProcName);
            }
        }

        return visited.Select(procName => programContext.ProceduresDictionary[procName].ToProcedure());
    }

    public IEnumerable<Variable> GetVariables(string procedureName)
    {
        var visitedProcedures = new HashSet<string>();
        var variables = new HashSet<Variable>();

        var proceduresToVisit = new Stack<string>(new[] { procedureName });

        while (proceduresToVisit.TryPop(out var currentProcedure))
        {
            if (!visitedProcedures.Add(currentProcedure))
            {
                continue;
            }

            if (_usesProcedureDictionary.TryGetValue(currentProcedure, out var variableList))
            {
                variables.UnionWith(variableList.Select(variable =>
                    programContext.VariablesDictionary[variable].ToVariable()));
            }

            foreach (var procedure in callsRelation.GetCalled(currentProcedure))
            {
                proceduresToVisit.Push(procedure.ProcName);
            }
        }

        return variables;
    }

    public bool IsUsed(int lineNumber, string variableName) =>
        GetVariables(lineNumber).Any(variable => variable.Name == variableName);

    public bool IsUsed(string procedureName, string variableName) =>
        GetVariables(procedureName).Any(variable => variable.Name == variableName);
}