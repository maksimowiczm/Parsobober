using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Creators;

namespace Parsobober.DesignExtractor.Extractors;

internal class ParentExtractor(IParentCreator creator) : SimpleExtractor
{
    public override void While(TreeNode result)
    {
        ContainerStmt(result, result.Children[1]);
    }

    public override void If(TreeNode result)
    {
        ContainerStmt(result, result.Children[1]);
        ContainerStmt(result, result.Children[2]);
    }

    private void ContainerStmt(TreeNode container, TreeNode stmtList)
    {
        var stmtListElem = stmtList.Children[0];
        while (stmtListElem is not null)
        {
            creator.SetParent(container, stmtListElem);
            stmtListElem = stmtListElem.RightSibling;
        }
    }
}