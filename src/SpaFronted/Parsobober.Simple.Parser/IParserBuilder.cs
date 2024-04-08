namespace Parsobober.Simple.Parser;

public interface IParserBuilder
{
    ISimpleParser BuildParser(string programCode);
}