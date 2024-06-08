using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Ast.Abstractions;
using Parsobober.Pql.Pattern.Parser.Abstractions;
using Parsobober.Simple.Lexer;
using Parsobober.Simple.Parser;

namespace Parsobober.Pql.Pattern.Parser;

internal class PatternParser(
    IEnumerator<LexicalToken> tokens) : IPatternParser
{
    private LexicalToken _currentToken = tokens.Current;

    public TreeNode Parse()
    {
        GetNextToken();
        TreeNode exprNode = Expr();

        Match(SimpleToken.WhiteSpace);

        return exprNode;
    }

    private void GetNextToken()
    {
        if (!tokens.MoveNext())
        {
            _currentToken = new LexicalToken("EOF", SimpleToken.WhiteSpace);
            return;
        }

        _currentToken = tokens.Current;
    }

    private void Match(SimpleToken type)
    {
        if (_currentToken.Type != type)
        {
            throw new ParseException(_currentToken, type);
        }
        GetNextToken();
    }

    private TreeNode CreateTreeNode(EntityType type, int lineNumber, string? attr = null)
    {
        TreeNode node = IAst.CreateTreeNode(lineNumber, type);
        if (attr is not null)
        {
            IAst.SetAttribute(node, attr);
        }

        return node;
    }

    private void AddNthChild(TreeNode parent, TreeNode child, int n)
    {
        int number = IAst.SetParenthood(parent, child);
        if (number != n)
        {
            throw new Exception("Parser internal error - bad child position");
        }
    }

    private TreeNode Expr()
    {
        TreeNode termNode = Term();

        if (_currentToken.Type == SimpleToken.Plus || _currentToken.Type == SimpleToken.Minus)
        {
            return BasicOperator(termNode);
        }
        else
        {
            return termNode;
        }
    }

    private TreeNode Term()
    {
        TreeNode factorNode = Factor();
        if (_currentToken.Type != SimpleToken.Multiply)
        {
            return factorNode;
        }

        Match(SimpleToken.Multiply);

        TreeNode termNode = Term();
        var mainExprNode = CreateTreeNode(EntityType.Times, 0);
        AddNthChild(mainExprNode, factorNode, 1);
        AddNthChild(mainExprNode, termNode, 2);
        return mainExprNode;
    }

    private TreeNode BasicOperator(TreeNode termNode)
    {
        EntityType entityType;
        switch (_currentToken.Type)
        {
            case SimpleToken.Plus:
                Match(SimpleToken.Plus);
                entityType = EntityType.Plus;
                break;
            case SimpleToken.Minus:
                Match(SimpleToken.Minus);
                entityType = EntityType.Minus;
                break;
            default:
                throw new Exception("Operator not found");

        }
        TreeNode exprNode = Expr();
        var mainExprNode = CreateTreeNode(entityType, 0);
        AddNthChild(mainExprNode, termNode, 1);
        AddNthChild(mainExprNode, exprNode, 2);
        return mainExprNode;
    }

    private TreeNode Factor()
    {
        if (_currentToken.Type == SimpleToken.Integer)
        {
            var constatValue = _currentToken.Value;

            Match(SimpleToken.Integer);
            var constantNode = CreateTreeNode(EntityType.Constant, 0, constatValue);
            return constantNode;
        }
        else if (_currentToken.Type == SimpleToken.Name)
        {
            var variableNode = Variable();
            return variableNode;
        }
        else
        {
            Match(SimpleToken.LeftParenthesis);
            var exprNode = Expr();
            Match(SimpleToken.RightParenthesis);
            return exprNode;
        }
    }

    private TreeNode Variable()
    {
        var name = _currentToken.Value;
        Match(SimpleToken.Name);

        var variableNode = CreateTreeNode(EntityType.Variable, 0, name);
        return variableNode;
    }
}
