using sly.buildresult;
using sly.parser;

namespace Parsobober.Pql.Parser;

internal class PqlParserException : Exception
{
    public PqlParserException(IEnumerable<InitializationError> parserResultErrors)
        : base(string.Join('\n', parserResultErrors.Select(e => e.Message)))
    {
    }

    public PqlParserException(IEnumerable<ParseError> parserResultErrors)
        : base(string.Join('\n', parserResultErrors.Select(e => e.ErrorMessage)))
    {
    }
}