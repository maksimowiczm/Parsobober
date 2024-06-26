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

        while (_currentToken.Type == SimpleToken.Procedure)
        {
            var nextProcedureNode = Procedure();
            IAst.SetSiblings(procedureNode, nextProcedureNode);
        }

        Match(SimpleToken.WhiteSpace);
        IAst.SetParenthood(ast.Root, procedureNode);
        NotifyAll(ex => ex.FinishedParsing());
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
        TreeNode node = CreateTreeNode(EntityType.StatementsList, _currentLineNumber);
        NotifyAll(ex => ex.StmtLst());
        var firstNode = StmtLstElement(node);
        node.LineNumber = firstNode.LineNumber;
        NotifyAll(ex => ex.StmtLst(node));
        return node;
    }

    private TreeNode StmtLstElement(TreeNode list)
    {
        TreeNode node = Stmt();
        IAst.SetParenthood(list, node);
        if (_currentToken.Type == SimpleToken.RightCurly)
        {
            return node;
        }

        TreeNode nextNode = StmtLstElement(list);
        IAst.SetSiblings(node, nextNode);
        return node;
    }

    private TreeNode Stmt()
    {
        TreeNode stmtNode;
        if (_currentToken.Type == SimpleToken.While)
        {
            stmtNode = While();
        }
        else if (_currentToken.Type == SimpleToken.If)
        {
            stmtNode = If();
        }
        else if (_currentToken.Type == SimpleToken.Call)
        {
            stmtNode = Call();
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

    private TreeNode If()
    {
        var ifLine = GetNextLineNumber();

        Match(SimpleToken.If);
        var variableNode = Variable();

        Match(SimpleToken.Then);
        Match(SimpleToken.LeftCurly);
        TreeNode stmtList1Node = StmtLst();
        Match(SimpleToken.RightCurly);

        Match(SimpleToken.Else);
        Match(SimpleToken.LeftCurly);
        TreeNode stmtList2Node = StmtLst();
        Match(SimpleToken.RightCurly);

        var ifNode = CreateTreeNode(EntityType.If, ifLine);
        AddNthChild(ifNode, variableNode, 1);
        AddNthChild(ifNode, stmtList1Node, 2);
        AddNthChild(ifNode, stmtList2Node, 3);

        NotifyAll(ex => ex.If(ifNode));
        return ifNode;
    }

    private TreeNode Call()
    {
        var callLine = GetNextLineNumber();
        Match(SimpleToken.Call);
        var callProcedureName = _currentToken.Value;
        Match(SimpleToken.Name);
        Match(SimpleToken.Semicolon);

        var callNode = CreateTreeNode(EntityType.Call, callLine, callProcedureName);
        NotifyAll(ex => ex.Call(callNode));
        return callNode;
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
        TreeNode mainExprNode = Term();

        while (_currentToken.Type != SimpleToken.Semicolon && _currentToken.Type != SimpleToken.RightParenthesis)
        {
            mainExprNode = BasicOperator(mainExprNode);
        }

        NotifyAll(ex => ex.Expr(mainExprNode));
        return mainExprNode;
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
        var mainExprNode = CreateTreeNode(EntityType.Times, _currentLineNumber);
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
        TreeNode exprNode = Term();
        var mainExprNode = CreateTreeNode(entityType, _currentLineNumber);
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
            var constantNode = CreateTreeNode(EntityType.Constant, _currentLineNumber, constatValue);
            NotifyAll(ex => ex.Constant(constantNode));
            return constantNode;
        }
        else if (_currentToken.Type == SimpleToken.Name)
        {
            var variableNode = Variable();
            NotifyAll(ex => ex.Factor(variableNode));
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