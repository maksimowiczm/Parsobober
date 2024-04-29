namespace Parsobober.Pql.Query.Abstractions;

public interface IStatementDeclaration : IDeclaration
{
    new static IDeclaration? Parse(string type)
    {
        return type switch
        {
            "stmt" => new Statement(),
            "while" => new While(),
            "assign" => new Assign(),
            _ => null
        };
    }

    /// <summary>
    /// Represents a statement declaration in a PQL query.
    /// </summary>
    public readonly record struct Statement : IStatementDeclaration;

    /// <summary>
    /// Represents a while declaration in a PQL query.
    /// </summary>
    public readonly record struct While : IStatementDeclaration;

    /// <summary>
    /// Represents an assign declaration in a PQL query.
    /// </summary>
    public readonly record struct Assign : IStatementDeclaration;
}