using Parsobober.Pkb.Ast.Abstractions;

namespace Parsobober.Simple.Parser.Abstractions;

public interface ISimpleParser
{
    IAst Parse();
}