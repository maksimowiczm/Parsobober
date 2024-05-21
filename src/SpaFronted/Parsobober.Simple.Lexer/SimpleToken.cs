using sly.lexer;

namespace Parsobober.Simple.Lexer;

public enum SimpleToken
{
    #region KEYWORDS
    [Lexeme("procedure")] Procedure,
    [Lexeme("call")] Call,
    [Lexeme("while")] While,
    [Lexeme("if")] If,
    [Lexeme("then")] Then,
    [Lexeme("else")] Else,
    #endregion

    #region OPERATORS
    [Lexeme("=")] Equal,
    [Lexeme("-")] Minus,
    [Lexeme(@"\+")] Plus,
    [Lexeme(@"\*")] Multiply,
    #endregion

    #region SEPRATORS
    [Lexeme("{")] LeftCurly,
    [Lexeme("}")] RightCurly,
    [Lexeme(@"\(")] LeftParenthesis,
    [Lexeme(@"\)")] RightParenthesis,
    [Lexeme(";")] Semicolon,
    #endregion

    [Lexeme("[0-9]+")] Integer,
    [Lexeme(@"([a-zA-Z])([a-zA-Z\d])*")] Name,

    [Lexeme(@"[ \t]+", true)] WhiteSpace,
    [Lexeme(@"[\n\r]+", true, true)] EndOfLine,
}