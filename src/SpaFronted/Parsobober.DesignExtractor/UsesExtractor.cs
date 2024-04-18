using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Creators;

namespace Parsobober.Simple.Parser.Extractor
{
    internal class UsesExtractor(IUsesCreator creator) : SimpleExtractor
    {
        private readonly IUsesCreator creator = creator;

        public override void While(TreeNode result)
        {
            creator.SetUses(result, result.Children[0]);
        }
    }
}