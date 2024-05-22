namespace Parsobober.Pql.Query.Arguments;

internal interface IStatementDeclaration : IDeclaration
{
    new static IDeclaration? Parse(string type, string name)
    {
        return type switch
        {
            "stmt" => new Statement(name),
            "while" => new While(name),
            "assign" => new Assign(name),
            _ => null
        };
    }

    /// <summary>
    /// Represents a statement declaration in a PQL query.
    /// </summary>
    public readonly record struct Statement(string Name) : IStatementDeclaration;

    /// <summary>
    /// Represents a while declaration in a PQL query.
    /// </summary>
    public readonly record struct While(string Name) : IStatementDeclaration;

    /// <summary>
    /// Represents an assign declaration in a PQL query.
    /// </summary>
    public readonly record struct Assign(string Name) : IStatementDeclaration;
}