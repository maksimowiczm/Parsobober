using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Creators;

namespace Parsobober.Simple.Parser.Extractor
{
    internal class ModifiesExtractor(IModifiesCreator creator) : SimpleExtractor
    {
        private readonly IModifiesCreator creator = creator;
        private Stack<TreeNode> stack = new Stack<TreeNode>();

        public override void While(TreeNode result)
        {
            Stack<TreeNode> tempStack = new Stack<TreeNode>();
            while (stack.Count > 0)
            {
                TreeNode node = stack.Pop();
                creator.SetModifies(result, node);
                tempStack.Push(node);
            }
            stack = new Stack<TreeNode>(tempStack);

        }

        public override void Assign(TreeNode result)
        {
            creator.SetModifies(result, result.Children[0]);
            stack.Push(result.Children[0]);
        }
    }
}
