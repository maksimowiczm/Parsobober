namespace Parsobober.Pkb.Relations.Dto;

public record Procedure(string Name) : IPkbDto
{
    public override string ToString() => Name;
}