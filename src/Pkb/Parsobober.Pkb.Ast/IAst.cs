namespace Parsobober.Pkb.Ast;

public interface IAst
{
    TreeNode CreateTreeNode(int lineNumber, EntityType type);

    void SetAttribute(TreeNode node, string attribute);

    void SetSibling(TreeNode left, TreeNode right);

    int SetParenthood(TreeNode parent, TreeNode child);

    TreeNode GetChildN(TreeNode node, int n);
}