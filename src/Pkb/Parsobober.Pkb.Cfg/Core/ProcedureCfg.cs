using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Cfg.Abstractions;

namespace Parsobober.Pkb.Cfg.Core;

public class ProcedureCfg
{
    private readonly Dictionary<int, ICfgNode> _nodeDictionary = new();
    public ICfgNode EntryNode { get; private set; } = null!;
    public IReadOnlyDictionary<int, ICfgNode> NodeDictionary => _nodeDictionary;

    public ProcedureCfg(TreeNode procedureNode)
    {
        GenerateGraph(procedureNode);
    }

    public ICfgNode? FindByLineNumber(int lineNumber)
    {
        _nodeDictionary.TryGetValue(lineNumber, out var node);
        return node;
    }

    public IEnumerable<ICfgNode> Search(ICfgNode startNode, ICfgTraversalStrategy strategy)
    {
        return strategy.Traverse(startNode);
    }

    private ICfgNode CreateNode(int lineNumber)
    {
        if (_nodeDictionary.TryGetValue(lineNumber, out var node))
        {
            return node;
        }

        var newNode = new CfgNode(lineNumber);
        _nodeDictionary[lineNumber] = newNode;
        return newNode;
    }

    #region GenerateGraph

    private void GenerateGraph(TreeNode procedureNode)
    {
        // procedureNode[0] -> stmtListNode[0] -> firstStatement
        var entryTreeNode = procedureNode.Children[0].Children[0];
        EntryNode = CreateNode(entryTreeNode.LineNumber);
        var (_, endSet) = entryTreeNode.Type switch
        {
            EntityType.Assign or EntityType.Call => GenerateForOtherNodeRecursive(EntryNode, entryTreeNode),
            EntityType.If => GenerateForIfNodeRecursive(EntryNode, entryTreeNode),
            EntityType.While => GenerateForWhileNodeRecursive(EntryNode, entryTreeNode),
            _ => throw new NotSupportedException($"Node type: {entryTreeNode.Type} is not supported.")
        };
        if (endSet is null || entryTreeNode.Type != EntityType.While)
        {
            return;
        }

        foreach (var node in endSet)
        {
            node.AddNext(EntryNode);
        }
    }

    private (ICfgNode, HashSet<ICfgNode>?) GenerateGraphRecursive(
        ICfgNode previousCfgNode,
        TreeNode? currentTreeNode,
        HashSet<ICfgNode>? endNodes = null)
    {
        if (currentTreeNode is null)
        {
            return (previousCfgNode, endNodes);
        }

        var currentNode = CreateNode(currentTreeNode.LineNumber);

        if (endNodes is not null)
        {
            foreach (var endNode in endNodes)
            {
                endNode.AddNext(currentNode);
            }
        }
        else
        {
            previousCfgNode.AddNext(currentNode);
        }

        return currentTreeNode.Type switch
        {
            EntityType.Assign or EntityType.Call => GenerateForOtherNodeRecursive(currentNode, currentTreeNode),
            EntityType.If => GenerateForIfNodeRecursive(currentNode, currentTreeNode),
            EntityType.While => GenerateForWhileNodeRecursive(currentNode, currentTreeNode),
            _ => throw new NotSupportedException($"Node type: {currentTreeNode.Type} is not supported.")
        };
    }

    private (ICfgNode, HashSet<ICfgNode>?) GenerateForOtherNodeRecursive(ICfgNode currentNode, TreeNode currentTreeNode)
    {
        return currentTreeNode.RightSibling is null
            ? (currentNode, null)
            : GenerateGraphRecursive(currentNode, currentTreeNode.RightSibling);
    }

    private (ICfgNode, HashSet<ICfgNode>?) GenerateForIfNodeRecursive(ICfgNode currentNode, TreeNode currentTreeNode)
    {
        var thenTreeNode = currentTreeNode.Children[1].Children[0];
        var elseTreeNode = currentTreeNode.Children[2].Children[0];

        var (lastThenCfgNode, thenEndSet) = GenerateGraphRecursive(currentNode, thenTreeNode);
        var (lastElseCfgNode, elseEndSet) = GenerateGraphRecursive(currentNode, elseTreeNode);

        if (currentTreeNode.RightSibling is null)
        {
            return (thenEndSet, elseEndSet) switch
            {
                (null, null) => (currentNode, new HashSet<ICfgNode>() { lastThenCfgNode, lastElseCfgNode }),
                (null, not null) => (currentNode, [.. elseEndSet, lastThenCfgNode]),
                (not null, null) => (currentNode, [.. thenEndSet, lastElseCfgNode]),
                (not null, not null) => (currentNode, [.. thenEndSet, .. elseEndSet])
            };
        }

        return (thenEndSet, elseEndSet) switch
        {
            (null, null) => GenerateGraphRecursive(currentNode, currentTreeNode.RightSibling,
                new HashSet<ICfgNode>() { lastThenCfgNode, lastElseCfgNode }),
            (null, not null) => GenerateGraphRecursive(currentNode, currentTreeNode.RightSibling,
                [.. elseEndSet, lastThenCfgNode]),
            (not null, null) => GenerateGraphRecursive(currentNode, currentTreeNode.RightSibling,
                [.. thenEndSet, lastElseCfgNode]),
            (not null, not null) => GenerateGraphRecursive(currentNode, currentTreeNode.RightSibling,
                [.. thenEndSet, .. elseEndSet])
        };
    }

    private (ICfgNode, HashSet<ICfgNode>?) GenerateForWhileNodeRecursive(ICfgNode currentNode, TreeNode currentTreeNode)
    {
        var insideNode = currentTreeNode.Children[1].Children[0];
        var (lastInsideCfgNode, insideEndSet) = GenerateGraphRecursive(currentNode, insideNode);

        if (insideEndSet is not null)
        {
            foreach (var endNode in insideEndSet)
            {
                endNode.AddNext(currentNode);
            }
        }
        else
        {
            lastInsideCfgNode.AddNext(currentNode);
        }

        return GenerateGraphRecursive(currentNode, currentTreeNode.RightSibling);
    }

    #endregion
}