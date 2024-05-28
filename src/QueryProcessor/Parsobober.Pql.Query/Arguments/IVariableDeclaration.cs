namespace Parsobober.Pql.Query.Arguments;

internal interface IVariableDeclaration : IDeclaration
{
    new static IDeclaration? Parse(string type, string name)
    {
        return type switch
        {
            "variable" => new Variable(name),
            _ => null
        };
    }

    /// <summary>
    /// Represents a variable declaration in a PQL query.
    /// </summary>
    public readonly record struct Variable(string Name) : IVariableDeclaration
    {
#if DEBUG
        public override string ToString() => Name;
#endif
    }
}