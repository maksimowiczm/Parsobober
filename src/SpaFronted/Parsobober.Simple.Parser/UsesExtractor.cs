using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Creators;
using Parsobober.Simple.Parser.Abstractions;

namespace Parsobober.Simple.Parser
{
    internal class UsesExtractor(ISimpleExtractor wrappee, IUsesCreator creator) : SimpleExtractor(wrappee)
    {
        override
        public TreeNode While()
        {
            var result = wrappee.While();
            creator.SetUses(result, result.Children[0]);
            return result;
        }
    }
}