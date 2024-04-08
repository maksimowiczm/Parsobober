using Parsobober.Pkb.Ast.AstNodes;

namespace Parsobober.Simple.Parser;

public interface ISimpleParser
{
    IAst Parse(string program);
}