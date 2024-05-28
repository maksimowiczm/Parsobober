namespace Parsobober.Pkb.Ast;

public enum EntityType
{
    Program,
    Procedure,
    StatementsList,
    Statement,
    Assign,
    Call,
    While,
    If,
    Plus,
    Minus,
    Times,
    Variable,
    Constant,
}

public static class EntityTypeExtensions
{
    public static bool IsStatement(this EntityType entityType)
    {
        return entityType switch
        {
            EntityType.Statement
                or EntityType.Assign
                or EntityType.Call
                or EntityType.If
                or EntityType.While => true,
            _ => false,
        };
    }

    public static bool IsContainerStatement(this EntityType entityType)
    {
        return entityType switch
        {
            EntityType.If
                or EntityType.While => true,
            _ => false,
        };
    }

    public static bool IsVariable(this EntityType entityType) => entityType == EntityType.Variable;

    public static bool IsProcedure(this EntityType entityType) => entityType == EntityType.Procedure;
}