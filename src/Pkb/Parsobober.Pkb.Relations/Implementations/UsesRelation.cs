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
    /// [statement line number,  list of variable names].
    /// </summary>
    /// <remarks>!USED ONLY FOR COMPUTING!</remarks>
    private readonly Dictionary<int, HashSet<string>> _usesStatementDictionary = new();

    /// <summary>
    /// [procedure name, list of variable names].
    /// </summary>
    /// <remarks>!USED ONLY FOR COMPUTING!</remarks>
    private readonly Dictionary<string, HashSet<string>> _usesProcedureDictionary = new();

    /// <summary>
    /// [procedure name, list of variable names].
    /// </summary>
    private readonly Dictionary<string, HashSet<string>> _usesProcedureDictionaryFull = new();

    /// <summary>
    /// [statement line number,  list of variable names].
    /// </summary>
    private readonly Dictionary<int, HashSet<string>> _usesStatementDictionaryFull = new();

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
        // Compute for procedures
        foreach (var procedure in programContext.ProceduresDictionary.Keys.Reverse())
        {
            var modifiedVariables = new HashSet<string>();
            ComputeForProcedure(procedure, modifiedVariables);
            _usesProcedureDictionaryFull.Add(procedure, modifiedVariables);
        }

        // Compute for call statements
        var callStatements = programContext.StatementsDictionary
            .Where(e => e.Value.Type.IsCallStatement())
            .Select(e => e.Key);

        foreach (var call in callStatements)
        {
            var modified = _usesProcedureDictionaryFull[programContext.StatementsDictionary[call].Attribute!];
            _usesStatementDictionaryFull.Add(call, modified);
        }

        // Compute for assign
        var assignStatements = programContext.StatementsDictionary
            .Where(e => e.Value.Type.IsAssign())
            .Select(e => e.Key);

        // Compute for containers (can be optimized but complicated and probably not necessary)
        var containerStatements = programContext.StatementsDictionary
            .Where(e => e.Value.Type.IsContainerStatement())
            .Select(e => e.Key)
            .Reverse();
        foreach (var container in containerStatements)
        {
            var modifiedVariables = ComputeForContainerStatement(container);
            _usesStatementDictionaryFull.Add(container, modifiedVariables);
        }

        foreach (var assignLineNumber in assignStatements)
        {
            _usesStatementDictionaryFull.Add(assignLineNumber,
                _usesStatementDictionary.TryGetValue(assignLineNumber, out var variableList) ? variableList : []);
        }
        
        // Clear unused dictionaries
        _usesStatementDictionary.Clear();
        _usesProcedureDictionary.Clear();
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
                proceduresToVisit.Push(procedure.Name);
            }
        }
    }

    private HashSet<string> ComputeForContainerStatement(int statementLineNumber)
    {
        var modifiedVariables = new HashSet<string>();
        if (_usesStatementDictionary.TryGetValue(statementLineNumber, out var flatModifiedVariables))
        {
            modifiedVariables.UnionWith(flatModifiedVariables);
        }

        if (callsRelation.GetAllContainerCalls().TryGetValue(statementLineNumber, out var procedureList))
        {
            foreach (var procedure in procedureList)
            {
                modifiedVariables.UnionWith(_usesProcedureDictionaryFull[procedure]);
            }
        }

        return modifiedVariables;
    }

    #endregion

    public IEnumerable<Variable> GetVariables<T>() where T : IRequest
    {
        return _usesStatementDictionaryFull
            .Where(stmt => programContext.StatementsDictionary[stmt.Key].IsType<T>())
            .SelectMany(stmt => stmt.Value)
            .Distinct()
            .Select(variableName => programContext.VariablesDictionary[variableName].ToVariable());
    }

    public IEnumerable<Variable> GetVariables(int lineNumber)
    {
        return _usesStatementDictionaryFull.TryGetValue(lineNumber, out var variableList)
            ? variableList.Select(variableName => programContext.VariablesDictionary[variableName].ToVariable())
            : Enumerable.Empty<Variable>();
    }

    public IEnumerable<Statement> GetStatements()
    {
        return _usesStatementDictionaryFull.Keys
            .Select(lineNumber => programContext.StatementsDictionary[lineNumber].ToStatement());
    }

    public IEnumerable<Statement> GetStatements(string variableName)
    {
        return _usesStatementDictionaryFull
            .Where(stmt => stmt.Value.Contains(variableName))
            .Select(stmt => programContext.StatementsDictionary[stmt.Key].ToStatement());
    }

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