using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Creators;

namespace Parsobober.Simple.Parser.Extractor
{
    internal class FollowsExtractor(IFollowsCreator creator) : SimpleExtractor
    {
        private readonly IFollowsCreator creator = creator;

        public override void While(TreeNode result)
        {
            ContainerStmt(result, result.Children[1]);
        }

        public override void Procedure(TreeNode result)
        {
            ContainerStmt(result, result.Children[0]);
        }
        private void ContainerStmt(TreeNode container, TreeNode? stmtListElem)
        {
            while (stmtListElem is not null)
            {
                creator.SetFollows(container, stmtListElem);
                stmtListElem = stmtListElem.RightSibling;
            }
        }
    }
}