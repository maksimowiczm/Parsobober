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
    /// [preceding node line number, following node line number]
    /// </summary>
    private readonly Dictionary<int, int> _predecessorDictionary = new();

    /// <summary>
    /// [following node line number, preceding node line number]
    /// </summary>
    private readonly Dictionary<int, int> _followerDictionary = new();

    public void SetFollows(TreeNode preceding, TreeNode following)
    {
        if (!preceding.Type.IsStatement() || !following.Type.IsStatement())
        {
            logger.LogError(
                "Follows relation can only be established between two statement nodes. ({parent} => {child})",
                following, preceding);

            throw new ArgumentException(
                $"At least one of provided nodes type: {preceding.Type}, {following.Type} is different than any of required: {EntityType.Statement} types.");
        }

        _predecessorDictionary.TryAdd(preceding.LineNumber, following.LineNumber);
        _followerDictionary.TryAdd(following.LineNumber, preceding.LineNumber);
    }

    public IEnumerable<Statement> GetFollowers<TStatement>() where TStatement : Statement
    {
        return _predecessorDictionary
            .Where(statement => programContext.StatementsDictionary[statement.Key].IsType<TStatement>())
            .Select(statement => programContext.StatementsDictionary[statement.Value].ToStatement())
            .Distinct();
    }

    public Statement? GetFollower(int lineNumber)
    {
        return _predecessorDictionary.TryGetValue(lineNumber, out var statement)
            ? programContext.StatementsDictionary[statement].ToStatement()
            : null;
    }

    public IEnumerable<Statement> GetPreceding<TStatement>() where TStatement : Statement
    {
        return _followerDictionary
            .Where(statement => programContext.StatementsDictionary[statement.Key].IsType<TStatement>())
            .Select(statement => programContext.StatementsDictionary[statement.Value].ToStatement())
            .Distinct();
    }

    public Statement? GetPreceding(int lineNumber)
    {
        return _followerDictionary.TryGetValue(lineNumber, out var statement)
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

        var traversedAst = statementNode.Traverse(new RightSiblingStrategy());

        return traversedAst
            .Where(visited => visited.node.Type.IsStatement())
            .Select(visited => visited.node.ToStatement());
    }

    public IEnumerable<Statement> GetPrecedingTransitive<TStatement>() where TStatement : Statement
    {
        return GetTransitive<TStatement>(new BfsStatementStrategy());
    }

    public IEnumerable<Statement> GetPrecedingTransitive(int lineNumber)
    {
        if (!programContext.StatementsDictionary.TryGetValue(lineNumber, out var statementNode))
        {
            return Enumerable.Empty<Statement>();
        }

        var traversedAst = statementNode.Traverse(new LeftSiblingStrategy());

        return traversedAst
            .Where(visited => visited.node.Type.IsStatement())
            .Select(visited => visited.node.ToStatement());
    }

    public bool IsFollows(int predecessor, int follower) =>
        GetPreceding(follower) switch
        {
            { Line: var line } => line == predecessor,
            _ => false
        };

    public bool IsFollowsTransitive(int predecessor, int follower) =>
        GetPrecedingTransitive(follower).Any(f => f.Line == predecessor);

    private IEnumerable<Statement> GetTransitive<TStatement>(IAstTraversalStrategy strategy)
        where TStatement : Statement
    {
        var procedures = programContext.ProceduresDictionary.Values;
        var set = new HashSet<TreeNode>();
        foreach (var procedure in procedures)
        {
            var traversedAst = procedure.Traverse(strategy);
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
        }

        return set.Select(node => node.ToStatement());
    }
}