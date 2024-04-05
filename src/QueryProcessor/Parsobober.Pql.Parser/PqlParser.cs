using Parsobober.Pql.Query;
using sly.parser.generator;

namespace Parsobober.Pql.Parser;

/// <summary>
/// The PqlParser class is responsible for parsing PQL queries.
/// </summary>
public class PqlParser
{
    private readonly sly.parser.Parser<PqlToken, IQuery> _parser = BuildParser();

    /// <summary>
    /// Parses a PQL query from a string.
    /// </summary>
    /// <param name="input">The string containing the PQL query to parse.</param>
    /// <returns>An IQuery object representing the parsed PQL query.</returns>
    public IQuery Parse(string input)
    {
        // Parses the input string into a PQL query.
        var queryResult = _parser.Parse(input);
        var query = queryResult.Result;

        return query;
    }

    private static sly.parser.Parser<PqlToken, IQuery> BuildParser()
    {
        var grammar = new PqlGrammar();

        var builder = new ParserBuilder<PqlToken, IQuery>();
        var parserResult = builder.BuildParser(grammar, ParserType.EBNF_LL_RECURSIVE_DESCENT, "select-clause");

        return parserResult.Result;
    }
}