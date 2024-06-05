namespace Parsobober.Pkb.Relations.Dto;

public record StatementList(int Line) : IPkbDto
{
    public override string ToString() => Line.ToString();
}