using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Arguments.Exceptions;

namespace Parsobober.Pql.Query.Arguments;

/// <summary>
/// Represents a declaration in a PQL query.
/// </summary>
public interface IDeclaration : IArgument
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
            IProcedureDeclaration.Parse,
            IOtherDeclaration.Parse
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

    public IEnumerable<IPkbDto> ExtractFromContext(
        IDtoProgramContextAccessor context
    ) => this switch
    {
        IOtherDeclaration.ProgramLine => context.ProgramLines,
        IStatementDeclaration.Statement => context.Statements,
        IStatementDeclaration.Assign => context.Assigns,
        IStatementDeclaration.While => context.Whiles,
        IStatementDeclaration.If => context.Ifs,
        IStatementDeclaration.Call => context.Calls,
        IVariableDeclaration.Variable => context.Variables,
        IProcedureDeclaration.Procedure => context.Procedures,
        IOtherDeclaration.StatementList => context.StatementLists,
        IOtherDeclaration.Constant => context.Constants,
        _ => throw new ArgumentOutOfRangeException(GetType().Name)
    };
}