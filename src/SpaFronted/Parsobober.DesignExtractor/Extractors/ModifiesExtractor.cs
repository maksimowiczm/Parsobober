﻿using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Creators;

namespace Parsobober.DesignExtractor.Extractors;

internal class ModifiesExtractor(IModifiesCreator creator) : SimpleExtractor
{
    private Stack<List<TreeNode>> containerStack = new();
    private List<TreeNode> procedureVariables = new();

    public override void Procedure(TreeNode node)
    {
        foreach (var variable in procedureVariables.Distinct())
        {
            creator.SetModifies(node, variable);
        }

        procedureVariables.Clear();
    }

    public override void StmtLst()
    {
        containerStack.Push(new List<TreeNode>());
    }

    public override void While(TreeNode result)
    {
        var varList = containerStack.Pop();
        foreach (var variable in varList)
        {
            creator.SetModifies(result, variable);
        }

        containerStack.Peek().AddRange(varList);
    }

    public override void If(TreeNode result)
    {
        var varElseList = containerStack.Pop();
        containerStack.Peek().AddRange(varElseList);
        var varList = containerStack.Pop();
        foreach (var variable in varList)
        {
            creator.SetModifies(result, variable);
        }

        containerStack.Peek().AddRange(varList);
    }

    public override void Assign(TreeNode result)
    {
        var leftVariable = result.Children[0];
        creator.SetModifies(result, leftVariable);
        procedureVariables.Add(leftVariable);
        containerStack.Peek().Add(leftVariable);
    }
}