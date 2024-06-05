using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Utilities;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Queries.Exceptions;

namespace Parsobober.Pql.Query.Queries.With;

internal class WithQueryFactory(IProgramContextAccessor accessor)
{
    public WithQuery Create(IDeclaration declaration, string attribute, string value)
    {
        return attribute switch
        {
            "stmt#" => new StatementLine(declaration, int.Parse(value), accessor),
            "varName" => new VariableName(declaration, value, accessor),
            "value" => new ConstantValue(declaration, int.Parse(value)),
            "procName" => new ProcedureName(declaration, value),
            _ => throw new AttributeParseException(attribute)
        };
    }

    private class ConstantValue(IDeclaration declaration, int value) : WithQuery(declaration)
    {
        public override IEnumerable<IComparable> Do()
        {
            throw new NotImplementedException();
        }

        public override IQueryDeclaration ApplyAttribute(IQueryDeclaration queryDeclaration)
        {
            throw new NotImplementedException();
        }
    }

    private class ProcedureName(IDeclaration declaration, string name) : WithQuery(declaration)
    {
        public override IEnumerable<IComparable> Do()
        {
            throw new NotImplementedException();
        }

        public override IQueryDeclaration ApplyAttribute(IQueryDeclaration queryDeclaration)
        {
            throw new NotImplementedException();
        }
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

        public override IQueryDeclaration ApplyAttribute(IQueryDeclaration queryDeclaration)
        {
            if (queryDeclaration.Left == Declaration)
            {
                return queryDeclaration.ReplaceArgument(Declaration, new IArgument.Line(line));
            }

            if (queryDeclaration.Right == Declaration)
            {
                return queryDeclaration.ReplaceArgument(Declaration, new IArgument.Line(line));
            }

            return queryDeclaration;
        }

#if DEBUG
        public override string ToString() => $"{Declaration.Name}.stmt# = {line}";
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

        public override IQueryDeclaration ApplyAttribute(IQueryDeclaration queryDeclaration)
        {
            if (queryDeclaration.Left == Declaration)
            {
                return queryDeclaration.ReplaceArgument(Declaration, new IArgument.VarName(name));
            }

            if (queryDeclaration.Right == Declaration)
            {
                return queryDeclaration.ReplaceArgument(Declaration, new IArgument.VarName(name));
            }

            return queryDeclaration;
        }
    }
}