using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Creators;

namespace Parsobober.DesignExtractor.Extractors;

internal class CallsExtractor(ICallsCreator creator) : SimpleExtractor
{
    private readonly List<TreeNode> _callNodes = [];
    private readonly Stack<List<TreeNode>> _containerStack = new();

    public override void Procedure(TreeNode node)
    {
        if (_callNodes.Count == 0)
        {
            return;
        }

        foreach (var callNode in _callNodes)
        {
            creator.SetCalls(node, callNode);
        }

        _callNodes.Clear();
    }

    public override void StmtLst()
    {
        _containerStack.Push(new List<TreeNode>());
    }

    public override void While(TreeNode result)
    {
        var varList = _containerStack.Pop();
        foreach (var callStatement in varList)
        {
            creator.SetContainerCalls(result, callStatement);
        }

        _containerStack.Peek().AddRange(varList);
    }

    public override void If(TreeNode result)
    {
        var callStatementElseList = _containerStack.Pop();
        _containerStack.Peek().AddRange(callStatementElseList);
        var callStatementList = _containerStack.Pop();
        foreach (var callStatement in callStatementList)
        {
            creator.SetContainerCalls(result, callStatement);
        }

        _containerStack.Peek().AddRange(callStatementList);
    }

    public override void Call(TreeNode node)
    {
        _callNodes.Add(node);
        _containerStack.Peek().Add(node);
    }
}