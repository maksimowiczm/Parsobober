namespace Parsobober.Pkb.Relations.Dto;

public record ProgramLine(int Line) : IPkbDto
{
    public override string ToString() => Line.ToString();
}