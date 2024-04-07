namespace Parsobober.Simple.Lexer;

public record LexicalToken(string Value, SimpleToken Type, int LineNumber);