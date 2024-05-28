using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Creators;

namespace Parsobober.DesignExtractor.Extractors;

internal class UsesExtractor(IUsesCreator creator) : SimpleExtractor
{
    private Stack<List<TreeNode>> containerStack = new();
    private List<TreeNode> assignVariables = new();

    public override void StmtLst()
    {
        containerStack.Push(new List<TreeNode>());
    }

    public override void While(TreeNode result)
    {
        var varList = containerStack.Pop();
        foreach (var variable in varList)
        {
            creator.SetUses(result, variable);
        }
        containerStack.Peek().AddRange(varList);

        creator.SetUses(result, result.Children[0]);
        containerStack.Peek().Add(result.Children[0]);
    }

    public override void If(TreeNode result)
    {
        var varElseList = containerStack.Pop();
        containerStack.Peek().AddRange(varElseList);
        var varList = containerStack.Pop();
        foreach (var variable in varList)
        {
            creator.SetUses(result, variable);
        }
        containerStack.Peek().AddRange(varList);

        creator.SetUses(result, result.Children[0]);
        containerStack.Peek().Add(result.Children[0]);
    }


    public override void Assign(TreeNode result)
    {
        foreach (var variable in assignVariables)
        {
            creator.SetUses(result, variable);
        }
        containerStack.Peek().AddRange(assignVariables);
        assignVariables.Clear();
    }

    public override void Factor(TreeNode result)
    {
        if (result.Type == EntityType.Variable)
        {
            assignVariables.Add(result);
        }
    }
}