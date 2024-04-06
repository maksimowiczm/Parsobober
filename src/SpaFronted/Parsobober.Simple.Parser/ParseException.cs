using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parsobober.Simple.Lexer;

namespace Parsobober.Simple.Parser
{
     public class ParseException(LexicalToken getToken, LexicalToken expectedToken) :
        Exception(String.Format("Parse exception: get {0}:{1}, expected {2}:{3}",
        getToken.Type, getToken.Value, expectedToken.Type, expectedToken.Value));
}
