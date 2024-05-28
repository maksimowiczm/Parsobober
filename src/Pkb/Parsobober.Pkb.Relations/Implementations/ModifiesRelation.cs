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
    IProgramContextAccessor programContext
) : IModifiesCreator, IModifiesAccessor

{
    /// <summary>
    /// Stores modifies relation between statement and variable list
    /// </summary>
    /// <remarks>[statement line number,  list of variable names].</remarks>
    private readonly Dictionary<int, List<string>> _modifiesDictionary = new();

    public void SetModifies(TreeNode modifier, TreeNode variable)
    {
        // Check types
        if (!modifier.Type.IsStatement())
        {
            logger.LogError(
                "Statement modifies relation can only be established between statement and variable node. ({modifier} must be statement)",
                modifier);

            throw new ArgumentException(
                $"Modifier node type {modifier.Type} is different than any of required: {EntityType.Statement} types.");
        }

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

        if (_modifiesDictionary.TryGetValue(modifier.LineNumber, out var variableList))
        {
            if (variableList.Contains(variable.Attribute!))
            {
                return;
            }

            variableList.Add(variable.Attribute!);

            return;
        }

        _modifiesDictionary.Add(modifier.LineNumber, [variable.Attribute!]);
    }


    public IEnumerable<Variable> GetVariables<T>() where T : IRequest
    {
        return _modifiesDictionary
            .Where(statement => programContext.StatementsDictionary[statement.Key].IsType<T>())
            .SelectMany(statement => statement.Value)
            .Distinct()
            .Select(variable => programContext.VariablesDictionary[variable].ToVariable());
    }

    public IEnumerable<Statement> GetStatements()
    {
        return _modifiesDictionary.Select(entry =>
            programContext.StatementsDictionary[entry.Key].ToStatement()
        );
    }

    public IEnumerable<Variable> GetVariables(int lineNumber)
    {
        return _modifiesDictionary.TryGetValue(lineNumber, out var variableList)
            ? variableList.Select(variableName => programContext.VariablesDictionary[variableName].ToVariable())
            : Enumerable.Empty<Variable>();
    }

    public IEnumerable<Statement> GetStatements(string variableName)
    {
        return _modifiesDictionary
            .Where(stmt => stmt.Value.Contains(variableName))
            .Select(stmt => programContext.StatementsDictionary[stmt.Key].ToStatement());
    }

    public bool IsModified(int lineNumber, string variableName) =>
        GetVariables(lineNumber).Any(variable => variable.Name == variableName);

    public bool IsModified(string procedureName, string variableName)
    {
        throw new NotImplementedException();
    }
}