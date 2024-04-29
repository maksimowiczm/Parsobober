namespace Parsobober.Pql.Query;

public interface IArgument
{
    static IArgument Parse(IReadOnlyDictionary<string, IDeclaration> declarations, string argument)
    {
        if (declarations.TryGetValue(argument, out var declaration))
        {
            return declaration;
        }

        if (int.TryParse(argument, out var line))
        {
            return new Line(line);
        }

        throw new ArgumentOutOfRangeException();
    }

    public readonly record struct Line(int Value) : IArgument;
}

public interface IDeclaration : IArgument
{
    static IDeclaration Parse(string type)
    {
        return type switch
        {
            "stmt" => new Statement(),
            "while" => new While(),
            "assign" => new Assign(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    Type ToDtoStatementType()
    {
        return this switch
        {
            Statement => typeof(Pkb.Relations.Dto.Statement),
            While => typeof(Pkb.Relations.Dto.While),
            Assign => typeof(Pkb.Relations.Dto.Assign),
            _ => throw new InvalidOperationException()
        };
    }

    public readonly record struct Statement : IDeclaration;

    public readonly record struct While : IDeclaration;

    public readonly record struct Assign : IDeclaration;
}