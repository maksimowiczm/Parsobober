namespace Parsobober.Pkb.Relations.Dto;

public record Constant(int Value) : IComparable
{
    public override string ToString() => Value.ToString();

    public int CompareTo(object? obj) =>
        obj switch
        {
            Constant constant => Value.CompareTo(constant.Value),
            _ => -1
        };
}