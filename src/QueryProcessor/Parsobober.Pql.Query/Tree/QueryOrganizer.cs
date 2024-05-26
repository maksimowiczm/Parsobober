using Microsoft.Extensions.Logging;
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
        var result = InnerOrganize(select)!;

        if (attributes.Count > 0)
        {
            throw new Exception("Not all attributes were used in query.");
        }

        if (queries.Count > 0)
        {
            throw new Exception("Not all relations were used in query.");
        }

        return result;
    }

    private IQueryNode OrganizeSelect(IDeclaration select, IQueryDeclaration rootQuery)
    {
        queries.Remove(rootQuery);

        var anotherSelect = rootQuery.GetAnotherSide(select);

        if (anotherSelect is not null)
        {
            var anotherNode = InnerOrganize(anotherSelect);

            return anotherNode switch
            {
                null => new EnumerableQueryNode(rootQuery.Do(select)),
                not null => new DependentQueryNode(rootQuery, anotherSelect, anotherNode)
            };
        }

        return new EnumerableQueryNode(rootQuery.Do(select));
    }

    private IQueryNode? InnerOrganize(IDeclaration select)
    {
        // break recursion if there are no queries
        if (queries.Count == 0)
        {
            return null;
        }

        var rootQuery = queries.FirstOrDefault(q => q.Left == select || q.Right == select);

        // create root node
        var rootNode = rootQuery switch
        {
            // todo select something else doesn't work right now 
            null => null, // OrganizeSelectNothing(select),
            not null => OrganizeSelect(select, rootQuery)
        };

        // apply attribute 
        var attribute = attributes.SingleOrDefault(a => a.Declaration == select);
        if (attribute is not null)
        {
            return new AttributeQueryNode(attribute, rootNode);
        }

        return rootNode;
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