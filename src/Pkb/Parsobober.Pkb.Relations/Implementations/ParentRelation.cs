using Microsoft.Extensions.Logging;
using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Ast.AstTraverser;
using Parsobober.Pkb.Ast.AstTraverser.Strategies;
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
    /// [child node line number, parent node line number]
    /// </summary>
    private readonly Dictionary<int, int> _childParentDictionary = new();

    /// <summary>
    /// [parent node line number, set of children node line numbers]
    /// </summary>
    private readonly Dictionary<int, HashSet<int>> _parentChildrenDictionary = new();

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

        _childParentDictionary.TryAdd(childNode.LineNumber, parentNode.LineNumber);
        if (_parentChildrenDictionary.TryGetValue(parentNode.LineNumber, out var children))
        {
            children.Add(childNode.LineNumber);
            return;
        }

        _parentChildrenDictionary.Add(parentNode.LineNumber, [childNode.LineNumber]);
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
        return _parentChildrenDictionary.TryGetValue(lineNumber, out var children)
            ? children.Select(statement => programContext.StatementsDictionary[statement].ToStatement())
            : Enumerable.Empty<Statement>();
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
        var procedures = programContext.ProceduresDictionary.Values;
        var resultSet = new HashSet<Statement>();

        foreach (var procedure in procedures)
        {
            var traversedAst = procedure.Traverse(new DfsStatementStrategy());
            var containerDepth = -1;

            foreach (var (node, depth) in traversedAst)
            {
                if (depth <= containerDepth)
                {
                    containerDepth = -1;
                }
                else if (containerDepth != -1 && node.Type.IsStatement())
                {
                    resultSet.Add(node.ToStatement());
                }
                else if (node.IsType<TParentStatement>() && node.Type.IsContainerStatement())
                {
                    containerDepth = depth;
                }
            }
        }

        return resultSet;
    }

    public IEnumerable<Statement> GetChildrenTransitive(int lineNumber)
    {
        if (!programContext.StatementsDictionary.TryGetValue(lineNumber, out var statementNode))
        {
            return Enumerable.Empty<Statement>();
        }

        var traversedAst = statementNode.Traverse(new DfsStatementStrategy());

        return traversedAst
            .Where(visited => visited.node.Type.IsStatement())
            .Select(visited => visited.node.ToStatement());
    }

    public IEnumerable<Statement> GetParentsTransitive<TChildStatement>() where TChildStatement : Statement
    {
        var procedures = programContext.ProceduresDictionary.Values;
        var resultSet = new HashSet<Statement>();

        foreach (var procedure in procedures)
        {
            var traversedAst = procedure.Traverse(new DfsStatementStrategy());
            var containerStack = new Stack<(TreeNode node, int depth)>();

            foreach (var visited in traversedAst)
            {
                while (containerStack.Count != 0 && containerStack.Peek().depth >= visited.depth)
                {
                    containerStack.Pop();
                }

                if (visited.node.IsType<TChildStatement>())
                {
                    foreach (var parentStatement in containerStack.Select(container => container.node.ToStatement()))
                    {
                        resultSet.Add(parentStatement);
                    }
                }

                if (visited.node.Type.IsContainerStatement())
                {
                    containerStack.Push(visited);
                }
            }
        }

        return resultSet;
    }

    public IEnumerable<Statement> GetParentsTransitive(int lineNumber)
    {
        if (!programContext.StatementsDictionary.TryGetValue(lineNumber, out var statementNode))
        {
            return Enumerable.Empty<Statement>();
        }

        var traversedAst = statementNode.Traverse(new OnlyParentStrategy());

        return traversedAst
            .Where(visited => visited.node.Type.IsContainerStatement())
            .Select(visited => visited.node.ToStatement());
    }

    public bool IsParent(int parentLineNumber, int childLineNumber) =>
        GetParent(childLineNumber) switch
        {
            { Line: var line } => line == parentLineNumber,
            _ => false
        };

    public bool IsParentTransitive(int parentLineNumber, int childLineNumber) =>
        GetParentsTransitive(childLineNumber).Any(p => p.Line == parentLineNumber);
}