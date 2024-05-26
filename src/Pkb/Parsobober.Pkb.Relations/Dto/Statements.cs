using Parsobober.Pkb.Relations.Abstractions.Accessors;

namespace Parsobober.Pkb.Relations.Dto;

public abstract record Statement(int LineNumber) : IModifiesAccessor.IRequest, IUsesAccessor.IRequest, IComparable
{
    public override string ToString() => LineNumber.ToString();

    public int CompareTo(object? obj)
    {
        if (obj is Statement statement)
        {
            return LineNumber.CompareTo(statement.LineNumber);
        }

        return -1;
    }
};

public record Assign(int LineNumber) : Statement(LineNumber)
{
    public override string ToString() => base.ToString();
}

public record If(int LineNumber) : Statement(LineNumber)
{
    public override string ToString() => base.ToString();
}

public record While(int LineNumber) : Statement(LineNumber)
{
    public override string ToString() => base.ToString();
}

public record Call(int LineNumber) : Statement(LineNumber)
{
    public override string ToString() => base.ToString();
}