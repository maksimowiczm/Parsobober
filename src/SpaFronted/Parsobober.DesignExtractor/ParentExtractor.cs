using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Creators;

namespace Parsobober.Simple.Parser.Extractor;

internal class ParentExtractor(IParentCreator creator) : SimpleExtractor
{
    public override void While(TreeNode result)
    {
        ContainerStmt(result, result.Children[1]);
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