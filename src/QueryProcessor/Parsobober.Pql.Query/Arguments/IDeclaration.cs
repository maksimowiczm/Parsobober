using Parsobober.Pql.Query.Arguments.Exceptions;

namespace Parsobober.Pql.Query.Arguments;

/// <summary>
/// Represents a declaration in a PQL query.
/// </summary>
internal interface IDeclaration : IArgument
{
    string Name { get; }

    /// <summary>
    /// Parses a string type into an IDeclaration instance.
    /// </summary>
    /// <param name="type">The string type to parse.</param>
    /// <param name="name">The name of the declaration.</param>
    /// <returns>An IDeclaration instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the type cannot be parsed.</exception>
    static IDeclaration Parse(string type, string name)
    {
        // todo really ugly, refactor (clueless)
        List<Func<string, string, IDeclaration?>> parsers =
        [
            IStatementDeclaration.Parse,
            IVariableDeclaration.Parse,
            IProcedureDeclaration.Parse
        ];

        foreach (var parser in parsers)
        {
            var parsedDeclaration = parser(type, name);
            if (parsedDeclaration is not null)
            {
                return parsedDeclaration;
            }
        }

        throw new DeclarationParseException(type, name);
    }
}