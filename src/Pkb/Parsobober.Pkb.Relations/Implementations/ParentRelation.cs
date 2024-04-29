using Microsoft.Extensions.Logging;
using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Abstractions.Creators;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pkb.Relations.Utilities;

namespace Parsobober.Pkb.Relations.Implementations;

public class ParentRelation(
    ILogger<ParentRelation> logger,
    IProgramContextAccessor programContext
) : IParentCreator, IParentAccessor
{
    /// <summary>
    /// Stores parent relation between two statements using their line numbers.
    /// Key is a line number of a CHILD node, value is a line number of a PARENT node (child can have only one parent).
    /// </summary>
    /// <remarks>[child node line number, parent node line number].</remarks>
    private readonly Dictionary<int, int> _childParentDictionary = new();

    public void SetParent(TreeNode parentNode, TreeNode childNode)
    {
        if (!parentNode.Type.IsContainerStatement())
        {
            logger.LogError(
                "Parent relation can only be established between container statement and statement node. ({parent} must be container statement)",
                parentNode);

            throw new ArgumentException(
                $"Parent node type {parentNode.Type} is different than any of required {EntityType.Statement} container types.");
        }

        if (!childNode.Type.IsStatement())
        {
            logger.LogError(
                "Parent relation can only be established between container statement and statement node. ({child} must be statement)",
                childNode);

            throw new ArgumentException(
                $"Child node type {parentNode.Type} is different than any of required {EntityType.Statement} types.");
        }

        if (!_childParentDictionary.TryAdd(childNode.LineNumber, parentNode.LineNumber))
        {
            logger.LogError("Relation {parent} parents {child} already exists",
                parentNode.LineNumber, childNode.LineNumber);
        }
    }

    public IEnumerable<Statement> GetChildren<TParentStatement>() where TParentStatement : Statement
    {
        return _childParentDictionary
            .Where(statement => programContext.StatementsDictionary[statement.Value].IsType<TParentStatement>())
            .Select(statement => programContext.StatementsDictionary[statement.Key].ToStatement())
            .Distinct();
    }

    public IEnumerable<Statement> GetChildren(int lineNumber)
    {
        return _childParentDictionary
            .Where(stmt => stmt.Value == lineNumber)
            .Select(stmt => programContext.StatementsDictionary[stmt.Key].ToStatement());
    }

    public IEnumerable<Statement> GetParents<TChildStatement>() where TChildStatement : Statement
    {
        return _childParentDictionary
            .Where(statement => programContext.StatementsDictionary[statement.Key].IsType<TChildStatement>())
            .Select(statement => programContext.StatementsDictionary[statement.Value].ToStatement())
            .Distinct();
    }

    public Statement? GetParent(int lineNumber)
    {
        return _childParentDictionary.TryGetValue(lineNumber, out var statement)
            ? programContext.StatementsDictionary[statement].ToStatement()
            : null;
    }

    public IEnumerable<Statement> GetChildrenTransitive<TParentStatement>() where TParentStatement : Statement
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Statement> GetChildrenTransitive(int lineNumber)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Statement> GetParentsTransitive<TChildStatement>() where TChildStatement : Statement
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Statement> GetParentsTransitive(int lineNumber)
    {
        throw new NotImplementedException();
    }
}