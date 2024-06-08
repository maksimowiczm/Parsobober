namespace Parsobober.Pql.Pattern.Parser.Abstractions;
internal interface IPatternParserBuilder
{
    IPatternParser BuildParser(string pattern);
}
