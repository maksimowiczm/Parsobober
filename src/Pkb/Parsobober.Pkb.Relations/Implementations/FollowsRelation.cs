using Microsoft.Extensions.Logging;
using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Ast.Abstractions;
using Parsobober.Pkb.Ast.AstTraverser;
using Parsobober.Pkb.Ast.AstTraverser.Strategies;
using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Abstractions.Creators;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pkb.Relations.Utilities;

namespace Parsobober.Pkb.Relations.Implementations;

public class FollowsRelation(
    ILogger<FollowsRelation> logger,
    IProgramContextAccessor programContext,
    IAst ast
) : IFollowsCreator, IFollowsAccessor
{
    /// <summary>
    /// Stores follows relation between two statements using their line numbers.
    /// </summary>
    /// <remarks>[following node line number, preceding node line number].</remarks>
    private readonly Dictionary<int, int> _followsDictionary = new();

    public void SetFollows(TreeNode node, TreeNode parent)
    {
        if (!node.Type.IsStatement() || !parent.Type.IsStatement())
        {
            logger.LogError(
                "Follows relation can only be established between two statement nodes. ({parent} => {child})",
                parent, node);

            throw new ArgumentException(
                $"At least one of provided nodes type: {parent.Type}, {node.Type} is different than any of required: {EntityType.Statement} types.");
        }

        if (!_followsDictionary.TryAdd(node.LineNumber, parent.LineNumber))
        {
            logger.LogWarning("Relation {node} follows {parent} already exists",
                node.LineNumber, parent.LineNumber);
        }
    }

    public IEnumerable<Statement> GetFollowers<TStatement>() where TStatement : Statement
    {
        return _followsDictionary
            .Where(statement => programContext.StatementsDictionary[statement.Value].IsType<TStatement>())
            .Select(statement => programContext.StatementsDictionary[statement.Key].ToStatement())
            .Distinct();
    }

    public Statement? GetFollower(int lineNumber)
    {
        var followerStmt = _followsDictionary.SingleOrDefault(stmt => stmt.Value == lineNumber);
        if (followerStmt.Equals(default(KeyValuePair<int, int>)))
        {
            return null;
        }

        return programContext.StatementsDictionary[followerStmt.Key].ToStatement();
    }

    public IEnumerable<Statement> GetFollowed<TStatement>() where TStatement : Statement
    {
        return _followsDictionary
            .Where(statement => programContext.StatementsDictionary[statement.Key].IsType<TStatement>())
            .Select(statement => programContext.StatementsDictionary[statement.Value].ToStatement())
            .Distinct();
    }

    public Statement? GetFollowed(int lineNumber)
    {
        return _followsDictionary.TryGetValue(lineNumber, out var statement)
            ? programContext.StatementsDictionary[statement].ToStatement()
            : null;
    }

    public IEnumerable<Statement> GetFollowersTransitive<TStatement>() where TStatement : Statement
    {
        return GetTransitive<TStatement>(new BfsReversedStatementStrategy());
    }

    public IEnumerable<Statement> GetFollowersTransitive(int lineNumber)
    {
        if (!programContext.StatementsDictionary.TryGetValue(lineNumber, out var statementNode))
        {
            return Enumerable.Empty<Statement>();
        }

        var traversedAst =
            AstTraverser.Traverse(statementNode, new RightSiblingStrategy());

        return traversedAst
            .Where(visited => visited.node.Type.IsStatement())
            .Select(visited => visited.node.ToStatement());
    }

    public IEnumerable<Statement> GetFollowedTransitive<TStatement>() where TStatement : Statement
    {
        return GetTransitive<TStatement>(new BfsStatementStrategy());
    }

    public IEnumerable<Statement> GetFollowedTransitive(int lineNumber)
    {
        if (!programContext.StatementsDictionary.TryGetValue(lineNumber, out var statementNode))
        {
            return Enumerable.Empty<Statement>();
        }

        var traversedAst =
            AstTraverser.Traverse(statementNode, new LeftSiblingStrategy());

        return traversedAst
            .Where(visited => visited.node.Type.IsStatement())
            .Select(visited => visited.node.ToStatement());
    }

    private IEnumerable<Statement> GetTransitive<TStatement>(IAstTraversalStrategy strategy)
        where TStatement : Statement
    {
        var traversedAst = AstTraverser.Traverse(ast.Root, strategy);
        var set = new HashSet<TreeNode>();

        TreeNode? currentScope = null;
        var currentScopeFollowedList = new Stack<TreeNode>();

        foreach (var (node, _) in traversedAst)
        {
            // if nodes have different parent it means they are not in same scope
            if (currentScope != node.Parent)
            {
                currentScope = node.Parent;
                currentScopeFollowedList.Clear();
            }

            if (node.IsType<TStatement>())
            {
                set.UnionWith(currentScopeFollowedList);
                currentScopeFollowedList.Clear();
            }

            currentScopeFollowedList.Push(node);
        }

        return set.Select(node => node.ToStatement());
    }
}