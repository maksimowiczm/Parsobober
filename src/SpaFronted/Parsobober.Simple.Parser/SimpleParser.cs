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
    private int _currentLineNumber = 0;

    public IAst Parse()
    {
        logger.LogInformation("Starting parsing");
        GetNextToken();
        TreeNode procedureNode = Procedure();
        ast.SetParenthood(ast.Root, procedureNode);
        logger.LogInformation("Parsing completed successfully");
        return ast;
    }

    private int GetNextLineNumber()
    {
        _currentLineNumber++;
        return _currentLineNumber;
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
        Match(SimpleToken.Procedure);
        var procedureName = _currentToken.Value;
        Match(SimpleToken.Name);
        var procedureNode = CreateTreeNode(EntityType.Procedure, _currentLineNumber, procedureName);

        Match(SimpleToken.LeftCurly);
        TreeNode stmtListNode = StmtLst();
        Match(SimpleToken.RightCurly);
        AddNthChild(procedureNode, stmtListNode, 1);

        NotifyAll(ex => ex.Procedure(procedureNode));
        return procedureNode;
    }

    private TreeNode StmtLst()
    {
        NotifyAll(ex => ex.StmtLst());
        TreeNode node = CreateTreeNode(EntityType.StatementsList, _currentLineNumber);
        StmtLstElement(node);
        return node;
    }

    private TreeNode StmtLstElement(TreeNode list)
    {
        TreeNode node = Stmt();
        ast.SetParenthood(list, node);
        if (_currentToken.Type == SimpleToken.RightCurly)
        {
            return node;
        }

        TreeNode nextNode = StmtLstElement(list);
        ast.SetSiblings(node, nextNode);
        return node;
    }

    private TreeNode Stmt()
    {
        TreeNode stmtNode;
        if (_currentToken.Type == SimpleToken.While)
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
        var whileLine = GetNextLineNumber();

        Match(SimpleToken.While);
        var variableNode = Variable();

        Match(SimpleToken.LeftCurly);
        TreeNode stmtListNode = StmtLst();
        Match(SimpleToken.RightCurly);

        var whileNode = CreateTreeNode(EntityType.While, whileLine);
        AddNthChild(whileNode, variableNode, 1);
        AddNthChild(whileNode, stmtListNode, 2);

        NotifyAll(ex => ex.While(whileNode));
        return whileNode;
    }

    private TreeNode Assign()
    {
        var line = GetNextLineNumber();

        var varNode = Variable();
        Match(SimpleToken.Equal);
        TreeNode exprNode = Expr();
        Match(SimpleToken.Semicolon);

        var assignNode = CreateTreeNode(EntityType.Assign, line);
        AddNthChild(assignNode, varNode, 1);
        AddNthChild(assignNode, exprNode, 2);

        NotifyAll(ex => ex.Assign(assignNode));
        return assignNode;
    }

    private TreeNode Expr()
    {
        TreeNode factorNode = Factor();
        if (_currentToken.Type == SimpleToken.Semicolon)
        {
            return factorNode;
        }
        Match(SimpleToken.Plus);
        TreeNode exprNode = Expr();

        var mainExprNode = CreateTreeNode(EntityType.Plus, _currentLineNumber, "+");
        AddNthChild(mainExprNode, factorNode, 1);
        AddNthChild(mainExprNode, exprNode, 2);

        NotifyAll(ex => ex.Expr(mainExprNode));
        return mainExprNode;
    }

    private TreeNode Factor()
    {
        if (_currentToken.Type == SimpleToken.Integer)
        {
            var factorValue = _currentToken.Value;

            Match(SimpleToken.Integer);
            return CreateTreeNode(EntityType.Constant, _currentLineNumber, factorValue);
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
        Match(SimpleToken.Name);

        var variableNode = CreateTreeNode(EntityType.Variable, _currentLineNumber, name);
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