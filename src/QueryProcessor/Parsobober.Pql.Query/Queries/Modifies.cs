using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Queries.Core;
using Parsobober.Pql.Query.Queries.Exceptions;

namespace Parsobober.Pql.Query.Queries;

internal static class Modifies
{
    public class QueryDeclaration(IArgument left, IArgument right, IModifiesAccessor accessor)
        : ReplaceableArgumentQueryDeclaration<QueryDeclaration>, IQueryDeclaration
    {
        public override IArgument Left { get; } = left;
        public override IArgument Right { get; } = right;

        public override IEnumerable<IPkbDto> Do()
        {
            // pattern matching argumentów
            var query = (Left, Right) switch
            {
                (Line line, Name name) =>
                    new BooleanStatementModifiesQuery(accessor, line.Value, name.Value).Build(),
                (Name procName, Name varName) =>
                    new BooleanProcedureModifiesQuery(accessor, procName.Value, varName.Value).Build(),
                _ => DoDeclaration()
            };

            return query;
        }

        public override IEnumerable<IPkbDto> Do(IDeclaration select)
        {
            // pattern matching argumentów
            var query = (Left, Right) switch
            {
                // Modifies(stmt, 'v')
                (IStatementDeclaration declaration, Name right) =>
                    new GetStatementsByVariable(accessor, right.Value).Build(declaration),

                // Modifies(1, variable)
                (Line left, IVariableDeclaration) =>
                    accessor.GetVariables(left.Value),

                // Modifies('proc', variable)
                (Name left, IVariableDeclaration right) =>
                    accessor.GetVariables(left.Value),

                // Modifies(proc, 'v')
                (IProcedureDeclaration left, Name right) =>
                    accessor.GetProcedures(right.Value),

                // Modifies(stmt, variable)
                (IStatementDeclaration left, IVariableDeclaration right) => BuildModifiesWithSelect(left, right),

                _ => throw new QueryNotSupported(this, $"Modifies({Left}, {Right}) is not supported.")
            };

            return query;

            IEnumerable<IPkbDto> BuildModifiesWithSelect(
                IStatementDeclaration left,
                IVariableDeclaration right
            )
            {
                // tu nastąpi samowywrotka przy zapytaniach, w których nie ma wartości z selecta
                // przykład: Select x such that modifes(s, v)

                if (left == select)
                {
                    return new GetStatements(accessor).Build(left);
                }

                if (right == select)
                {
                    return new GetVariablesByStatementType(accessor).Build(left);
                }

                throw new DeclarationNotFoundException(select, this);
            }
        }

        protected override QueryDeclaration CloneSelf(IArgument left, IArgument right) => new(left, right, accessor);
    }

    #region Queries

    private class GetStatements(IModifiesAccessor modifiesAccessor) : ModifiesQueryStatement
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration statementDeclaration) =>
            statementDeclaration switch
            {
                IStatementDeclaration.Statement => modifiesAccessor.GetStatements(),
                IStatementDeclaration.Assign => modifiesAccessor.GetStatements().OfType<Assign>(),
                IStatementDeclaration.While => modifiesAccessor.GetStatements().OfType<While>(),
                IStatementDeclaration.If => modifiesAccessor.GetStatements().OfType<If>(),
                IStatementDeclaration.Call => modifiesAccessor.GetStatements().OfType<Call>(),
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
                IStatementDeclaration.If => modifiesAccessor.GetVariables<If>(),
                IStatementDeclaration.Call => modifiesAccessor.GetVariables<Call>(),
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
                IStatementDeclaration.If => modifiesAccessor.GetStatements(variableName).OfType<If>(),
                IStatementDeclaration.Call => modifiesAccessor.GetStatements(variableName).OfType<Call>(),
                _ => throw new ArgumentOutOfRangeException(nameof(left))
            };
        }
    }

    private class BooleanStatementModifiesQuery(IModifiesAccessor accessor, int line, string variableName)
    {
        public IEnumerable<IPkbDto> Build() => IPkbDto.Boolean(accessor.IsModified(line, variableName));
    }

    private class BooleanProcedureModifiesQuery(IModifiesAccessor accessor, string procedureName, string variableName)
    {
        public IEnumerable<IPkbDto> Build() => IPkbDto.Boolean(accessor.IsModified(procedureName, variableName));
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