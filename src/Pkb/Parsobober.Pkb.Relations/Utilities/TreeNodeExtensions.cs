using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Dto;


namespace Parsobober.Pkb.Relations.Utilities;

public static class TreeNodeExtensions
{
    public static Statement ToStatement(this TreeNode treeNode)
    {
        return treeNode.Type switch
        {
            EntityType.Assign => new Assign(treeNode.LineNumber),
            EntityType.If => new If(treeNode.LineNumber),
            EntityType.While => new While(treeNode.LineNumber),
            EntityType.Call => new Call(treeNode.LineNumber),
            _ => throw new NotSupportedException()
        };
    }

    public static bool IsType<T>(this TreeNode treeNode)
    {
        return true switch
        {
            true when typeof(T) == typeof(Assign) => treeNode.Type == EntityType.Assign,
            true when typeof(T) == typeof(While) => treeNode.Type == EntityType.While,
            true when typeof(T) == typeof(If) => treeNode.Type == EntityType.If,
            true when typeof(T) == typeof(Call) => treeNode.Type == EntityType.Call,
            true when typeof(T) == typeof(Statement) => treeNode.Type.IsStatement(),
            _ => throw new NotSupportedException(),
        };
    }

    public static Variable ToVariable(this TreeNode treeNode)
    {
        return treeNode.Type switch
        {
            EntityType.Variable => new Variable(treeNode.Attribute!),
            _ => throw new NotSupportedException()
        };
    }
}