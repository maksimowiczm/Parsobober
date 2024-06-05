namespace Parsobober.Pql.Query.Arguments;

public interface IProcedureDeclaration : IDeclaration
{
    new static IDeclaration? Parse(string type, string name)
    {
        return type switch
        {
            "procedure" => new Procedure(name),
            _ => null
        };
    }

    /// <summary>
    /// Represents a procedure declaration in a PQL query.
    /// </summary>
    public readonly record struct Procedure(string Name) : IProcedureDeclaration
    {
#if DEBUG
        public override string ToString() => Name;
#endif
    }
}