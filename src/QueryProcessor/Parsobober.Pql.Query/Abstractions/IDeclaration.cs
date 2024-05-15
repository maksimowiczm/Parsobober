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
        // todo really ugly, refactor (clueless)
        List<Func<string, IDeclaration?>> parsers = [IStatementDeclaration.Parse, IVariableDeclaration.Parse];

        foreach (var parser in parsers)
        {
            var parsedDeclaration = parser(type);
            if (parsedDeclaration is not null)
            {
                return parsedDeclaration;
            }
        }

        throw new ArgumentOutOfRangeException($"Declaration type '{type}' is not supported.");
    }
}