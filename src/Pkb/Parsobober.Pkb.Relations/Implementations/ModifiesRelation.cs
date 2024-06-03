using Microsoft.Extensions.Logging;
using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Abstractions.Creators;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pkb.Relations.Utilities;
using static Parsobober.Pkb.Relations.Abstractions.Accessors.IModifiesAccessor;

namespace Parsobober.Pkb.Relations.Implementations;

public class ModifiesRelation(
    ILogger<ParentRelation> logger,
    IProgramContextAccessor programContext,
    CallsRelation callsRelation
) : IModifiesCreator, IModifiesAccessor

{
    /// <summary>
    /// Stores modifies relation between statement and variable list
    /// </summary>
    /// <remarks>[statement line number,  list of variable names].</remarks>
    private readonly Dictionary<int, List<string>> _modifiesStatementDictionary = new();

    /// <summary>
    /// Stores modifies relation between procedure and variable list
    /// </summary>
    /// <remarks>[procedure name, list of variable names].</remarks>
    private readonly Dictionary<string, List<string>> _modifiesProcedureDictionary = new();

    public void SetModifies(TreeNode modifier, TreeNode variable)
    {
        if (!variable.Type.IsVariable())
        {
            logger.LogError(
                "Statement modifies relation can only be established between statement and variable node. ({variable} must be variable)",
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

        if (modifier.Type.IsStatement())
        {
            AddToStatementDictionary(modifier.LineNumber, variable.Attribute);
        }
        else if (modifier.Type.IsProcedure())
        {
            // Check required procedure attribute
            if (modifier.Attribute is null)
            {
                logger.LogError("Procedure must have an attribute. ({node})", modifier);

                throw new ArgumentNullException(modifier.Attribute);
            }

            AddToProcedureDictionary(modifier.Attribute, variable.Attribute);
        }
        else
        {
            logger.LogError(
                "Modifies relation can only be established between statement/procedure and variable node. ({user} must be statement/procedure)",
                modifier);

            throw new ArgumentException(
                $"User node type {modifier.Type} is different than any of required: {EntityType.Statement} or {EntityType.Procedure} types.");
        }
    }

    private void AddToStatementDictionary(int lineNumber, string variableName)
    {
        if (_modifiesStatementDictionary.TryGetValue(lineNumber, out var variableList))
        {
            if (variableList.Contains(variableName))
            {
                return;
            }

            variableList.Add(variableName);
            return;
        }

        _modifiesStatementDictionary.Add(lineNumber, [variableName]);
    }

    private void AddToProcedureDictionary(string procedureName, string variableName)
    {
        if (_modifiesProcedureDictionary.TryGetValue(procedureName, out var variableList))
        {
            if (variableList.Contains(variableName))
            {
                return;
            }

            variableList.Add(variableName);
            return;
        }

        _modifiesProcedureDictionary.Add(procedureName, [variableName]);
    }

    public IEnumerable<Variable> GetVariables<T>() where T : IRequest
    {
        return _modifiesStatementDictionary
            .Where(statement => programContext.StatementsDictionary[statement.Key].IsType<T>())
            .SelectMany(statement => statement.Value)
            .Distinct()
            .Select(variable => programContext.VariablesDictionary[variable].ToVariable());
    }

    public IEnumerable<Statement> GetStatements()
    {
        return _modifiesStatementDictionary.Select(entry =>
            programContext.StatementsDictionary[entry.Key].ToStatement()
        );
    }

    public IEnumerable<Variable> GetVariables(int lineNumber)
    {
        return _modifiesStatementDictionary.TryGetValue(lineNumber, out var variableList)
            ? variableList.Select(variableName => programContext.VariablesDictionary[variableName].ToVariable())
            : Enumerable.Empty<Variable>();
    }

    public IEnumerable<Statement> GetStatements(string variableName)
    {
        return _modifiesStatementDictionary
            .Where(stmt => stmt.Value.Contains(variableName))
            .Select(stmt => programContext.StatementsDictionary[stmt.Key].ToStatement());
    }

    public IEnumerable<Procedure> GetProcedures(string variableName)
    {
        var visited = new HashSet<string>();
        var proceduresToVisit = new Stack<string>();

        foreach (var procedure in _modifiesProcedureDictionary.Where(proc => proc.Value.Contains(variableName)))
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

            if (_modifiesProcedureDictionary.TryGetValue(currentProcedure, out var variableList))
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

    public bool IsModified(int lineNumber, string variableName) =>
        GetVariables(lineNumber).Any(variable => variable.Name == variableName);

    public bool IsModified(string procedureName, string variableName) =>
        GetVariables(procedureName).Any();
}