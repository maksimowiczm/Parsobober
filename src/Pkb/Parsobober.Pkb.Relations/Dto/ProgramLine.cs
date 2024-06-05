namespace Parsobober.Pkb.Relations.Dto;

public class ProgramLine(int line) : IPkbDto
{
    public int Line { get; } = line;
    public override string ToString() => Line.ToString();
};