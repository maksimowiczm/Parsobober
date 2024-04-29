namespace Parsobober.Pql.Query.Abstractions;

/// <summary>
/// Represents a declaration in a PQL query.
/// </summary>
public interface IDeclaration : IArgument
{
    /// <summary>
    /// Parses a string type into an IDeclaration instance.
    /// </summary>
    /// <param name="type">The string type to parse.</param>
    /// <returns>An IDeclaration instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the type cannot be parsed.</exception>
    static IDeclaration Parse(string type)
    {
        return type switch
        {
            "stmt" => new Statement(),
            "while" => new While(),
            "assign" => new Assign(),
            _ => throw new ArgumentOutOfRangeException($"Declaration type '{type}' is not supported.")
        };
    }


    [Obsolete("💀")]
    Type ToDtoStatementType()
    {
        return this switch
        {
            Statement => typeof(Pkb.Relations.Dto.Statement),
            While => typeof(Pkb.Relations.Dto.While),
            Assign => typeof(Pkb.Relations.Dto.Assign),
            _ => throw new InvalidOperationException()
        };
    }

    /// <summary>
    /// Represents a statement declaration in a PQL query.
    /// </summary>
    public readonly record struct Statement : IDeclaration;

    /// <summary>
    /// Represents a while declaration in a PQL query.
    /// </summary>
    public readonly record struct While : IDeclaration;

    /// <summary>
    /// Represents an assign declaration in a PQL query.
    /// </summary>
    public readonly record struct Assign : IDeclaration;
}