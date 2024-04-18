using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Creators;

namespace Parsobober.Simple.Parser.Extractor
{
    internal class ModifiesExtractor(IModifiesCreator creator) : SimpleExtractor
    {
        private readonly IModifiesCreator creator = creator;

        public override void Assign(TreeNode result)
        {
            creator.SetModifies(result, result.Children[0]);
        }
    }
}
