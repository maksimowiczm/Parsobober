using Microsoft.Extensions.Logging;
using Parsobober.Simple.Lexer;

namespace Parsobober.Simple.Parser;

public class SimpleParser(ILogger<SimpleParser> logger, SlyLexerAdapter lexer)
{
    private readonly SlyLexerAdapter _lexer = lexer;
    private List<LexicalToken>? _tokens;
    private int _currentTokenId = 0;
    private LexicalToken _currentToken = new LexicalToken("END OF PROGRAM", SimpleToken.WhiteSpace);

    public void Parse(string program)
    {
        logger.LogInformation("Starting parsing");
        _tokens = new List<LexicalToken>(_lexer.Tokenize(program));
        Procedure();
        _tokens = null;
        logger.LogInformation("Parsing completed successfully");
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

    private void Procedure()
    {
        GetToken();
        Match(new LexicalToken("procedure", SimpleToken.Keyword));
        var procedureName = _currentToken.Value;
        Match(new LexicalToken("", SimpleToken.Name));
        Match(new LexicalToken("{", SimpleToken.Separator));
        StmtLst();
        Match(new LexicalToken("}", SimpleToken.Separator));
    }

    private void StmtLst()
    {
        Stmt();
        if (_currentToken.Value == "}")
        {
            return;
        }
        StmtLst();

    }

    private void Stmt()
    {
        if (_currentToken.Value == "while")
        {
            While();
        }
        else
        {
            Assign();
        }
    }

    private void While()
    {
        Match(new LexicalToken("while", SimpleToken.Keyword));
        var controlVarName = _currentToken.Value;
        Match(new LexicalToken("", SimpleToken.Name));
        Match(new LexicalToken("{", SimpleToken.Separator));
        StmtLst();
        Match(new LexicalToken("}", SimpleToken.Separator));
    }

    private void Assign()
    {
        var leftVarName = _currentToken.Value;
        Match(new LexicalToken("", SimpleToken.Name));
        Match(new LexicalToken("=", SimpleToken.Operator));
        Expr();
        Match(new LexicalToken(";", SimpleToken.Separator));
    }

    private void Expr()
    {
        var varName = _currentToken.Value;
        Match(new LexicalToken("", SimpleToken.Name));
        if (_currentToken.Value == ";")
        {
            return;
        }
        Match(new LexicalToken("+", SimpleToken.Operator));
        Expr();
    }
}