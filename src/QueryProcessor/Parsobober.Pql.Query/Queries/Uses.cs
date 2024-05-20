using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query.Queries;

internal static class Uses
{
    #region Builder

    public class Builder(IUsesAccessor accessor)
    {
        private readonly List<(string left, string right)> _usesRelations = [];

        public void Add(string left, string right)
        {
            _usesRelations.Add((left, right));
        }

        public IEnumerable<IComparable>? Build(string select, IReadOnlyDictionary<string, IDeclaration> declarations)
        {
            if (_usesRelations.Count == 0)
            {
                return null;
            }

            // todo aktualnie działa tylko dla jednego uses
            // na pierwszą iterację wystarczy

            if (_usesRelations.Count > 1)
            {
                throw new InvalidOperationException("Invalid query");
            }

            var left = _usesRelations.First().left;
            var right = _usesRelations.First().right;

            var query = new InnerBuilder(accessor, select, declarations).Build(left, right);

            return query;
        }
    }

    private class InnerBuilder(
        IUsesAccessor accessor,
        string select,
        IReadOnlyDictionary<string, IDeclaration> declarations
    )
    {
        public IEnumerable<IComparable> Build(string leftStr, string rightStr)
        {
            // parsowanie argumentów
            var leftArgument = IArgument.Parse(declarations, leftStr);
            var rightArgument = IArgument.Parse(declarations, rightStr);

            // pattern matching argumentów
            var query = (leftArgument: leftArgument, rightArgument: rightArgument) switch
            {
                // Uses(stmt, 'v')
                (IStatementDeclaration declaration, IArgument.VarName right) =>
                    new GetStatementsByVariable(accessor, right.Value).Build(declaration),

                // Uses(1, variable)
                (IArgument.Line left, IVariableDeclaration right) =>
                    new GetVariablesByLineNumber(accessor, left.Value).Build(),

                // Uses(stmt, variable)
                (IStatementDeclaration left, IVariableDeclaration right) =>
                    BuildUsesWithSelect((leftStr, left), (rightStr, right)),

                // Uses(1, 'v') nie wspierane w tej wersji
                _ => throw new InvalidOperationException("Invalid query")
            };

            return query;
        }

        private IEnumerable<IComparable> BuildUsesWithSelect(
            (string key, IStatementDeclaration type) left,
            (string key, IVariableDeclaration type) right
        )
        {
            // tu nastąpi samowywrotka przy zapytaniach, w których nie ma wartości z selecta
            // przykład: Select x such that uses(s, v)

            if (left.key == select)
            {
                return new GetStatements(accessor).Build(left.type);
            }

            if (right.key == select)
            {
                return new GetVariablesByStatementType(accessor).Build(left.type);
            }

            throw new InvalidOperationException("Invalid query");
        }
    }

    #endregion

    #region Queries

    private class GetStatements(IUsesAccessor usesAccessor) : UsesQueryStatement
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration statementDeclaration) =>
            statementDeclaration switch
            {
                IStatementDeclaration.Statement => usesAccessor.GetStatements(),
                IStatementDeclaration.Assign => usesAccessor.GetStatements().OfType<Assign>(),
                IStatementDeclaration.While => usesAccessor.GetStatements().OfType<While>(),
                _ => throw new ArgumentOutOfRangeException(nameof(statementDeclaration))
            };
    }

    private class GetVariablesByStatementType(IUsesAccessor usesAccessor) : UsesQueryVariable
    {
        public override IEnumerable<Variable> Build(IStatementDeclaration statementDeclaration) =>
            statementDeclaration switch
            {
                IStatementDeclaration.Statement => usesAccessor.GetVariables<Statement>(),
                IStatementDeclaration.Assign => usesAccessor.GetVariables<Assign>(),
                IStatementDeclaration.While => usesAccessor.GetVariables<While>(),
                _ => throw new ArgumentOutOfRangeException(nameof(statementDeclaration))
            };
    }


    /// <summary>
    /// Get statements of given type by variable name.
    /// </summary>
    /// <param name="usesAccessor">Uses accessor.</param>
    /// <param name="variableName">Variable name.</param>
    private class GetStatementsByVariable(IUsesAccessor usesAccessor, string variableName)
        : UsesQueryStatement
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration left)
        {
            return left switch
            {
                IStatementDeclaration.Statement => usesAccessor.GetStatements(variableName),
                IStatementDeclaration.Assign => usesAccessor.GetStatements(variableName).OfType<Assign>(),
                IStatementDeclaration.While => usesAccessor.GetStatements(variableName).OfType<While>(),
                _ => throw new ArgumentOutOfRangeException(nameof(left))
            };
        }
    }

    /// <summary>
    /// Gets variables used by statement with given line number.
    /// </summary>
    /// <param name="usesAccessor">Uses accessor.</param>
    /// <param name="line">Line number.</param>
    private class GetVariablesByLineNumber(IUsesAccessor usesAccessor, int line)
    {
        public IEnumerable<Variable> Build() => usesAccessor.GetVariables(line);
    }

    /// <summary>
    /// Represents a uses query that returns statements.
    /// </summary>
    private abstract class UsesQueryStatement
    {
        /// <summary>
        /// Builds a query.
        /// </summary>
        /// <param name="declaration"> The declaration to build the query for. </param>
        /// <returns> The query. </returns>
        public abstract IEnumerable<Statement> Build(IStatementDeclaration declaration);
    }

    /// <summary>
    /// Represents a uses query that returns variables.
    /// </summary>
    private abstract class UsesQueryVariable
    {
        /// <summary>
        /// Builds a query.
        /// </summary>
        /// <param name="declaration"> The declaration to build the query for. </param>
        /// <returns> The query. </returns>
        public abstract IEnumerable<Variable> Build(IStatementDeclaration declaration);
    }

    #endregion
}