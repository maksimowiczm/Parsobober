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
    IProgramContextAccessor programContext
) : IUsesCreator, IUsesAccessor
{
    /// <summary>
    /// Stores uses relation between statement and variables it uses.
    /// </summary>
    /// <remarks>[statement line number,  list of variable names].</remarks>
    private readonly Dictionary<int, List<string>> _usesDictionary = new();

    public void SetUses(TreeNode user, TreeNode variable)
    {
        // Check types
        if (!user.Type.IsStatement())
        {
            logger.LogError(
                "Statement uses relation can only be established between statement and variable node. ({user} must be statement)",
                user);

            throw new ArgumentException(
                $"User node type {user.Type} is different than any of required: {EntityType.Statement} types.");
        }

        if (!variable.Type.IsVariable())
        {
            logger.LogError(
                "Statement uses relation can only be established between statement and variable node. ({variable} must be variable)",
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

        // Add to dictionary
        if (_usesDictionary.TryGetValue(user.LineNumber, out var variableList))
        {
            if (variableList.Contains(variable.Attribute))
            {
                return;
            }

            variableList.Add(variable.Attribute);
            return;
        }

        _usesDictionary.Add(user.LineNumber, [variable.Attribute]);
    }

    public IEnumerable<Variable> GetVariables<T>() where T : IRequest
    {
        return _usesDictionary
            .Where(statement => programContext.StatementsDictionary[statement.Key].IsType<T>())
            .SelectMany(statement => statement.Value)
            .Distinct()
            .Select(variable => programContext.VariablesDictionary[variable].ToVariable());
    }

    public IEnumerable<Variable> GetVariables(int lineNumber)
    {
        return _usesDictionary.TryGetValue(lineNumber, out var variableList)
            ? variableList.Select(variableName => programContext.VariablesDictionary[variableName].ToVariable())
            : Enumerable.Empty<Variable>();
    }

    public IEnumerable<Statement> GetStatements()
    {
        return _usesDictionary.Select(entry =>
            programContext.StatementsDictionary[entry.Key].ToStatement()
        );
    }

    public IEnumerable<Statement> GetStatements(string variableName)
    {
        return _usesDictionary
            .Where(stmt => stmt.Value.Contains(variableName))
            .Select(stmt => programContext.StatementsDictionary[stmt.Key].ToStatement());
    }

    public bool IsUsed(int lineNumber, string variableName) =>
        GetVariables(lineNumber).Any(variable => variable.Name == variableName);

    public bool IsUsed(string procedureName, string variableName)
    {
        throw new NotImplementedException();
    }
}