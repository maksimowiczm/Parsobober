using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Utilities;
using Parsobober.Pql.Query.Arguments;

namespace Parsobober.Pql.Query.Queries.With;

internal class WithQueryFactory(IProgramContextAccessor accessor)
{
    public WithQuery Create(IDeclaration declaration, string attribute, string value)
    {
        return attribute switch
        {
            "stmt#" => new StatementLine(declaration, int.Parse(value), accessor),
            "varName" => new VariableName(declaration, value, accessor),
            _ => throw new ArgumentException($"Unknown attribute: {attribute}")
        };
    }

    private class StatementLine(IDeclaration declaration, int line, IProgramContextAccessor accessor)
        : WithQuery(declaration)
    {
        public override IEnumerable<IComparable> Do()
        {
            if (accessor.StatementsDictionary.TryGetValue(line, out var statement))
            {
                return Enumerable.Repeat(statement.ToStatement(), 1);
            }

            return [];
        }

#if DEBUG
        public override string ToString() => $"{declaration.Name}.stmt# = {line}";
#endif
    }

    private class VariableName(IDeclaration declaration, string name, IProgramContextAccessor accessor)
        : WithQuery(declaration)
    {
        public override IEnumerable<IComparable> Do()
        {
            if (accessor.VariablesDictionary.TryGetValue(name, out var variable))
            {
                return Enumerable.Repeat(variable.ToVariable(), 1);
            }

            return [];
        }
    }
}