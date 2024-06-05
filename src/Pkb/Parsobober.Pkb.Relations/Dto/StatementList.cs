namespace Parsobober.Pkb.Relations.Dto;

public class StatementList(int line) : IPkbDto
{
    public int Line { get; } = line;

    public override string ToString() => Line.ToString();
}