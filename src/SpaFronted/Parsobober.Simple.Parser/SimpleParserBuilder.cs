using Microsoft.Extensions.Logging;
using Parsobober.Pkb.Ast.Abstractions;
using Parsobober.Simple.Lexer;

namespace Parsobober.Simple.Parser;

internal class SimpleParserBuilder(
    ILogger<SimpleParserBuilder> logger, 
    IAst ast, 
    ILogger<SimpleParser> parserLogger,
    SlyLexerAdapter lexer
) : IParserBuilder
{
    public ISimpleParser BuildParser(string programCode)
    {
        logger.LogInformation("Creating simple parser");
        var tokens = lexer.Tokenize(programCode);
        logger.LogInformation("Parsing completed successfully");
        return new SimpleParser(tokens.GetEnumerator(), ast, parserLogger);
    }
}