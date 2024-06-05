using Microsoft.Extensions.Logging;
using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions;
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
) : IModifiesCreator, IPostParseComputable, IModifiesAccessor

{
    /// <summary>
    /// Stores modifies relation between statement and variable set
    /// </summary>
    /// <remarks>[statement line number,  list of variable names].</remarks>
    private readonly Dictionary<int, HashSet<string>> _modifiesStatementDictionary = new();

    /// <summary>
    /// Stores modifies relation between procedure and variable set
    /// </summary>
    /// <remarks>[procedure name, list of variable names].</remarks>
    private readonly Dictionary<string, HashSet<string>> _modifiesProcedureDictionary = new();

    /// <summary>
    /// Stores modifies relation between procedure and variable set including calls connections
    /// </summary>
    /// <remarks>[procedure name, list of variable names].</remarks>
    private readonly Dictionary<string, HashSet<string>> _modifiesProcedureDictionaryFull = new();

    #region IModifiesCreator

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
            variableList.Add(variableName);
            return;
        }

        _modifiesStatementDictionary.Add(lineNumber, [variableName]);
    }

    private void AddToProcedureDictionary(string procedureName, string variableName)
    {
        if (_modifiesProcedureDictionary.TryGetValue(procedureName, out var variableList))
        {
            variableList.Add(variableName);
            return;
        }

        _modifiesProcedureDictionary.Add(procedureName, [variableName]);
    }

    #endregion

    #region PostParseComputable

    public void Compute()
    {
        foreach (var procedure in programContext.ProceduresDictionary.Keys.Reverse())
        {
            var modifiedVariables = new HashSet<string>();
            ComputeForProcedure(procedure, modifiedVariables);
            _modifiesProcedureDictionaryFull.Add(procedure, modifiedVariables);
        }
    }

    private void ComputeForProcedure(string procedureName, HashSet<string> modifiedVariables)
    {
        var visitedProcedures = new HashSet<string>();
        var proceduresToVisit = new Stack<string>(new[] { procedureName });

        while (proceduresToVisit.TryPop(out var currentProcedure))
        {
            if (!visitedProcedures.Add(currentProcedure))
            {
                if (_modifiesProcedureDictionaryFull.TryGetValue(currentProcedure, out var computedVariableList))
                {
                    modifiedVariables.UnionWith(computedVariableList);
                }

                continue;
            }

            if (_modifiesProcedureDictionary.TryGetValue(currentProcedure, out var variableList))
            {
                modifiedVariables.UnionWith(variableList);
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

    // Assuming that every Calls statement modifies at least one variable
    // (procedure needs at least calls or assign statement => somewhere in `calls line` there is procedure with assign)
    private IEnumerable<Variable> GetVariablesForCalls()
    {
        return callsRelation
            .GetAllCalledProcedures()
            .Select(procedure => _modifiesProcedureDictionaryFull[procedure])
            .SelectMany(variableList => variableList)
            .Distinct()
            .Select(variable => programContext.VariablesDictionary[variable].ToVariable());
    }

    private IEnumerable<Variable> GetVariablesForContainerStatements<T>() where T : IRequest
    {
        var flatModifiedVariables = _modifiesStatementDictionary
            .Where(statement => programContext.StatementsDictionary[statement.Key].IsType<T>())
            .SelectMany(statement => statement.Value)
            .Distinct();

        var transientModifiedVariables = callsRelation
            .GetAllContainerCalls()
            .Where(container => programContext.StatementsDictionary[container.Key].IsType<T>())
            .SelectMany(container => container.Value)
            .Distinct()
            .SelectMany(procedure => _modifiesProcedureDictionaryFull[procedure])
            .Distinct();

        return flatModifiedVariables
            .Union(transientModifiedVariables)
            .Select(variable => programContext.VariablesDictionary[variable].ToVariable());
    }

    private IEnumerable<Variable> GetVariablesForDefaultStatement<T>() where T : IRequest
    {
        return _modifiesStatementDictionary
            .Where(statement => programContext.StatementsDictionary[statement.Key].IsType<T>())
            .SelectMany(statement => statement.Value)
            .Distinct()
            .Select(variable => programContext.VariablesDictionary[variable].ToVariable());
    }

    // Only statements can modify variables, so it is safe to return all variables modified by procedures 
    private IEnumerable<Variable> GetVariableForGenericStatement()
    {
        return _modifiesProcedureDictionaryFull.Values
            .SelectMany(variableList => variableList)
            .Distinct()
            .Select(variable => programContext.VariablesDictionary[variable].ToVariable());
    }

    #endregion

    // Every statement have to modify at least one variable at some point
    public IEnumerable<Statement> GetStatements()
    {
        return programContext.StatementsDictionary.Keys
            .Select(lineNumber => programContext.StatementsDictionary[lineNumber].ToStatement());
    }

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

        return _modifiesStatementDictionary.TryGetValue(lineNumber, out var variableList)
            ? variableList.Select(variableName => programContext.VariablesDictionary[variableName].ToVariable())
            : Enumerable.Empty<Variable>();
    }

    private IEnumerable<Variable> GetVariablesForContainerStatement(int lineNumber)
    {
        var variableList = new HashSet<string>();

        if (_modifiesStatementDictionary.TryGetValue(lineNumber, out var flatModifiedVariables))
        {
            variableList.UnionWith(flatModifiedVariables);
        }

        if (callsRelation.GetAllContainerCalls().TryGetValue(lineNumber, out var procedureList))
        {
            variableList.UnionWith(procedureList
                .SelectMany(procedure => _modifiesProcedureDictionaryFull[procedure])
                .Distinct());
        }

        return variableList.Select(variable => programContext.VariablesDictionary[variable].ToVariable());
    }

    #endregion

    #region GetStatements(string)

    public IEnumerable<Statement> GetStatements(string variableName)
    {
        // Get all procedures that modify given variable
        var proceduresThatModifyVariable = _modifiesProcedureDictionaryFull
            .Where(procedure => procedure.Value.Contains(variableName))
            .Select(procedure => procedure.Key)
            .ToArray();

        // Get all calls that modify given variable
        var callStatements = programContext.StatementsDictionary
            .Where(stmt =>
                stmt.Value.Type.IsCallStatement() &&
                proceduresThatModifyVariable.Contains(stmt.Value.Attribute))
            .Select(stmt => stmt.Value.ToStatement());

        // Get all container statements that modify given variable by calling procedures that modify given variable
        var containerStatements = programContext.StatementsDictionary
            .Where(stmt =>
                stmt.Value.Type.IsContainerStatement() &&
                CallsProcedureThatModifiesVariable(stmt.Key, proceduresThatModifyVariable))
            .Select(stmt => stmt.Value.ToStatement());

        return _modifiesStatementDictionary
            .Where(stmt => stmt.Value.Contains(variableName))
            .Select(stmt => programContext.StatementsDictionary[stmt.Key].ToStatement())
            .Union(callStatements)
            .Union(containerStatements);
    }

    private bool CallsProcedureThatModifiesVariable(int lineNumber, IEnumerable<string> proceduresThatModifyVariable)
    {
        return callsRelation.GetAllContainerCalls().TryGetValue(lineNumber, out var procedureList) &&
               procedureList.Any(proceduresThatModifyVariable.Contains);
    }

    #endregion

    public IEnumerable<Procedure> GetProcedures(string variableName)
    {
        return _modifiesProcedureDictionaryFull.Where(procedure => procedure.Value.Contains(variableName))
            .Select(procedure => programContext.ProceduresDictionary[procedure.Key].ToProcedure());
    }

    public IEnumerable<Variable> GetVariables(string procedureName)
    {
        return _modifiesProcedureDictionaryFull.TryGetValue(procedureName, out var variableList)
            ? variableList.Select(variableName => programContext.VariablesDictionary[variableName].ToVariable())
            : Enumerable.Empty<Variable>();
    }

    public bool IsModified(int lineNumber, string variableName) =>
        GetVariables(lineNumber).Any(variable => variable.Name == variableName);

    public bool IsModified(string procedureName, string variableName) =>
        GetVariables(procedureName).Any(variable => variable.Name == variableName);
}