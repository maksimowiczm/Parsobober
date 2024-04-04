using Parsobober.Simple.Lexer;

namespace Parsobober.Simple.Parser;

public class SimpleParser(SlyLexerAdapter lexer)
{
    private SlyLexerAdapter _lexer = lexer;
}