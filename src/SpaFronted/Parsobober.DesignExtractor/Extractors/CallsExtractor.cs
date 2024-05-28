using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Creators;

namespace Parsobober.DesignExtractor.Extractors;

internal class CallsExtractor(ICallsCreator creator) : SimpleExtractor
{
    private readonly List<TreeNode> _callNodes = [];

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

    public override void Call(TreeNode node)
    {
        _callNodes.Add(node);
    }
}