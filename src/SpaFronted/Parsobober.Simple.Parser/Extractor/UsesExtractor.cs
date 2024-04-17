using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Creators;
using Parsobober.Simple.Parser.Abstractions;

namespace Parsobober.Simple.Parser.Extractor
{
    internal class UsesExtractor : SimpleExtractor
    {
        private readonly IUsesCreator creator;

        public UsesExtractor(ISimpleExtractor wrappee, IUsesCreator creator) : base(wrappee)
        {
            this.creator = creator;
        }

        public override TreeNode While()
        {
            var result = wrappee.While();
            creator.SetUses(result, result.Children[0]);
            return result;
        }
    }
}