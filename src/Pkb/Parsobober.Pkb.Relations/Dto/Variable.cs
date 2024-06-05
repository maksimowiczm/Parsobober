namespace Parsobober.Pkb.Relations.Dto;

public record Variable(string Name) : IPkbDto
{
    public override string ToString() => Name;
}