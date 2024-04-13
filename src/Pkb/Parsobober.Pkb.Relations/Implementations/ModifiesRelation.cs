using Microsoft.Extensions.Logging;
using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Abstractions.Creators;

namespace Parsobober.Pkb.Relations.Implementations;

public class ModifiesRelation(
    ILogger<ParentRelation> logger,
    IProgramContextAccessor programContext
) : IModifiesCreator
{
    /// <summary>
    /// Stores modifies relation between statement(assign) and variable.
    /// </summary>
    /// <remarks>[statement(assign) line number,  variable name].</remarks>
    private readonly Dictionary<int, string> _statementModifiesDictionary = new();

    /// <summary>
    /// Stores modifies relation between container statement(while, if) and variable list
    /// </summary>
    /// <remarks>[statement(while, if) line number,  list of variable names].</remarks>
    private readonly Dictionary<int, List<string>> _containerModifiesDictionary = new();

    public void SetModifies(TreeNode modifier, TreeNode variable)
    {
        // Check types
        if (modifier.Type.IsStatement() == false || variable.Type != EntityType.Variable)
        {
            logger.LogError(
                "Statement modifies relation can only be established between statement and variable node. ({modifier} => {variable})",
                modifier, variable);

            throw new ArgumentException(
                $"At least one of provided nodes type: {modifier.Type}, {variable.Type} is different than any of required: {EntityType.Statement} types or {EntityType.Variable}.");
        }

        // Check required attribute
        if (variable.Attribute is null)
        {
            logger.LogError("Variable must have an attribute. ({node})", variable);

            throw new ArgumentNullException(variable.Attribute);
        }

        // Add to dictionary based on modifier type
        if (modifier.Type.IsContainerStatement())
        {
            SetContainerModifies(modifier, variable);
        }
        else
        {
            _statementModifiesDictionary.TryAdd(modifier.LineNumber, variable.Attribute);
        }
    }

    private void SetContainerModifies(TreeNode modifer, TreeNode variable)
    {
        if (_containerModifiesDictionary.TryGetValue(modifer.LineNumber, out var variableList))
        {
            if (variableList.Contains(variable.Attribute!))
            {
                return;
            }

            variableList.Add(variable.Attribute!);

            return;
        }

        _containerModifiesDictionary.Add(modifer.LineNumber, [variable.Attribute!]);
    }
}