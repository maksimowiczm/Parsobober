namespace Parsobober.Pkb.Relations.Dto;

public record ProgramLine(int LineNumber) : IComparable
{
    public override string ToString() => LineNumber.ToString();

    public int CompareTo(object? obj)
    {
        if (obj is ProgramLine programLine)
        {
            return LineNumber.CompareTo(programLine.LineNumber);
        }

        return -1;
    }
};