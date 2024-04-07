using Microsoft.Extensions.Logging;
using Parsobober.Pkb.Ast.AstNodes;
using Parsobober.Simple.Lexer;

namespace Parsobober.Simple.Parser;

public class SimpleParser(ILogger<SimpleParser> logger, SlyLexerAdapter lexer)
{
    private List<LexicalToken>? _tokens;
    private IAst _ast = new Ast();
    private int _currentTokenId = 0;
    private LexicalToken _currentToken = new("END-OF-PROGRAM", SimpleToken.WhiteSpace, 0);

    public IAst Parse(string program)
    {
        logger.LogInformation("Starting parsing");
        _tokens = lexer.Tokenize(program).ToList();
        Procedure();
        _tokens = null;
        logger.LogInformation("Parsing completed successfully");
        return _ast;
    }
    private LexicalToken GetToken()
    {
        if (_tokens is null || _tokens.Count <= _currentTokenId)
        {
            return new("END-OF-PROGRAM", SimpleToken.WhiteSpace, 0);
        }

        var token = _tokens[_currentTokenId++];
        return token;
    }

    private void Match(string value, SimpleToken type)
    {
        if (_currentToken.Type == type && _currentToken.Value == value)
        {
            _currentToken = GetToken();
            return;
        }
        throw new ParseException(_currentToken, value, type);
    }
    private void Match(SimpleToken type)
    {
        if (_currentToken.Type == type)
        {
            _currentToken = GetToken();
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

    private void Procedure()
    {
        _currentToken = GetToken();
        Match("procedure", SimpleToken.Keyword);
        var procedureName = _currentToken.Value;
        var procedureLine = _currentToken.LineNumber;
        Match(SimpleToken.Name);
        Match("{", SimpleToken.Separator);
        TreeNode stmtNode = StmtLst();
        Match("}", SimpleToken.Separator);

        var procedureNode = CreateTNode(EntityType.Procedure, procedureLine, procedureName);
        _ast.SetRoot(procedureNode);
        AddNthChild(procedureNode, stmtNode, 1);
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
        var varLine = _currentToken.LineNumber;
        var varName = _currentToken.Value;
        Match(SimpleToken.Name);
        var varNode = CreateTNode(EntityType.Variable, varLine, varName);
        if (_currentToken.Value == ";")
        {
            return varNode;
        }
        Match("+", SimpleToken.Operator);
        TreeNode exprNode = Expr();

        var mainExprNode = CreateTNode(EntityType.Expr, varLine, "+");
        AddNthChild(mainExprNode, varNode, 1);
        AddNthChild(mainExprNode, exprNode, 2);
        return mainExprNode;
    }
}