namespace Parsobober.Pkb.Relations.Dto;

public class Constant(int value) : IPkbDto
{
    public int Value { get; } = value;
    public override string ToString() => Value.ToString();
}