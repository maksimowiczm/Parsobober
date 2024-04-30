using Parsobober.Pql.Query.Abstractions;
using sly.parser.generator;

namespace Parsobober.Pql.Parser;

/// <summary>
/// The PqlParser class is responsible for parsing PQL queries.
/// </summary>
public class PqlParser(IQueryBuilder queryBuilder)
{
    private readonly sly.parser.Parser<PqlToken, IQueryBuilder> _parser = BuildParser(queryBuilder);

    /// <summary>
    /// Parses a PQL query from a string.
    /// </summary>
    /// <param name="input">The string containing the PQL query to parse.</param>
    /// <returns>An IQuery object representing the parsed PQL query.</returns>
    public IQuery Parse(string input)
    {
        // Parses the input string into a PQL query.
        var queryResult = _parser.Parse(input);

        if (queryResult.IsError)
        {
            throw new PqlParserException(queryResult.Errors);
        }

        var query = queryResult.Result;
        return query.Build();
    }

    private static sly.parser.Parser<PqlToken, IQueryBuilder> BuildParser(IQueryBuilder queryBuilder)
    {
        var grammar = new PqlGrammar(queryBuilder);

        var builder = new ParserBuilder<PqlToken, IQueryBuilder>();
        var parserResult = builder.BuildParser(grammar, ParserType.EBNF_LL_RECURSIVE_DESCENT, "select-clause");

        if (parserResult.IsError)
        {
            throw new PqlParserException(parserResult.Errors);
        }

        return parserResult.Result;
    }
}