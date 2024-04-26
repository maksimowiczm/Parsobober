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

    [Production("with-clause : With[d] Attribute Equal[d] Reference")]
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

    [Production("relation : Follows[d] LeftParenthesis[d] Reference Coma[d] Reference RightParenthesis[d]")]
    public IQueryBuilder FollowsExpression(Token<PqlToken> reference1, Token<PqlToken> reference2) =>
         queryBuilder.AddFollows(reference1.Value, reference2.Value);

    [Production("relation : FollowsTransitive[d] LeftParenthesis[d] Reference Coma[d] Reference RightParenthesis[d]")]
    public IQueryBuilder FollowsTransitiveExpression(Token<PqlToken> reference1, Token<PqlToken> reference2) =>
         queryBuilder.AddFollowsTransitive(reference1.Value, reference2.Value);

    [Production("relation : Uses[d] LeftParenthesis[d] Reference Coma[d] Reference RightParenthesis[d]")]
    public IQueryBuilder UsesExpression(Token<PqlToken> reference1, Token<PqlToken> reference2) =>
         queryBuilder.AddUses(reference1.Value, reference2.Value);
}