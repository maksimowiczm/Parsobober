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
    /// [statement line number,  list of variable names].
    /// </summary>
    /// <remarks>!USED ONLY FOR COMPUTING!</remarks>
    private readonly Dictionary<int, HashSet<string>> _modifiesStatementDictionary = new();

    /// <summary>
    /// [procedure name, list of variable names].
    /// </summary>
    /// <remarks>!USED ONLY FOR COMPUTING!</remarks>
    private readonly Dictionary<string, HashSet<string>> _modifiesProcedureDictionary = new();

    /// <summary>
    /// [procedure name, list of variable names].
    /// </summary>
    private readonly Dictionary<string, HashSet<string>> _modifiesProcedureDictionaryFull = new();

    /// <summary>
    /// [statement line number,  list of variable names].
    /// </summary>
    private readonly Dictionary<int, HashSet<string>> _modifiesStatementDictionaryFull = new();

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
        // Compute for procedures
        foreach (var procedure in programContext.ProceduresDictionary.Keys.Reverse())
        {
            var modifiedVariables = new HashSet<string>();
            ComputeForProcedure(procedure, modifiedVariables);
            _modifiesProcedureDictionaryFull.Add(procedure, modifiedVariables);
        }

        // Compute for call statements
        var callStatements = programContext.StatementsDictionary
            .Where(e => e.Value.Type.IsCallStatement())
            .Select(e => e.Key);

        foreach (var call in callStatements)
        {
            var modified = _modifiesProcedureDictionaryFull[programContext.StatementsDictionary[call].Attribute!];
            _modifiesStatementDictionaryFull.Add(call, modified);
        }

        // Compute for assign
        var assignStatements = programContext.StatementsDictionary
            .Where(e => e.Value.Type.IsAssign())
            .Select(e => e.Key);

        foreach (var assignLineNumber in assignStatements)
        {
            _modifiesStatementDictionaryFull.Add(assignLineNumber, _modifiesStatementDictionary[assignLineNumber]);
        }

        // Compute for containers (can be optimized but complicated and probably not necessary)
        var containerStatements = programContext.StatementsDictionary
            .Where(e => e.Value.Type.IsContainerStatement())
            .Select(e => e.Key)
            .Reverse();
        foreach (var container in containerStatements)
        {
            var modifiedVariables = ComputeForContainerStatement(container);
            _modifiesStatementDictionaryFull.Add(container, modifiedVariables);
        }

        // Clear dictionaries that are no longer needed
        _modifiesStatementDictionary.Clear();
        _modifiesProcedureDictionary.Clear();
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
                proceduresToVisit.Push(procedure.Name);
            }
        }
    }

    private HashSet<string> ComputeForContainerStatement(int statementLineNumber)
    {
        var modifiedVariables = new HashSet<string>();
        if (_modifiesStatementDictionary.TryGetValue(statementLineNumber, out var flatModifiedVariables))
        {
            modifiedVariables.UnionWith(flatModifiedVariables);
        }

        if (callsRelation.GetAllContainerCalls().TryGetValue(statementLineNumber, out var procedureList))
        {
            foreach (var procedure in procedureList)
            {
                modifiedVariables.UnionWith(_modifiesProcedureDictionaryFull[procedure]);
            }
        }

        return modifiedVariables;
    }

    #endregion

    public IEnumerable<Variable> GetVariables<T>() where T : IRequest
    {
        return _modifiesStatementDictionaryFull
            .Where(stmt => programContext.StatementsDictionary[stmt.Key].IsType<T>())
            .SelectMany(stmt => stmt.Value)
            .Distinct()
            .Select(variableName => programContext.VariablesDictionary[variableName].ToVariable());
    }
    
    public IEnumerable<Statement> GetStatements()
    {
        return _modifiesStatementDictionaryFull.Keys
            .Select(lineNumber => programContext.StatementsDictionary[lineNumber].ToStatement());
    }
    
    public IEnumerable<Variable> GetVariables(int lineNumber)
    {
        return _modifiesStatementDictionaryFull.TryGetValue(lineNumber, out var variableList)
            ? variableList.Select(variableName => programContext.VariablesDictionary[variableName].ToVariable())
            : Enumerable.Empty<Variable>();
    }
    
    public IEnumerable<Statement> GetStatements(string variableName)
    {
        return _modifiesStatementDictionaryFull
            .Where(stmt => stmt.Value.Contains(variableName))
            .Select(stmt => programContext.StatementsDictionary[stmt.Key].ToStatement());
    }

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