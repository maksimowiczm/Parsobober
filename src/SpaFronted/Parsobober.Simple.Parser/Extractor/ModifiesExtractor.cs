using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Creators;
using Parsobober.Simple.Parser.Abstractions;

namespace Parsobober.Simple.Parser.Extractor
{
    internal class ModifiesExtractor : SimpleExtractor
    {
        private readonly IModifiesCreator creator;

        public ModifiesExtractor(ISimpleExtractor wrappee, IModifiesCreator creator) : base(wrappee)
        {
            this.creator = creator;
        }

        public override TreeNode Assign()
        {
            var result = wrappee.Assign();
            creator.SetModifies(result, result.Children[0]);
            return result;
        }
    }
}
