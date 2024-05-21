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
    private (IQueryNode node, IDeclaration? next) CreateNode(IDeclaration entry)
    {
        var query = queries.FirstOrDefault(q => q.Left == entry || q.Right == entry);

        if (query is null)
        {
            // na pierwsza iteracje wystarczy
            var anotherQuery = queries.First()!;
            var nothingQuery = SelectNothing(entry, anotherQuery);
            queries.Remove(anotherQuery);

            return (nothingQuery, null);
        }

        var next = query.GetAnother(entry) as IDeclaration;
        var attribute = attributes.SingleOrDefault(a => a.Declaration == entry);

        queries.Remove(query);
        var rawNode = new EnumerableQueryNode(query.Do(entry));

        if (attribute is not null)
        {
            attributes.Remove(attribute);
            return (new AttributeQueryNode(attribute, rawNode), next);
        }

        return (rawNode, next);
    }

    /// <summary>
    /// Organizes queries and select statement into query tree.
    /// </summary>
    /// <returns>Query tree</returns>
    public IQueryNode Organize(IDeclaration select)
    {
        // na pierwsza iteracje wystarczy
        var (root, next) = CreateNode(select);

        return root;
    }

    private BooleanQueryNode SelectNothing(IDeclaration select, IQueryDeclaration query) =>
        select switch
        {
            IStatementDeclaration.Statement => new BooleanQueryNode(query, context.Statements),
            IStatementDeclaration.Assign => new BooleanQueryNode(query, context.Assigns),
            IStatementDeclaration.While => new BooleanQueryNode(query, context.Whiles),
            IVariableDeclaration.Variable => new BooleanQueryNode(query, context.Variables),
            _ => throw new Exception("idk")
        };
}