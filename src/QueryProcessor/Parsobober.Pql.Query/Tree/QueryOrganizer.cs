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
        var query = queries.FirstOrDefault(q => q.Left == select || q.Right == select);

        if (query is null)
        {
            // na pierwsza iteracje wystarczy
            var anotherQuery = queries.First();

            var attributeLeft = attributes.SingleOrDefault(a => a.Declaration == anotherQuery.Left);
            var attributeRight = attributes.SingleOrDefault(a => a.Declaration == anotherQuery.Right);
            var anotherAttribute = (attributeLeft, attributeRight) switch
            {
                (null, { }) => attributeRight,
                ({ }, null) => attributeLeft,
                _ => null
            };

            if (anotherAttribute is not null)
            {
                var anotherRawNode = new EnumerableQueryNode(anotherQuery.Do(select));
                var anotherAttributeQuery = new AttributeQueryNode(anotherAttribute, anotherRawNode);
                return SelectNothing(select, anotherAttributeQuery);
            }

            var nothingQuery = SelectNothing(select, anotherQuery);

            return nothingQuery;
        }

        var attribute = attributes.SingleOrDefault(a => a.Declaration == select);

        var rawNode = new EnumerableQueryNode(query.Do(select));

        if (attribute is not null)
        {
            return new AttributeQueryNode(attribute, rawNode);
        }

        return rawNode;
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

    private BooleanQueryNode SelectNothing(IDeclaration select, IQueryNode query) =>
        select switch
        {
            IStatementDeclaration.Statement => new BooleanQueryNode(query, context.Statements),
            IStatementDeclaration.Assign => new BooleanQueryNode(query, context.Assigns),
            IStatementDeclaration.While => new BooleanQueryNode(query, context.Whiles),
            IVariableDeclaration.Variable => new BooleanQueryNode(query, context.Variables),
            _ => throw new Exception("idk")
        };
}