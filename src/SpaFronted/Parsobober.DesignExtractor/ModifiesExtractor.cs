using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Creators;

namespace Parsobober.Simple.Parser.Extractor;

internal class ModifiesExtractor(IModifiesCreator creator) : SimpleExtractor
{
    private Stack<TreeNode> stack = new();

    public override void While(TreeNode result)
    {
        var tempStack = new Stack<TreeNode>(stack);
        while (stack.Count > 0)
        {
            TreeNode node = stack.Pop();
            creator.SetModifies(result, node);
        }
        stack = tempStack;
    }

    public override void Assign(TreeNode result)
    {
        creator.SetModifies(result, result.Children[0]);
        stack.Push(result.Children[0]);
    }
}