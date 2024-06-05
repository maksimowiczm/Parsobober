using Parsobober.Pkb.Relations.Abstractions.Accessors;

namespace Parsobober.Pkb.Relations.Dto;

public abstract class Statement(int line) : IModifiesAccessor.IRequest, IUsesAccessor.IRequest, IPkbDto
{
    public int Line { get; } = line;
    public override string ToString() => Line.ToString();
}

public abstract class ContainerStatement(int line) : Statement(line);

public class Assign(int line) : Statement(line);

public class If(int line) : ContainerStatement(line);

public class While(int line) : ContainerStatement(line);

public class Call(int line) : Statement(line);