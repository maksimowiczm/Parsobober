using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Creators;

namespace Parsobober.DesignExtractor.Extractors;

internal class ProgramContextExtractor(IProgramContextCreator creator) : SimpleExtractor
{
    public override void Stmt(TreeNode result)
    {
        creator.TryAddStatement(result);
    }

    public override void Procedure(TreeNode result)
    {
        creator.TryAddProcedure(result);
    }

    public override void Variable(TreeNode result)
    {
        creator.TryAddVariable(result);
    }
}
