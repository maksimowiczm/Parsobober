using Microsoft.Extensions.Logging;
using sly.lexer;

namespace Parsobober.Simple.Lexer;

public class SlyLexerAdapter(ILogger<SlyLexerAdapter> logger)
{
    private readonly ILexer<SimpleToken> _innerLexer = LexerBuilder.BuildLexer<SimpleToken>().Result;

    public IEnumerable<LexicalToken> Tokenize(string input)
    {
        logger.LogInformation("Starting tokenization");
        var tokens = _innerLexer.Tokenize(input).Tokens.ToList();
        logger.LogInformation("Tokenized {} tokens", tokens.Count);

        return tokens
            .Where(t => !string.IsNullOrEmpty(t.Value)) // somehow we need this
            .Select<Token<SimpleToken>, LexicalToken>(Extensions.ToSimpleToken);
    }
}