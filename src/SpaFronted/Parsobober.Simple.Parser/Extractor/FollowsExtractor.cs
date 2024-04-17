using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Creators;
using Parsobober.Simple.Parser.Abstractions;

namespace Parsobober.Simple.Parser.Extractor
{
    internal class FollowsExtractor : SimpleExtractor
    {
        private readonly IFollowsCreator creator;

        public FollowsExtractor(ISimpleExtractor wrappee, IFollowsCreator creator) : base(wrappee)
        {
            this.creator = creator;
        }

        public override TreeNode While()
        {
            var result = wrappee.While();
            ContainerStmt(result, result.Children[1]);
            return result;
        }

        public override TreeNode Procedure()
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