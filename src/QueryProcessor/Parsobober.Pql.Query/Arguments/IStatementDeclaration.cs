namespace Parsobober.Pql.Query.Arguments;

public interface IStatementDeclaration : IDeclaration
{
    new static IDeclaration? Parse(string type, string name)
    {
        return type switch
        {
            "stmt" => new Statement(name),
            "while" => new While(name),
            "assign" => new Assign(name),
            "if" => new If(name),
            "call" => new Call(name),
            _ => null
        };
    }

    /// <summary>
    /// Represents a statement declaration in a PQL query.
    /// </summary>
    public readonly record struct Statement(string Name) : IStatementDeclaration
    {
#if DEBUG
        public override string ToString() => Name;
#endif
    }

    /// <summary>
    /// Represents a while declaration in a PQL query.
    /// </summary>
    public readonly record struct While(string Name) : IStatementDeclaration
    {
#if DEBUG
        public override string ToString() => Name;
#endif
    }

    /// <summary>
    /// Represents an assign declaration in a PQL query.
    /// </summary>
    public readonly record struct Assign(string Name) : IStatementDeclaration
    {
#if DEBUG
        public override string ToString() => Name;
#endif
    }

    /// <summary>
    /// Represents an if declaration in a PQL query.
    /// </summary>
    public readonly record struct If(string Name) : IStatementDeclaration
    {
#if DEBUG
        public override string ToString() => Name;
#endif
    }

    /// <summary>
    /// Represents an call declaration in a PQL query.
    /// </summary>
    public readonly record struct Call(string Name) : IStatementDeclaration
    {
#if DEBUG
        public override string ToString() => Name;
#endif
    }
}