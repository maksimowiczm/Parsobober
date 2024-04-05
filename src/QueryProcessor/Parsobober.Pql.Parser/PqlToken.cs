using sly.lexer;

namespace Parsobober.Pql.Parser;

internal static class PqlAuxiliaryTokens
{
    public const string Integer = @"\d+";
    public const string Ident = @"[a-zA-Z][a-zA-Z\d#]*";
    public const string Synonym = Ident;
    public const string DesignEntity = "(stmt|assign|while|variable|constant|prog_line)";
    public const string Name = @"[a-zA-Z][a-zA-Z\d]*";
}

internal enum PqlToken
{
    #region Separators

    [Lexeme(@"\(")]
    LeftParenthesis,

    [Lexeme(@"\)")]
    RightParenthesis,

    [Lexeme(",")]
    Coma,

    [Lexeme(";")]
    SemiColon,

    #endregion

    [Lexeme("=")]
    Equal,

    #region Keywords

    [Lexeme("Select")]
    Select,

    [Lexeme("such that")]
    SuchThat,

    [Lexeme("with")]
    With,

    #region Relations

    // Each relation should be a separate token
    [Lexeme("Parent")]
    Parent,

    [Lexeme("Modifies")]
    Modifies,

    // todo rest of the relations

    #endregion

    #endregion

    [Lexeme($@"{PqlAuxiliaryTokens.Ident}\.{PqlAuxiliaryTokens.Name}")]
    Attribute,

    [Lexeme($"\"{PqlAuxiliaryTokens.Ident}\"")]
    Ref,

    [Lexeme($"{PqlAuxiliaryTokens.DesignEntity} {PqlAuxiliaryTokens.Synonym}(, {PqlAuxiliaryTokens.Synonym})*;")]
    Declaration,

    // Segregate Design entities?
    [Lexeme(PqlAuxiliaryTokens.DesignEntity)]
    DesignEntity,

    // How to differentiate between Reference and EntReference?
    [Lexeme($"{PqlAuxiliaryTokens.Synonym}|_|{PqlAuxiliaryTokens.Integer}")]
    Reference,

    [Lexeme($"{PqlAuxiliaryTokens.Synonym}|_|\"{PqlAuxiliaryTokens.Ident}\"")]
    EntReference,

    [Lexeme(@"[ \t]+", true)]
    WhiteSpace,

    [Lexeme(@"[\n\r]+", true, true)]
    EndOfLine,
}