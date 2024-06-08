namespace Parsobober.Pql.Pattern.Parser.Abstractions;
public interface IPatternParserBuilder
{
    IPatternParser BuildParser(string pattern);
}
