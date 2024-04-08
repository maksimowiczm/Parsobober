using Microsoft.Extensions.Logging;
using Parsobober.Pkb.Ast.AstNodes;
using Parsobober.Simple.Lexer;

namespace Parsobober.Simple.Parser;

internal class SimpleParser(IEnumerator<LexicalToken> tokens, ILogger<SimpleParser> logger) : ISimpleParser
{
    private IAst _ast = new Ast();
    private LexicalToken _currentToken = tokens.Current;

    public IAst Parse()
    {
        logger.LogInformation("Starting parsing");
        GetToken();
        TreeNode procedureNode = Procedure();
        logger.LogInformation("Parsing completed successfully");
        return _ast;
    }
    private void GetToken()
    {
        if (!tokens.MoveNext()){
            _currentToken = new LexicalToken("EOF", SimpleToken.WhiteSpace, 0);
            return;
        };

        _currentToken = tokens.Current;
    }

    private void Match(string value, SimpleToken type)
    {
        if (_currentToken.Type == type && _currentToken.Value == value)
        {
            GetToken();
            return;
        }
        throw new ParseException(_currentToken, value, type);
    }
    private void Match(SimpleToken type)
    {
        if (_currentToken.Type == type)
        {
            GetToken();
            return;
        }
        throw new ParseException(_currentToken, type);
    }

    private TreeNode CreateTNode(EntityType type, int lineNumber, string? attr = null)
    {
        TreeNode node = _ast.CreateTNode(lineNumber, type);
        if (attr is not null)
        {
            _ast.SetAttr(node, attr);
        }

        return node;
    }

    private void AddNthChild(TreeNode parent, TreeNode child, int n)
    {
        int number = _ast.SetParenthood(parent, child);
        if (number != n)
        {
            throw new Exception("Parser internal error - bad child position");
        }

    }

    private TreeNode Procedure()
    {
        Match("procedure", SimpleToken.Keyword);
        var procedureName = _currentToken.Value;
        var procedureLine = _currentToken.LineNumber;
        Match(SimpleToken.Name);
        var procedureNode = CreateTNode(EntityType.Procedure, procedureLine, procedureName);

        Match("{", SimpleToken.Separator);
        TreeNode stmtNode = StmtLst();
        Match("}", SimpleToken.Separator);
        AddNthChild(procedureNode, stmtNode, 1);
        return procedureNode;
    }

    private TreeNode StmtLst()
    {
        TreeNode node = Stmt();
        if (_currentToken.Value == "}")
        {
            return node;
        }

        TreeNode nextNode = StmtLst();
        _ast.SetSibling(node, nextNode);
        return node;
    }

    private TreeNode Stmt()
    {
        if (_currentToken.Value == "while")
        {
            return While();
        }
        return Assign();
    }

    private TreeNode While()
    {
        var whileLine = _currentToken.LineNumber;
        Match("while", SimpleToken.Keyword);
        var controlVarName = _currentToken.Value;
        var varLine = _currentToken.LineNumber;
        Match(SimpleToken.Name);
        Match("{", SimpleToken.Separator);
        TreeNode stmtNode = StmtLst();
        Match("}", SimpleToken.Separator);

        var whileNode = CreateTNode(EntityType.While, whileLine);
        var variableNode = CreateTNode(EntityType.Variable, varLine, controlVarName);
        AddNthChild(whileNode, variableNode, 1);
        AddNthChild(whileNode, stmtNode, 2);
        return whileNode;
    }

    private TreeNode Assign()
    {
        var varLine = _currentToken.LineNumber;
        var leftVarName = _currentToken.Value;
        Match(SimpleToken.Name);
        Match("=", SimpleToken.Operator);
        TreeNode exprNode = Expr();
        Match(";", SimpleToken.Separator);

        var assignNode = CreateTNode(EntityType.Assign, varLine);
        var varNode = CreateTNode(EntityType.Variable, varLine, leftVarName);
        AddNthChild(assignNode, varNode, 1);
        AddNthChild(assignNode, exprNode, 2);
        return assignNode;
    }

    private TreeNode Expr()
    {
        TreeNode factorNode = Factor();
        if (_currentToken.Value == ";")
        {
            return factorNode;
        }
        var exprLine = _currentToken.LineNumber;
        Match("+", SimpleToken.Operator);
        TreeNode exprNode = Expr();

        var mainExprNode = CreateTNode(EntityType.Expr, exprLine, "+");
        AddNthChild(mainExprNode, factorNode, 1);
        AddNthChild(mainExprNode, exprNode, 2);
        return mainExprNode;
    }

    private TreeNode Factor()
    {
        var factorLine = _currentToken.LineNumber;
        var factorValue = _currentToken.Value;
        EntityType factorType;
        if (_currentToken.Type == SimpleToken.Integer)
        {
            Match(SimpleToken.Integer);
            factorType = EntityType.Constant;
        }
        else
        {
            Match(SimpleToken.Name);
            factorType = EntityType.Variable;
        }
        return CreateTNode(factorType, factorLine, factorValue);
    }
}