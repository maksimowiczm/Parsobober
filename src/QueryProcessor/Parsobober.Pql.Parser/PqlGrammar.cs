using Parsobober.Pql.Query.Abstractions;
using sly.lexer;
using sly.parser.generator;
using sly.parser.parser;

namespace Parsobober.Pql.Parser;

internal class PqlGrammar(IQueryBuilder queryBuilder)
{
    [Production("select-clause : Declaration* Select[d] Reference condition-clause+")]
    public IQueryBuilder SelectClause(
        List<Token<PqlToken>> declaration, // declarations
        Token<PqlToken> synonym, // reference
        List<IQueryBuilder> _ // condition-clauses
    )
    {
        queryBuilder.AddSelect(synonym.Value);
        declaration.ForEach(d => queryBuilder.AddDeclaration(d.Value));
        return queryBuilder;
    }

    [Production("condition-clause : such-that-clause")]
    [Production("condition-clause : with-clause")]
    public IQueryBuilder ConditionClause(IQueryBuilder relation) => relation;

    [Production("such-that-clause : SuchThat[d] relation and-relation*")]
    public IQueryBuilder SuchThatClause(IQueryBuilder relation, List<IQueryBuilder> _) => relation;

    [Production("and-relation : And[d] relation")]
    public IQueryBuilder AndRelation(IQueryBuilder relation) => relation;

    #region With Clause

    [Production("with-clause : With[d] attribute-compare and-attribute-compare*")]
    public IQueryBuilder WithClause(IQueryBuilder compare, List<IQueryBuilder> _) => compare;

    [Production("and-attribute-compare : And[d] attribute-compare")]
    public IQueryBuilder AndAttributeCompare(IQueryBuilder compare) => compare;

    [Production("attribute-compare : Attribute Equal[d] Reference")]
    public IQueryBuilder AttributeCompare(Token<PqlToken> attribute, Token<PqlToken> reference) =>
        queryBuilder.With(attribute.Value, reference.Value);

    #endregion

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