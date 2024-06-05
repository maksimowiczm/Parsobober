using Microsoft.Extensions.Logging;
using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions;
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
) : IUsesCreator, IPostParseComputable, IUsesAccessor
{
    /// <summary>
    /// Stores uses relation between statement and variables it uses.
    /// </summary>
    /// <remarks>[statement line number,  list of variable names].</remarks>
    private readonly Dictionary<int, HashSet<string>> _usesStatementDictionary = new();

    /// <summary>
    /// Stores uses relation between procedure and variable it uses.
    /// </summary>
    /// <remarks>[procedure name, list of variable names].</remarks>
    private readonly Dictionary<string, HashSet<string>> _usesProcedureDictionary = new();

    /// <summary>
    /// Stores uses relation between procedure and variable set including calls connections
    /// </summary>
    /// <remarks>[procedure name, list of variable names].</remarks>
    private readonly Dictionary<string, HashSet<string>> _usesProcedureDictionaryFull = new();

    #region IUsesCreator

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
            variableList.Add(variableName);
            return;
        }

        _usesStatementDictionary.Add(lineNumber, [variableName]);
    }

    private void AddToProcedureDictionary(string procedureName, string variableName)
    {
        if (_usesProcedureDictionary.TryGetValue(procedureName, out var variableList))
        {
            variableList.Add(variableName);
            return;
        }

        _usesProcedureDictionary.Add(procedureName, [variableName]);
    }

    #endregion

    #region PostParseComputable

    public void Compute()
    {
        foreach (var procedure in programContext.ProceduresDictionary.Keys.Reverse())
        {
            var modifiedVariables = new HashSet<string>();
            ComputeForProcedure(procedure, modifiedVariables);
            _usesProcedureDictionaryFull.Add(procedure, modifiedVariables);
        }
    }

    private void ComputeForProcedure(string procedureName, HashSet<string> usedVariables)
    {
        var visitedProcedures = new HashSet<string>();
        var proceduresToVisit = new Stack<string>(new[] { procedureName });

        while (proceduresToVisit.TryPop(out var currentProcedure))
        {
            if (!visitedProcedures.Add(currentProcedure))
            {
                if (_usesProcedureDictionaryFull.TryGetValue(currentProcedure, out var computedVariableList))
                {
                    usedVariables.UnionWith(computedVariableList);
                }

                continue;
            }

            if (_usesProcedureDictionary.TryGetValue(currentProcedure, out var variableList))
            {
                usedVariables.UnionWith(variableList);
            }

            foreach (var procedure in callsRelation.GetCalled(currentProcedure))
            {
                proceduresToVisit.Push(procedure.ProcName);
            }
        }
    }

    #endregion

    #region GetVariables<T>

    public IEnumerable<Variable> GetVariables<T>() where T : IRequest
    {
        return true switch
        {
            true when typeof(T) == typeof(Call) => // Call
                GetVariablesForCalls(),
            true when typeof(T).IsSubclassOf(typeof(ContainerStatement)) => // If, While
                GetVariablesForContainerStatements<T>(),
            true when typeof(T).IsSubclassOf(typeof(Statement)) => // Assign
                GetVariablesForDefaultStatement<T>(),
            true when typeof(T) == typeof(Statement) => // Statement
                GetVariableForGenericStatement(),
            _ => throw new NotSupportedException("How you passed type that does not implement IRequest?")
        };
    }

    // Return all variables used by called procedures, might be empty
    private IEnumerable<Variable> GetVariablesForCalls()
    {
        return callsRelation
            .GetAllCalledProcedures()
            .Select(procedure => _usesProcedureDictionaryFull[procedure])
            .SelectMany(variableList => variableList)
            .Distinct()
            .Select(variable => programContext.VariablesDictionary[variable].ToVariable());
    }

    private IEnumerable<Variable> GetVariablesForContainerStatements<T>() where T : IRequest
    {
        var flatUsedVariables = _usesStatementDictionary
            .Where(statement => programContext.StatementsDictionary[statement.Key].IsType<T>())
            .SelectMany(entry => entry.Value)
            .Distinct();

        var transientUsedVariables = callsRelation
            .GetAllContainerCalls()
            .Where(statement => programContext.StatementsDictionary[statement.Key].IsType<T>())
            .SelectMany(statement => statement.Value)
            .Distinct()
            .SelectMany(procedure => _usesProcedureDictionaryFull[procedure])
            .Distinct();

        return flatUsedVariables
            .Union(transientUsedVariables)
            .Select(variable => programContext.VariablesDictionary[variable].ToVariable());
    }

    private IEnumerable<Variable> GetVariablesForDefaultStatement<T>() where T : IRequest
    {
        return _usesStatementDictionary
            .Where(statement => programContext.StatementsDictionary[statement.Key].IsType<T>())
            .SelectMany(statement => statement.Value)
            .Distinct()
            .Select(variable => programContext.VariablesDictionary[variable].ToVariable());
    }

    // Todo: check in tests carefully
    private IEnumerable<Variable> GetVariableForGenericStatement()
    {
        return _usesProcedureDictionary.Values
            .SelectMany(variableList => variableList)
            .Distinct()
            .Select(variable => programContext.VariablesDictionary[variable].ToVariable());
    }

    #endregion

    #region GetVariables(int)

    public IEnumerable<Variable> GetVariables(int lineNumber)
    {
        if (!programContext.StatementsDictionary.TryGetValue(lineNumber, out var statement))
        {
            return Enumerable.Empty<Variable>();
        }

        if (statement.Type.IsCallStatement())
        {
            return GetVariables(statement.Attribute!);
        }

        if (statement.Type.IsContainerStatement())
        {
            return GetVariablesForContainerStatement(lineNumber);
        }

        return _usesStatementDictionary.TryGetValue(lineNumber, out var variableList)
            ? variableList.Select(variableName => programContext.VariablesDictionary[variableName].ToVariable())
            : Enumerable.Empty<Variable>();
    }

    private IEnumerable<Variable> GetVariablesForContainerStatement(int lineNumber)
    {
        var variableList = new HashSet<string>();

        if (_usesStatementDictionary.TryGetValue(lineNumber, out var flatModifiedVariables))
        {
            variableList.UnionWith(flatModifiedVariables);
        }

        if (callsRelation.GetAllContainerCalls().TryGetValue(lineNumber, out var procedureList))
        {
            variableList
                .UnionWith(procedureList
                    .SelectMany(procedure => _usesProcedureDictionaryFull[procedure])
                    .Distinct());
        }

        return variableList.Select(variable => programContext.VariablesDictionary[variable].ToVariable());
    }

    #endregion

    public IEnumerable<Statement> GetStatements()
    {
        var defaultAndContainerStatements = _usesStatementDictionary.Keys.AsEnumerable();

        var callStatements = programContext.StatementsDictionary
            .Where(statement => statement.Value.Type.IsCallStatement())
            .Where(statement => _usesProcedureDictionaryFull[statement.Value.Attribute!].Count != 0)
            .Select(statement => statement.Key);

        return defaultAndContainerStatements
            .Union(callStatements)
            .Select(statement => programContext.StatementsDictionary[statement].ToStatement());
    }

    #region GetStatements(string)

    public IEnumerable<Statement> GetStatements(string variableName)
    {
        // Get all procedures that use given variable
        var proceduresThatUseVariable = _usesProcedureDictionaryFull
            .Where(procedure => procedure.Value.Contains(variableName))
            .Select(procedure => procedure.Key)
            .ToArray();

        // Get all calls that modify given variable
        var callStatements = programContext.StatementsDictionary
            .Where(stmt =>
                stmt.Value.Type.IsCallStatement() &&
                proceduresThatUseVariable.Contains(stmt.Value.Attribute))
            .Select(stmt => stmt.Value.ToStatement());

        // Get all container statements that modify given variable by calling procedures that modify given variable
        var containerStatements = programContext.StatementsDictionary
            .Where(stmt =>
                stmt.Value.Type.IsContainerStatement() &&
                CallsProcedureThatUseVariable(stmt.Key, proceduresThatUseVariable))
            .Select(stmt => stmt.Value.ToStatement());

        return _usesStatementDictionary
            .Where(stmt => stmt.Value.Contains(variableName))
            .Select(stmt => programContext.StatementsDictionary[stmt.Key].ToStatement())
            .Union(callStatements)
            .Union(containerStatements);
    }

    private bool CallsProcedureThatUseVariable(int lineNumber, IEnumerable<string> proceduresThatUseVariable)
    {
        return callsRelation.GetAllContainerCalls().TryGetValue(lineNumber, out var procedureList) &&
               procedureList.Any(proceduresThatUseVariable.Contains);
    }

    #endregion

    public IEnumerable<Procedure> GetProcedures(string variableName)
    {
        return _usesProcedureDictionaryFull.Where(procedure => procedure.Value.Contains(variableName))
            .Select(procedure => programContext.ProceduresDictionary[procedure.Key].ToProcedure());
    }

    public IEnumerable<Variable> GetVariables(string procedureName)
    {
        return _usesProcedureDictionaryFull.TryGetValue(procedureName, out var variableList)
            ? variableList.Select(variableName => programContext.VariablesDictionary[variableName].ToVariable())
            : Enumerable.Empty<Variable>();
    }

    public bool IsUsed(int lineNumber, string variableName) =>
        GetVariables(lineNumber).Any(variable => variable.Name == variableName);

    public bool IsUsed(string procedureName, string variableName) =>
        GetVariables(procedureName).Any(variable => variable.Name == variableName);
}