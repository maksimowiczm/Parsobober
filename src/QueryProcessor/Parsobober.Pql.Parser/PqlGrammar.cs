using Parsobober.Pql.Query;
using sly.lexer;
using sly.parser.generator;

namespace Parsobober.Pql.Parser;

internal class PqlGrammar
{
    /// <summary>
    /// Defines a production rule for the "Parent" expression in the PQL grammar.
    /// </summary>
    /// <param name="parent">The "Parent" token.</param>
    /// <param name="leftParenthesis">The "(" token.</param>
    /// <param name="reference1">The first "Reference" token.</param>
    /// <param name="coma">The "," token.</param>
    /// <param name="reference2">The second "Reference" token.</param>
    /// <param name="rightParenthesis">The ")" token.</param>
    /// <returns>A new instance of the Parent query.</returns>
    [Production("expression : Parent LeftParenthesis Reference Coma Reference RightParenthesis")]
    public IQuery ParentExpression(
        Token<PqlToken> parent,
        Token<PqlToken> leftParenthesis,
        Token<PqlToken> reference1,
        Token<PqlToken> coma,
        Token<PqlToken> reference2,
        Token<PqlToken> rightParenthesis
    )
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Defines a production rule for the "Modifies" expression in the PQL grammar.
    /// </summary>
    /// <param name="modifies">The "Modifies" token.</param>
    /// <param name="leftParenthesis">The "(" token.</param>
    /// <param name="reference1">The first "Reference" token.</param>
    /// <param name="coma">The "," token.</param>
    /// <param name="reference2">The second "Reference" token.</param>
    /// <param name="rightParenthesis">The ")" token.</param>
    /// <returns>A new instance of the Modifies query.</returns>
    [Production("expression : Modifies LeftParenthesis Reference Coma Reference RightParenthesis")]
    public IQuery ModifiesExpression(
        Token<PqlToken> modifies,
        Token<PqlToken> leftParenthesis,
        Token<PqlToken> reference1,
        Token<PqlToken> coma,
        Token<PqlToken> reference2,
        Token<PqlToken> rightParenthesis
    )
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Defines a production rule for the "With" clause in the PQL grammar.
    /// </summary>
    /// <param name="with">The "With" token.</param>
    /// <param name="attribute">The "Attribute" token.</param>
    /// <param name="equal">The "Equal" token.</param>
    /// <param name="reference">The "Reference" token.</param>
    /// <returns>A new instance of the With query.</returns>
    [Production("with-clause : With Attribute Equal Ref")]
    public IQuery WithClause(
        Token<PqlToken> with,
        Token<PqlToken> attribute,
        Token<PqlToken> equal,
        Token<PqlToken> reference
    )
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Defines a production rule for the "SuchThat" clause in the PQL grammar.
    /// </summary>
    /// <param name="suchThat">The "SuchThat" token.</param>
    /// <param name="expression">The "expression" query.</param>
    /// <returns>The passed in expression query.</returns>
    [Production("such-that-clause : SuchThat expression")]
    public IQuery SuchThatClause(Token<PqlToken> suchThat, IQuery expression) => expression;

    /// <summary>
    /// Defines a production rule for the "Select" clause in the PQL grammar.
    /// </summary>
    /// <param name="declaration">The list of "Declaration" tokens.</param>
    /// <param name="select">The "Select" token.</param>
    /// <param name="reference">The "Reference" token.</param>
    /// <param name="suchThatClause">The list of "SuchThat" queries.</param>
    /// <param name="withClause">The list of "With" queries.</param>
    /// <returns>A new instance of the QueryWrapper query.</returns>
    [Production("select-clause : Declaration* Select Reference such-that-clause* with-clause*")]
    public IQuery SelectClause(
        List<Token<PqlToken>> declaration,
        Token<PqlToken> select,
        Token<PqlToken> reference,
        List<IQuery> suchThatClause,
        List<IQuery> withClause
    )
    {
        throw new NotImplementedException();
    }
}