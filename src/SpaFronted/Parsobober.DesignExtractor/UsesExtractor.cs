using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Creators;

namespace Parsobober.Simple.Parser.Extractor
{
    internal class UsesExtractor(IUsesCreator creator) : SimpleExtractor
    {
        private readonly IUsesCreator creator = creator;
        private Stack<TreeNode> containerStack = new Stack<TreeNode>();
        private Stack<TreeNode> assignStack = new Stack<TreeNode>();

        public override void While(TreeNode result)
        {
            Stack<TreeNode> tempStack = new Stack<TreeNode>();
            while (containerStack.Count > 0)
            {
                TreeNode node = containerStack.Pop();
                creator.SetUses(result, node);
                tempStack.Push(node);
            }
            containerStack = new Stack<TreeNode>(tempStack);

            creator.SetUses(result, result.Children[0]);
            containerStack.Push(result.Children[0]);
        }

        public override void Assign(TreeNode result)
        {
            while (assignStack.Count > 0)
            {
                TreeNode node = assignStack.Pop();
                creator.SetUses(result, node);
                containerStack.Push(node);
            }
        }

        public override void Expr(TreeNode result)
        {
            assignStack.Push(result.Children[0]);

            if (result.Children[1].Type == EntityType.Variable)
            {
                assignStack.Push(result.Children[1]);
            }
        }
    }
}