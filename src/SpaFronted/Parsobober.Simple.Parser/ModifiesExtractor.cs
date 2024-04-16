using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Creators;
using Parsobober.Simple.Parser.Abstractions;

namespace Parsobober.Simple.Parser
{
    internal class ModifiesExtractor(ISimpleExtractor wrappee, IModifiesCreator creator) : SimpleExtractor(wrappee)
    {
        override
        public TreeNode Assign()
        {
            var result = wrappee.Assign();
            creator.SetModifies(result, result.Children[0]);
            return result;
        }
    }
}
