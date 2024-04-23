using Microsoft.Extensions.Logging;
using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Ast.Abstractions;
using Parsobober.Simple.Lexer;
using Parsobober.Simple.Parser.Abstractions;

namespace Parsobober.Simple.Parser;

internal class SimpleParser(
    IEnumerator<LexicalToken> tokens,
    IAst ast,
    ILogger<SimpleParser> logger,
    IReadOnlyList<ISimpleExtractor> extractors
) : ISimpleParser
{
    private LexicalToken _currentToken = tokens.Current;

    public IAst Parse()
    {
        logger.LogInformation("Starting parsing");
        GetToken();
        TreeNode procedureNode = Procedure();
        ast.SetParenthood(ast.Root, procedureNode);
        logger.LogInformation("Parsing completed successfully");
        return ast;
    }

    private void GetToken()
    {
        if (!tokens.MoveNext())
        {
            _currentToken = new LexicalToken("EOF", SimpleToken.WhiteSpace);
            return;
        }

        _currentToken = tokens.Current;
    }

    private void Match(string value, SimpleToken type)
    {
        if (_currentToken.Type != type || _currentToken.Value != value)
        {
            throw new ParseException(_currentToken, type);
        }
        GetToken();
    }

    private void Match(SimpleToken type)
    {
        if (_currentToken.Type != type)
        {
            throw new ParseException(_currentToken, type);
        }
        GetToken();
    }

    private TreeNode CreateTreeNode(EntityType type, int lineNumber, string? attr = null)
    {
        TreeNode node = ast.CreateTreeNode(lineNumber, type);
        if (attr is not null)
        {
            ast.SetAttribute(node, attr);
        }

        return node;
    }

    private void AddNthChild(TreeNode parent, TreeNode child, int n)
    {
        int number = ast.SetParenthood(parent, child);
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
        var procedureNode = CreateTreeNode(EntityType.Procedure, procedureLine, procedureName);

        Match("{", SimpleToken.Separator);
        NotifyAll(ex => ex.StmtLst());
        TreeNode stmtNode = StmtLst();
        Match("}", SimpleToken.Separator);
        AddNthChild(procedureNode, stmtNode, 1);

        NotifyAll(ex => ex.Procedure(procedureNode));
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
        ast.SetSiblings(node, nextNode);
        return node;
    }

    private TreeNode Stmt()
    {
        TreeNode stmtNode;
        if (_currentToken.Value == "while")
        {
            stmtNode = While();
        }
        else
        {
            stmtNode = Assign();
        }

        NotifyAll(ex => ex.Stmt(stmtNode));
        return stmtNode;
    }

    private TreeNode While()
    {
        var whileLine = _currentToken.LineNumber;

        Match("while", SimpleToken.Keyword);
        var variableNode = Variable();

        Match("{", SimpleToken.Separator);
        NotifyAll(ex => ex.StmtLst());
        TreeNode stmtNode = StmtLst();
        Match("}", SimpleToken.Separator);

        var whileNode = CreateTreeNode(EntityType.While, whileLine);
        AddNthChild(whileNode, variableNode, 1);
        AddNthChild(whileNode, stmtNode, 2);

        NotifyAll(ex => ex.While(whileNode));
        return whileNode;
    }

    private TreeNode Assign()
    {
        var line = _currentToken.LineNumber;

        var varNode = Variable();
        Match("=", SimpleToken.Operator);
        TreeNode exprNode = Expr();
        Match(";", SimpleToken.Separator);

        var assignNode = CreateTreeNode(EntityType.Assign, line);
        AddNthChild(assignNode, varNode, 1);
        AddNthChild(assignNode, exprNode, 2);

        NotifyAll(ex => ex.Assign(assignNode));
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

        var mainExprNode = CreateTreeNode(EntityType.Plus, exprLine, "+");
        AddNthChild(mainExprNode, factorNode, 1);
        AddNthChild(mainExprNode, exprNode, 2);

        NotifyAll(ex => ex.Expr(mainExprNode));
        return mainExprNode;
    }

    private TreeNode Factor()
    {
        if (_currentToken.Type == SimpleToken.Integer)
        {
            var factorLine = _currentToken.LineNumber;
            var factorValue = _currentToken.Value;

            Match(SimpleToken.Integer);
            return CreateTreeNode(EntityType.Constant, factorLine, factorValue);
        }
        else
        {
            var variableNode = Variable();
            NotifyAll(ex => ex.Factor(variableNode));
            return variableNode;
        }
    }

    private TreeNode Variable()
    {
        var name = _currentToken.Value;
        var line = _currentToken.LineNumber;
        Match(SimpleToken.Name);

        var variableNode = CreateTreeNode(EntityType.Variable, line, name);
        NotifyAll(ex => ex.Variable(variableNode));
        return variableNode;
    }

    private void NotifyAll(Action<ISimpleExtractor> method)
    {
        foreach (var ex in extractors)
        {
            method(ex);
        }
    }
}