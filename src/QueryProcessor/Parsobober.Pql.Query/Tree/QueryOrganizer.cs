using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Tree.Node;

namespace Parsobober.Pql.Query.Tree;

/// <summary>
/// Organizes queries and select statement into query tree.
/// </summary>
/// <param name="select">Select statement</param>
/// <param name="queries">Queries declarations</param>
internal class QueryOrganizer(IDeclaration select, IEnumerable<IQueryDeclaration> queries)
{
    /// <summary>
    /// Organizes queries and select statement into query tree.
    /// </summary>
    /// <returns>Query tree</returns>
    public IQueryNode Organize()
    {
        // na pierwsza iteracje wystarczy
        var query = queries.First();

        // if there is nothing to select
        if (!IsSelect(query, q => q.Left, select) &&
            !IsSelect(query, q => q.Right, select))
        {
            return select switch
            {
                // todo replace Enumerable with valid data from context
                IStatementDeclaration.Statement => new BooleanQueryNode(select, query, Enumerable.Empty<Statement>),
                IStatementDeclaration.Assign => new BooleanQueryNode(select, query, Enumerable.Empty<Assign>),
                IStatementDeclaration.While => new BooleanQueryNode(select, query, Enumerable.Empty<While>),
                IVariableDeclaration.Variable => new BooleanQueryNode(select, query, Enumerable.Empty<Variable>),
                _ => throw new Exception("idk")
            };
        }

        return new QueryNode(select, query);
    }

    private static bool IsSelect(
        IQueryDeclaration query,
        Func<IQueryDeclaration, IArgument> sideSelector,
        IDeclaration select
    ) => sideSelector(query) is IDeclaration side && side.Name == select.Name;
}