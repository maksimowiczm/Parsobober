using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Creators;
using Parsobober.Simple.Parser.Abstractions;

namespace Parsobober.Simple.Parser
{
    internal class FollowsExtractor(ISimpleExtractor wrappee, IFollowsCreator creator) : SimpleExtractor(wrappee)
    {
        override
        public TreeNode While()
        {
            var result = wrappee.While();
            ContainerStmt(result, result.Children[1]);
            return result;
        }

        override
        public TreeNode Procedure()
        {
            var result = wrappee.Procedure();
            ContainerStmt(result, result.Children[0]);
            return result;
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