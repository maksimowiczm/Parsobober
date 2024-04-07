using Microsoft.Extensions.Logging;
using Parsobober.Pkb.Ast.AstNodes;
using Parsobober.Simple.Lexer;

namespace Parsobober.Simple.Parser;

public class SimpleParser(ILogger<SimpleParser> logger, SlyLexerAdapter lexer)
{
    private readonly SlyLexerAdapter _lexer = lexer;
    private List<LexicalToken>? _tokens;
    private IAst _ast = new Ast();
    private int _currentTokenId = 0;
    private int _currentLineNumber = 1;
    private LexicalToken _currentToken = new LexicalToken("END OF PROGRAM", SimpleToken.WhiteSpace);

    public IAst Parse(string program)
    {
        logger.LogInformation("Starting parsing");
        _tokens = new List<LexicalToken>(_lexer.Tokenize(program));
        Procedure();
        _tokens = null;
        logger.LogInformation("Parsing completed successfully");
        return _ast;
    }
    private LexicalToken GetToken()
    {
        if (_tokens == null || _tokens.Count <= _currentTokenId)
        {
            return new LexicalToken("END-OF-PROGRAM", SimpleToken.WhiteSpace);
        }

        _currentToken = _tokens[_currentTokenId++];
        return _currentToken;
    }

    private void Match(LexicalToken token)
    {
        if ((_currentToken == token) ||
            (token.Value == "" && _currentToken.Type == token.Type))
        {
            _currentToken = GetToken();
            return;
        }
        throw new ParseException(_currentToken, token);
    }

    private TreeNode CreateTNode(EntityType type, String attr = null)
    {
        TreeNode node = _ast.CreateTNode(_currentLineNumber++, type);
        if (attr != null)
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
        GetToken();
        Match(new LexicalToken("procedure", SimpleToken.Keyword));
        var procedureName = _currentToken.Value;
        Match(new LexicalToken("", SimpleToken.Name));
        Match(new LexicalToken("{", SimpleToken.Separator));
        TreeNode stmtNode = StmtLst();
        Match(new LexicalToken("}", SimpleToken.Separator));
        
        var procedureNode = CreateTNode(EntityType.Procedure, procedureName);
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
        Match(new LexicalToken("while", SimpleToken.Keyword));
        var controlVarName = _currentToken.Value;
        Match(new LexicalToken("", SimpleToken.Name));
        Match(new LexicalToken("{", SimpleToken.Separator));
        TreeNode stmtNode = StmtLst();
        Match(new LexicalToken("}", SimpleToken.Separator));

        var whileNode = CreateTNode(EntityType.While);
        var variableNode = CreateTNode(EntityType.Variable, controlVarName);
        AddNthChild(whileNode, variableNode, 1);
        AddNthChild(whileNode, stmtNode, 2);
        return whileNode;
    }

    private TreeNode Assign()
    {
        var leftVarName = _currentToken.Value;
        Match(new LexicalToken("", SimpleToken.Name));
        Match(new LexicalToken("=", SimpleToken.Operator));
        TreeNode exprNode = Expr();
        Match(new LexicalToken(";", SimpleToken.Separator));

        var assignNode = CreateTNode(EntityType.Assign);
        var varNode = CreateTNode(EntityType.Variable, leftVarName);
        AddNthChild(assignNode, varNode , 1);
        AddNthChild(assignNode, exprNode , 2);
        return assignNode;
    }

    private TreeNode Expr()
    {
        var varName = _currentToken.Value;
        Match(new LexicalToken("", SimpleToken.Name));
        var varNode = CreateTNode(EntityType.Variable, varName);
        if (_currentToken.Value == ";")
        {
            return varNode;
        }
        Match(new LexicalToken("+", SimpleToken.Operator));
        TreeNode exprNode = Expr();
        var mainExprNode = CreateTNode(EntityType.Expr, "+");
        AddNthChild(mainExprNode, varNode, 1);
        AddNthChild(mainExprNode, exprNode, 2);
        return mainExprNode;
    }
}