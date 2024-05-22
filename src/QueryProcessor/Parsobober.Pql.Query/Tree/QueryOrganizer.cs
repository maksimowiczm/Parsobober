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

        var selectNode = query switch
        {
            null => OrganizeSelectNothing(select),
            not null => new EnumerableQueryNode(query.Do(select))
        };

        // apply attribute on select
        var attribute = attributes.SingleOrDefault(a => a.Declaration == select);
        if (attribute is not null)
        {
            return new AttributeQueryNode(attribute, selectNode);
        }

        return selectNode;
    }

    private IQueryNode OrganizeSelectNothing(IDeclaration select)
    {
        // na pierwsza iteracje wystarczy
        var query = queries.First();

        var attributeLeft = attributes.SingleOrDefault(a => a.Declaration == query.Left);
        var attributeRight = attributes.SingleOrDefault(a => a.Declaration == query.Right);
        var queryAttribute = (attributeLeft, attributeRight) switch
        {
            (null, { }) => attributeRight,
            ({ }, null) => attributeLeft,
            _ => null
        };

        // apply attribute on query without select
        if (queryAttribute is not null)
        {
            var rawQuery = new AmbiguousConditionalQueryNode(select, query, context);
            var attributeNode = new AttributeQueryNode(queryAttribute, new EnumerableQueryNode(query.DoLeft()));
            var selectNothingNode = new ConditionalQueryNode(attributeNode, rawQuery);
            return selectNothingNode;
        }

        var selectNode = new AmbiguousConditionalQueryNode(select, query, context);
        return selectNode;
    }
}