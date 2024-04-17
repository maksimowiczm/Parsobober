using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Creators;
using Parsobober.Simple.Parser.Abstractions;

namespace Parsobober.Simple.Parser.Extractor
{
    internal class ProgramContextExtractor : SimpleExtractor
    {
        private readonly IProgramContextCreator creator;

        public ProgramContextExtractor(ISimpleExtractor wrappee, IProgramContextCreator creator) : base(wrappee)
        {
            this.creator = creator;
        }

        public override TreeNode Stmt()
        {
            var result = wrappee.Stmt();
            creator.TryAddStatement(result);
            return result;
        }

        public override TreeNode Procedure()
        {
            var result = wrappee.Procedure();
            creator.TryAddProcedure(result);
            return result;
        }

        public override TreeNode Variable()
        {
            var result = wrappee.Variable();
            creator.TryAddVariable(result);
            return result;
        }
    }
}
