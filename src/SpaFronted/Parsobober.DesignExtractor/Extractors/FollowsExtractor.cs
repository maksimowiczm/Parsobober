using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Creators;

namespace Parsobober.DesignExtractor.Extractors;

internal class FollowsExtractor(IFollowsCreator creator) : SimpleExtractor
{
    public override void While(TreeNode result)
    {
        ContainerStmt(result.Children[1]);
    }

    public override void If(TreeNode result)
    {
        ContainerStmt(result.Children[1]);
        ContainerStmt(result.Children[2]);
    }

    public override void Procedure(TreeNode result)
    {
        ContainerStmt(result.Children[0]);
    }

    private void ContainerStmt(TreeNode stmtList)
    {
        var stmtListElem = stmtList.Children[0];

        var nextStmtListElem = stmtListElem.RightSibling;
        while (nextStmtListElem is not null)
        {
            creator.SetFollows(stmtListElem, nextStmtListElem);
            stmtListElem = nextStmtListElem;
            nextStmtListElem = stmtListElem.RightSibling;
        }
    }
}