namespace Parsobober.Pql.Query.Arguments;

/// <summary>
/// Represents an argument in a PQL query.
/// </summary>
internal interface IArgument
{
    /// <summary>
    /// Parses a string argument into an IArgument instance.
    /// </summary>
    /// <param name="declarations">A dictionary of declarations where the key is the declaration name.</param>
    /// <param name="argument">The string argument to parse.</param>
    /// <returns>An IArgument instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the argument cannot be parsed.</exception>
    static IArgument Parse(IReadOnlyDictionary<string, IDeclaration> declarations, string argument)
    {
        if (declarations.TryGetValue(argument, out var declaration))
        {
            return declaration;
        }

        if (int.TryParse(argument, out var line))
        {
            return new Line(line);
        }

        return new VarName(argument);
    }

    /// <summary>
    /// Represents a line in a PQL query.
    /// </summary>
    public readonly record struct Line(int Value) : IArgument;

    public readonly record struct VarName(string Value) : IArgument;
}