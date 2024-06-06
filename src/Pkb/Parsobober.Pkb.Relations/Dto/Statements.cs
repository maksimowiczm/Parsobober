using Parsobober.Pkb.Relations.Abstractions.Accessors;

namespace Parsobober.Pkb.Relations.Dto;

public abstract record Statement(int Line) : IModifiesAccessor.IRequest, IUsesAccessor.IRequest, IPkbDto
{
    public override string ToString() => Line.ToString();
}

public abstract record ContainerStatement(int Line) : Statement(Line)
{
    public override string ToString() => base.ToString();
}

public record Assign(int Line) : Statement(Line)
{
    public override string ToString() => base.ToString();
}

public record If(int Line) : ContainerStatement(Line)
{
    public override string ToString() => base.ToString();
}

public record While(int Line) : ContainerStatement(Line)
{
    public override string ToString() => base.ToString();
}

public record Call(int Line, string ProcedureName) : Statement(Line)
{
    public override string ToString() => base.ToString();

#if DEBUG
    public string DebugString => $"{Line} {ProcedureName}";
#endif
}