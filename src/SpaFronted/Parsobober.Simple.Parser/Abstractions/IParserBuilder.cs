namespace Parsobober.Simple.Parser.Abstractions;

public interface IParserBuilder
{
    ISimpleParser BuildParser(string programCode);
}