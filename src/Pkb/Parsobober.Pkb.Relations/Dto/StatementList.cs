namespace Parsobober.Pkb.Relations.Dto;

public record StatementList(int Value) : IComparable
{
    public override string ToString() => Value.ToString();

    public int CompareTo(object? obj) =>
        obj switch
        {
            StatementList statementList => Value.CompareTo(statementList.Value),
            _ => -1
        };
}