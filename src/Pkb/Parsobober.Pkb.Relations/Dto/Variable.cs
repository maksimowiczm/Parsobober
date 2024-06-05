namespace Parsobober.Pkb.Relations.Dto;

public class Variable(string name) : IPkbDto
{
    public string Name { get; } = name;
    public override string ToString() => Name;
}