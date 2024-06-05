using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Queries.Exceptions;

namespace Parsobober.Pql.Query.Arguments;

/// <summary>
/// Represents an argument in a PQL query.
/// </summary>
public interface IArgument
{
    /// <summary>
    /// Parses a string argument into an IArgument instance.
    /// </summary>
    /// <param name="declarations">A dictionary of declarations where the key is the declaration name.</param>
    /// <param name="argument">The string argument to parse.</param>
    /// <returns>An IArgument instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the argument cannot be parsed.</exception>
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

        return new Name(argument.Replace('\"', ' ').Trim());
    }

    static IArgument Parse(object argument) =>
        argument switch
        {
            Statement { Line: var line } => new Line(line),
            Variable { Name: var name } => new Name(name),
            Procedure { Name: var procName } => new Name(procName),
            Constant { Value: var value } => new ConstantValue(value),
            ProgramLine { Line: var line } => new Line(line),
            StatementList { Line: var line } => new Line(line),
            _ => throw new ArgumentParseException($"Given argument could not be parsed. {argument}")
        };

    /// <summary>
    /// Represents a line in a PQL query.
    /// </summary>
    public readonly record struct Line(int Value) : IArgument
    {
#if DEBUG
        public override string ToString() => Value.ToString();
#endif
    }

    public readonly record struct Name(string Value) : IArgument
    {
#if DEBUG
        public override string ToString() => $"\"{Value}\"";
#endif
    }

    public record ConstantValue(int Value) : IArgument
    {
#if DEBUG
        public override string ToString() => $"Constant = {Value.ToString()}";
#endif
    };
}