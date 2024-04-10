using Parsobober.Pkb.Ast.Abstractions;

namespace Parsobober.Simple.Parser;

public interface ISimpleParser
{
    IAst Parse();
}