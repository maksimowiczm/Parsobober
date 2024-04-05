using Parsobober.Pql.Query;
using sly.lexer;
using sly.parser.generator;
using sly.parser.parser;

namespace Parsobober.Pql.Parser;

internal class PqlGrammar(IQueryBuilder queryBuilder)
{
    [Production("select-clause : Declaration* Select[d] Reference such-that-clause? with-clause?")]
    public IQueryBuilder SelectClause(
        List<Token<PqlToken>> declaration,
        Token<PqlToken> synonym,
        ValueOption<IQueryBuilder> _1, // discard
        ValueOption<IQueryBuilder> _2 // discard
    )
    {
        queryBuilder.AddSelect(synonym.Value);
        declaration.ForEach(d => queryBuilder.AddDeclaration(d.Value));
        return queryBuilder;
    }

    [Production("such-that-clause : SuchThat[d] relation")]
    public IQueryBuilder SuchThatClause(IQueryBuilder relation) => relation;

    [Production("with-clause : With[d] Attribute Equal[d] Ref")]
    public IQueryBuilder WithClause(Token<PqlToken> attribute, Token<PqlToken> reference) =>
        queryBuilder.With(attribute.Value, reference.Value);

    [Production("relation : Parent[d] LeftParenthesis[d] Reference Coma[d] Reference RightParenthesis[d]")]
    public IQueryBuilder ParentExpression(Token<PqlToken> parent, Token<PqlToken> child) =>
        queryBuilder.AddParent(parent.Value, child.Value);

    [Production("relation : ParentTransitive[d] LeftParenthesis[d] Reference Coma[d] Reference RightParenthesis[d]")]
    public IQueryBuilder ParentTransitiveExpression(Token<PqlToken> parent, Token<PqlToken> child) =>
        queryBuilder.AddParentTransitive(parent.Value, child.Value);

    [Production("relation : Modifies[d] LeftParenthesis[d] Reference Coma[d] Reference RightParenthesis[d]")]
    public IQueryBuilder ModifiesExpression(Token<PqlToken> reference1, Token<PqlToken> reference2) =>
        queryBuilder.AddModifies(reference1.Value, reference2.Value);

    // todo rest of the relations
}