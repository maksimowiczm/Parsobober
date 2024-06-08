using Parsobober.Pql.Pattern.Parser.Abstractions;
using Parsobober.Simple.Lexer;

namespace Parsobober.Pql.Pattern.Parser;

public class PatternParserBuilder(
    SlyLexerAdapter lexer
) : IPatternParserBuilder
{
    public IPatternParser BuildParser(string pattern)
    {
        var tokens = lexer.Tokenize(pattern);
        return new PatternParser(tokens.GetEnumerator());
    }
}