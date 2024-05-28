using System.Diagnostics.CodeAnalysis;
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

    /// <summary>
    /// Creates query tree with given select statement.
    /// </summary>
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

    /// <summary>
    /// Creates query node with given select statement. Select statement has to be part of query declaration.
    /// </summary>
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


    /// <summary>
    /// Creates query node with given select statement. Select statement is not part of any query declarations.
    /// </summary>
    private ConditionalQueryNode OrganizeSelectNothing(IDeclaration select)
    {
        // Select nothing => boolean query which returns all elements of select type,
        // example `Select a such that Parent(b, c)`.

        // get first query, guaranteed that it doesn't have select as any of declarations
        var query = queries.First();
        queries.Remove(query);

        // create node which returns all elements of select type
        IQueryNode selectNode = new PkbQueryNode(select, context);

        // Na drugą iterację wystarczy chyba

        // create condition node, if there are any conditions, this node will determine if selectNode should return any elements
        IQueryNode? conditionNode = query switch
        {
            // if left side of query is a declaration, check if it's used in any other query
            // example `Select a such that Parent(b, c) and Parent(b, d)`
            { Left: IDeclaration left } when queries.Any(q => q.Left == left || q.Right == left) =>
                new ReplacerQueryNode(query, left, InnerOrganize(left)!),
            // if right side of query is a declaration, check if it's used in any other query
            // example `Select a such that Parent(b, c) and Parent(c, d)`
            { Right: IDeclaration right } when queries.Any(q => q.Left == right || q.Right == right) =>
                new ReplacerQueryNode(query, right, InnerOrganize(right)!),
            // otherwise, split query into subquery
            // example `Select a such that Parent(b, c) and Parent(d, e)`
            // or      `Select a such that Parent(b, c)`
            _ => IntoSubquery(select, selectNode)
        };

        // no conditions,
        // example `Select a such that Parent(b, c)`
        if (conditionNode is null)
        {
            // if there are no conditions, attributes could be used as conditions
            // example
            //       select    |      query       | attributeLeft | attributeRight
            // `Select a such that Parent(b, c) with b.stmt# = 1 and c.stmt# = 2`

            // check if query declarations are used in any attribute,
            selectNode = ApplyAttribute(query.Left, selectNode);
            selectNode = ApplyAttribute(query.Right, selectNode);

            return new ConditionalQueryNode(new EnumerableQueryNode(query.Do()), selectNode);
        }

        // otherwise use condition node
        var result = new ConditionalQueryNode(conditionNode, selectNode);

        return result;

        #region Inner helper functions

        [SuppressMessage("ReSharper", "VariableHidesOuterVariable")]
        ConditionalQueryNode? IntoSubquery(IDeclaration select, IQueryNode selectNode)
        {
            var subQuery = InnerOrganize(select);
            if (subQuery is not null)
            {
                // if there is a subquery, create conditional node
                return new ConditionalQueryNode(selectNode, subQuery);
            }

            return null;
        }

        IQueryNode ApplyAttribute(IArgument argument, IQueryNode node)
        {
            var attribute = attributes.SingleOrDefault(a => a.Declaration == argument);

            if (attribute is not null)
            {
                attributes.Remove(attribute);
                // guaranteed that there are no subqueries with this argument, so we have to use all elements of select type
                var pkbNode = new PkbQueryNode((IDeclaration)argument, context);
                // create attribute node
                var attributeNode = new AttributeQueryNode(attribute, pkbNode);
                return new ConditionalQueryNode(attributeNode, node);
            }

            // if there are no attributes, return node
            return node;
        }

        #endregion
    }
}