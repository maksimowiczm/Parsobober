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
            null => OrganizeSelectNothing(select),
            not null => OrganizeSelect(select, rootQuery)
        };

        // apply attribute 
        var attribute = attributes.SingleOrDefault(a => a.Declaration == select);
        if (attribute is not null)
        {
            attributes.Remove(attribute);
            return new AttributeQueryNode(attribute, rootNode);
        }

        return rootNode;
    }

    private IQueryNode OrganizeSelectNothing(IDeclaration select)
    {
        // get first query, guaranteed that it doesn't have select
        var query = queries.First();
        queries.Remove(query);

        IQueryNode ambiguousNode = new AmbiguousQueryNode(select, context);

        IQueryNode? conditionNode = null;
        // Na drugą iterację wystarczy chyba
        if (query.Left is IDeclaration leftSelect &&
            queries.Any(q => q.Left == leftSelect || q.Right == leftSelect))
        {
            var leftNode = InnerOrganize(leftSelect)!;
            conditionNode = new DependentQueryNode(query, leftSelect, leftNode);
        }
        else if (query.Right is IDeclaration rightSelect &&
                 queries.Any(q => q.Left == rightSelect || q.Right == rightSelect))
        {
            var rightNode = InnerOrganize(rightSelect)!;
            conditionNode = new DependentQueryNode(query, rightSelect, rightNode);
        }
        else
        {
            var node = InnerOrganize(select);
            if (node is not null)
            {
                conditionNode = new ConditionalQueryNode(ambiguousNode, node);
            }
        }

        if (conditionNode is null)
        {
            var attributeLeft = attributes.SingleOrDefault(a => a.Declaration == query.Left);
            if (attributeLeft is not null)
            {
                attributes.Remove(attributeLeft);
                var attributeNode = new AttributeQueryNode(attributeLeft,
                    new AmbiguousQueryNode((IDeclaration)query.Left, context));
                ambiguousNode = new ConditionalQueryNode(attributeNode, ambiguousNode);
            }

            var attributeRight = attributes.SingleOrDefault(a => a.Declaration == query.Right);
            if (attributeRight is not null)
            {
                attributes.Remove(attributeRight);
                var attributeNode = new AttributeQueryNode(attributeRight,
                    new AmbiguousQueryNode((IDeclaration)query.Right, context));
                ambiguousNode = new ConditionalQueryNode(attributeNode, ambiguousNode);
            }

            return new ConditionalQueryNode(new EnumerableQueryNode(query.Do()), ambiguousNode);
        }

        var result = new ConditionalQueryNode(conditionNode, ambiguousNode);

        return result;
    }
}