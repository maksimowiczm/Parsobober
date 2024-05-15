namespace Parsobober.Pql.Query.Abstractions;

public interface IVariableDeclaration : IDeclaration
{
    new static IDeclaration? Parse(string type)
    {
        return type switch
        {
            "variable" => new Variable(),
            _ => null
        };
    }

    /// <summary>
    /// Represents a variable declaration in a PQL query.
    /// </summary>
    public readonly record struct Variable : IVariableDeclaration;
}