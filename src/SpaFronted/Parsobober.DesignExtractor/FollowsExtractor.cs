using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Creators;

namespace Parsobober.Simple.Parser.Extractor;

internal class FollowsExtractor(IFollowsCreator creator) : SimpleExtractor
{
    public override void While(TreeNode result)
    {
        ContainerStmt(result, result.Children[1]);
    }

    public override void Procedure(TreeNode result)
    {
        ContainerStmt(result, result.Children[0]);
    }

    private void ContainerStmt(TreeNode container, TreeNode stmtList)
    {
        var stmtListElem = stmtList.Children[0];
        if (stmtListElem is null)
        {
            return;
        }

        var nextStmtListElem = stmtListElem.RightSibling;
        while (nextStmtListElem is not null)
        {
            creator.SetFollows(stmtListElem, nextStmtListElem);
            stmtListElem = nextStmtListElem;
            nextStmtListElem = stmtListElem.RightSibling;
        }
    }
}