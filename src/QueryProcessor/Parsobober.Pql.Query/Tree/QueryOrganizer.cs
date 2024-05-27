using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Tree.Exceptions;
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
            throw new NotAllAttributesUsedException();
        }

        if (queries.Count > 0)
        {
            throw new NotAllRelationsUsedException();
        }

        return result;
    }

    private IQueryNode? InnerOrganize(IDeclaration select)
    {
        // break recursion if there are no queries
        if (queries.Count == 0)
        {
            return null;
        }

        // get query with select
        var selectQuery = queries.FirstOrDefault(q => q.Left == select || q.Right == select);

        // create root node
        var selectNode = selectQuery switch
        {
            // if there is query with select => OrganizeSelect
            not null => OrganizeSelect(select, selectQuery),
            // if there is no query with select => OrganizeSelectNothing
            null => OrganizeSelectNothing(select)
        };

        // apply attribute 
        var selectAttribute = attributes.SingleOrDefault(a => a.Declaration == select);
        // if there is attribute, create attribute node wrapper
        if (selectAttribute is not null)
        {
            attributes.Remove(selectAttribute);
            return new AttributeQueryNode(selectAttribute, selectNode);
        }

        // otherwise return select node
        return selectNode;
    }

    private IQueryNode OrganizeSelect(IDeclaration select, IQueryDeclaration rootQuery)
    {
        queries.Remove(rootQuery);

        // get another side of query, example `Parent(a, b)` with `select` = a then `anotherSelect` = b
        var anotherDeclaration = rootQuery.GetAnotherSide(select);

        // if there is another declaration
        if (anotherDeclaration is not null)
        {
            // recursively organize another declaration (subquery)
            var anotherNode = InnerOrganize(anotherDeclaration);

            // If there is another node, it means that there is subquery,
            // example `Select a such that Parent(a, b) and Parent(b, c)`.
            // It will create DependentQueryNode which will replace `b` in `Parent(a, b)`
            // with result of subquery `Parent(b, c)`.
            // Otherwise create EnumerableQueryNode.
            return anotherNode switch
            {
                null => new EnumerableQueryNode(rootQuery.Do(select)),
                not null => new ReplacerQueryNode(rootQuery, anotherDeclaration, anotherNode)
            };
        }

        var rootNode = new EnumerableQueryNode(rootQuery.Do(select));

        // if there is no another declaration, it means that there might be boolean condition,
        // example |             query       |   conditions...
        // `Select a such that Parent(a, b) and Parent(c, d) and Parent(d, e)`.
        // Delegate to InnerOrganize to find out if there are any conditions.
        var rest = InnerOrganize(select);
        // if there are conditions, create ConditionalQueryNode
        if (rest is not null)
        {
            return new ConditionalQueryNode(rest, rootNode);
        }

        return rootNode;
    }

    private IQueryNode OrganizeSelectNothing(IDeclaration select)
    {
        // czarna magia nie wiem o co tu chodziło xD. Działa, pozdrawiam

        // get first query, guaranteed that it doesn't have select as any of declarations
        var query = queries.First();
        queries.Remove(query);

        // create ambiguous node with select
        IQueryNode ambiguousNode = new PkbQueryNode(select, context);

        IQueryNode? conditionNode = null;
        // Na drugą iterację wystarczy chyba
        if (query.Left is IDeclaration leftSelect &&
            queries.Any(q => q.Left == leftSelect || q.Right == leftSelect))
        {
            var leftNode = InnerOrganize(leftSelect)!;
            conditionNode = new ReplacerQueryNode(query, leftSelect, leftNode);
        }
        else if (query.Right is IDeclaration rightSelect &&
                 queries.Any(q => q.Left == rightSelect || q.Right == rightSelect))
        {
            var rightNode = InnerOrganize(rightSelect)!;
            conditionNode = new ReplacerQueryNode(query, rightSelect, rightNode);
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
                    new PkbQueryNode((IDeclaration)query.Left, context));
                ambiguousNode = new ConditionalQueryNode(attributeNode, ambiguousNode);
            }

            var attributeRight = attributes.SingleOrDefault(a => a.Declaration == query.Right);
            if (attributeRight is not null)
            {
                attributes.Remove(attributeRight);
                var attributeNode = new AttributeQueryNode(attributeRight,
                    new PkbQueryNode((IDeclaration)query.Right, context));
                ambiguousNode = new ConditionalQueryNode(attributeNode, ambiguousNode);
            }

            return new ConditionalQueryNode(new EnumerableQueryNode(query.Do()), ambiguousNode);
        }

        var result = new ConditionalQueryNode(conditionNode, ambiguousNode);

        return result;
    }
}