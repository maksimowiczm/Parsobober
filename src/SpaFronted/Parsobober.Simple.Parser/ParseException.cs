using Parsobober.Simple.Lexer;

namespace Parsobober.Simple.Parser
{
    public class ParseException : Exception
    {
        public ParseException(LexicalToken getToken, SimpleToken type) :
            base(string.Format("Parse exception: got {0}:{1}, expected {2}",
            getToken.Type, getToken.Value, type))
        {
        }

    }
}
