namespace Parsobober.Simple.Lexer;

internal static class Extensions
{
    public static LexicalToken ToSimpleToken(this sly.lexer.Token<SimpleToken> token) =>
        new(token.Value, token.TokenID);
}