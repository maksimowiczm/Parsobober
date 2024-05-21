using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Tree.Node;

namespace Parsobober.Pql.Query.Tree;

/// <summary>
/// Organizes queries and select statement into query tree.
/// </summary>
internal class QueryOrganizer(
    List<IQueryDeclaration> queries,
    List<IAttributeQuery> attributes,
    IDtoProgramContextAccessor context
)
{
    /// <summary>
    /// Organizes queries and select statement into query tree.
    /// </summary>
    /// <returns>Query tree</returns>
    public IQueryNode Organize(IDeclaration select)
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
                IStatementDeclaration.Statement => new BooleanQueryNode(select, query, context.Statements),
                IStatementDeclaration.Assign => new BooleanQueryNode(select, query, context.Assigns),
                IStatementDeclaration.While => new BooleanQueryNode(select, query, context.Whiles),
                IVariableDeclaration.Variable => new BooleanQueryNode(select, query, context.Variables),
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