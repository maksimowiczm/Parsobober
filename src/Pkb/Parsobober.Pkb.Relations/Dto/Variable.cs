namespace Parsobober.Pkb.Relations.Dto;

public record Variable(string Name) : IComparable
{
    public override string ToString()
    {
        return Name;
    }

    public int CompareTo(object? obj)
    {
        if (obj is Variable variable)
        {
            return string.Compare(Name, variable.Name, StringComparison.Ordinal);
        }

        return -1;
    }
}