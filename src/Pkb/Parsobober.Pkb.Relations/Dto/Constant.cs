namespace Parsobober.Pkb.Relations.Dto;

public record Constant(int Value) : IComparable
{
    public override string ToString() => Value.ToString();

    public int CompareTo(object? obj) =>
        obj switch
        {
            Statement statement => Value.CompareTo(statement.LineNumber),
            Procedure procedure => string.Compare(Value.ToString(), procedure.ProcName, StringComparison.Ordinal),
            Variable variable => string.Compare(Value.ToString(), variable.Name, StringComparison.Ordinal),
            _ => -1
        };
}