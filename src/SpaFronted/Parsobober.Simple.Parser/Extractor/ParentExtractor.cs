using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Creators;
using Parsobober.Simple.Parser.Abstractions;

namespace Parsobober.Simple.Parser.Extractor
{
    internal class ParentExtractor : SimpleExtractor
    {
        private readonly IParentCreator creator;

        public ParentExtractor(ISimpleExtractor wrappee, IParentCreator creator) : base(wrappee)
        {
            this.creator = creator;
        }

        public override TreeNode While()
        {
            var result = wrappee.While();
            ContainerStmt(result, result.Children[1]);
            return result;
        }
        private void ContainerStmt(TreeNode container, TreeNode? stmtListElem)
        {
            while (stmtListElem is not null)
            {
                creator.SetParent(container, stmtListElem);
                stmtListElem = stmtListElem.RightSibling;
            }
        }
    }

}