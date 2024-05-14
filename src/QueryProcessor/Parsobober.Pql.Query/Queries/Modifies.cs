using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query.Queries;

internal static class Modifies
{
    #region Builder

    public class Builder(IModifiesAccessor accessor)
    {
        private readonly List<(string left, string right)> _modifiesRelations = [];

        public void Add(string left, string right)
        {
            _modifiesRelations.Add((left, right));
        }

        public IEnumerable<IComparable>? Build(string select, IReadOnlyDictionary<string, IDeclaration> declarations)
        {
            if (_modifiesRelations.Count == 0)
            {
                return null;
            }

            // todo aktualnie działa tylko dla jednego modifes
            // na pierwszą iterację wystarczy

            if (_modifiesRelations.Count > 1)
            {
                throw new InvalidOperationException("Invalid query");
            }

            var left = _modifiesRelations.First().left;
            var right = _modifiesRelations.First().right;

            var query = new InnerBuilder(accessor, select, declarations).Build(left, right);

            return query;
        }
    }

    private class InnerBuilder(
        IModifiesAccessor accessor,
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
                // Modifies(stmt, 'v')
                (IStatementDeclaration declaration, IArgument.VarName right) =>
                    new GetStatementsByVariable(accessor, right.Value).Build(declaration),

                // Modifies(1, variable)
                (IArgument.Line left, IVariableDeclaration right) =>
                    new GetVariablesByLineNumber(accessor, left.Value).Build(),

                // Modifies(stmt, variable)
                (IStatementDeclaration left, IVariableDeclaration right) =>
                    BuildModifiesWithSelect((leftStr, left), (rightStr, right)),

                // Modifies(1, 'v') nie wspierane w tej wersji
                _ => throw new InvalidOperationException("Invalid query")
            };

            return query;
        }

        private IEnumerable<IComparable> BuildModifiesWithSelect(
            (string key, IStatementDeclaration type) left,
            (string key, IVariableDeclaration type) right
        )
        {
            // tu nastąpi samowywrotka przy zapytaniach, w których nie ma wartości z selecta
            // przykład: Select x such that modifes(s, v)

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

    private class GetStatements(IModifiesAccessor modifiesAccessor) : ModifiesQueryStatement
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration statementDeclaration) =>
            statementDeclaration switch
            {
                IStatementDeclaration.Statement => modifiesAccessor.GetStatements(),
                IStatementDeclaration.Assign => modifiesAccessor.GetStatements().OfType<Assign>(),
                IStatementDeclaration.While => modifiesAccessor.GetStatements().OfType<While>(),
                _ => throw new ArgumentOutOfRangeException(nameof(statementDeclaration))
            };
    }

    private class GetVariablesByStatementType(IModifiesAccessor modifiesAccessor) : ModifiesQueryVariable
    {
        public override IEnumerable<Variable> Build(IStatementDeclaration statementDeclaration) =>
            statementDeclaration switch
            {
                IStatementDeclaration.Statement => modifiesAccessor.GetVariables<Statement>(),
                IStatementDeclaration.Assign => modifiesAccessor.GetVariables<Assign>(),
                IStatementDeclaration.While => modifiesAccessor.GetVariables<While>(),
                _ => throw new ArgumentOutOfRangeException(nameof(statementDeclaration))
            };
    }


    /// <summary>
    /// Get statements of given type by variable name.
    /// </summary>
    /// <param name="modifiesAccessor">Modifies accessor.</param>
    /// <param name="variableName">Variable name.</param>
    private class GetStatementsByVariable(IModifiesAccessor modifiesAccessor, string variableName)
        : ModifiesQueryStatement
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration left)
        {
            return left switch
            {
                IStatementDeclaration.Statement => modifiesAccessor.GetStatements(variableName),
                IStatementDeclaration.Assign => modifiesAccessor.GetStatements(variableName).OfType<Assign>(),
                IStatementDeclaration.While => modifiesAccessor.GetStatements(variableName).OfType<While>(),
                _ => throw new ArgumentOutOfRangeException(nameof(left))
            };
        }
    }

    /// <summary>
    /// Gets variables modified by statement with given line number.
    /// </summary>
    /// <param name="modifiesAccessor">Modifies accessor.</param>
    /// <param name="line">Line number.</param>
    private class GetVariablesByLineNumber(IModifiesAccessor modifiesAccessor, int line)
    {
        public IEnumerable<Variable> Build() => modifiesAccessor.GetVariables(line);
    }

    /// <summary>
    /// Represents a modifies query that returns statements.
    /// </summary>
    private abstract class ModifiesQueryStatement
    {
        /// <summary>
        /// Builds a query.
        /// </summary>
        /// <param name="declaration"> The declaration to build the query for. </param>
        /// <returns> The query. </returns>
        public abstract IEnumerable<Statement> Build(IStatementDeclaration declaration);
    }

    /// <summary>
    /// Represents a modifies query that returns variables.
    /// </summary>
    private abstract class ModifiesQueryVariable
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