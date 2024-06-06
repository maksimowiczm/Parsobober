namespace Parsobober.Pkb.Relations.Dto;

public record Constant(int Value) : IPkbDto
{
    public override string ToString() => Value.ToString();
}