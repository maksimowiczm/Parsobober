namespace Parsobober.Pkb.Relations.Dto;

public record Procedure(string ProcName) : IComparable
{
    public override string ToString()
    {
        return ProcName;
    }

    public int CompareTo(object? obj)
    {
        if (obj is Procedure procedure)
        {
            return string.Compare(ProcName, procedure.ProcName, StringComparison.Ordinal);
        }

        return -1;
    }
}