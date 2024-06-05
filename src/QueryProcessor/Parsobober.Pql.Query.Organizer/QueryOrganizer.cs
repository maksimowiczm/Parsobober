using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pql.Query.Abstractions;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Shared;
using QueryContext =
    System.Collections.Generic.Dictionary<
        Parsobober.Pql.Query.Arguments.IDeclaration,
        System.Collections.Generic.IEnumerable<System.IComparable>
    >;

namespace Parsobober.Pql.Query.Organizer;

public class QueryOrganizer : IQueryOrganizer
{
    private readonly List<IQueryDeclaration> _queries;
    private readonly List<IAttributeQuery> _attributes;
    private readonly IDtoProgramContextAccessor _context;

    public QueryOrganizer(
        List<IQueryDeclaration> queries,
        List<IAttributeQuery> attributes,
        IDtoProgramContextAccessor context
    )
    {
        _context = context;
        _queries = queries;
        _attributes = attributes;

        // map all declarations used in queries
        _declarations = _queries
            .SelectMany<IQueryDeclaration, IDeclaration?>(
                q => new[] { q.Left as IDeclaration, q.Right as IDeclaration })
            .Distinct()
            .WhereNotNull()
            .ToList();

        _declarationsMap = new QueryContext();
        _declarations.ForEach(d => TryAddDeclarationToMap(d));
    }

    private readonly List<IDeclaration> _declarations;
    private readonly QueryContext _declarationsMap;

    private bool TryAddDeclarationToMap(IDeclaration declaration)
    {
        // if declaration is already in map => fail
        if (_declarationsMap.ContainsKey(declaration))
        {
            return false;
        }

        // add declaration to map
        _declarationsMap[declaration] = declaration.ExtractFromContext(_context);

        // Apply attribute
        var attribute = _attributes.FirstOrDefault(a => a.Declaration == declaration);
        if (attribute is not null)
        {
            _declarationsMap[declaration] = _declarationsMap[declaration].Intersect(attribute.Do());
        }

        return true;
    }

    public IEnumerable<IComparable> Organize(IDeclaration select)
    {
        var selectNothing = TryAddDeclarationToMap(select);

        foreach (var optimizer in _queries.Select(_ => new QueryOptimizer(_queries.ToList())))
        {
            // iterate multiple times because I said so
            Iterate(optimizer);
        }

        return selectNothing switch
        {
            true when !_declarationsMap.Values.All(v => v.Any()) => Enumerable.Empty<IComparable>(),
            _ => _declarationsMap[select]
        };
    }

    private void Iterate(QueryOptimizer optimizer)
    {
        while (optimizer.Count > 0)
        {
            if (!optimizer.HasQueries)
            {
                break;
            }

            var (query, currentSelect) = optimizer.GetBest();

            ProcessQuery(currentSelect, query);

            optimizer.Consume(query);
        }
    }

    public bool OrganizeBoolean()
    {
        var select = _declarations.FirstOrDefault();

        // if there is select in any query declaration => Organize
        if (select is not null)
        {
            return Organize(select).Any();
        }

        // otherwise just do every query in place
        var result = _queries
            .Select(q => q.Do())
            .All(r => r.Any());

        return result;
    }

    private void ProcessQuery(IDeclaration currentSelect, IQueryDeclaration query)
    {
        var (left, right) = (query.Left, query.Right);

        // if both left and right are not currentSelect => skip
        if (left != currentSelect && right != currentSelect)
        {
            return;
        }

        var other = left == currentSelect ? right : left;

        // if other is not declaration, it is a constant argument
        if (other is not IDeclaration otherDeclaration)
        {
            _declarationsMap[currentSelect] = _declarationsMap[currentSelect].Intersect(query.Do());
            return;
        }

        var newCurrentSelect = _declarationsMap[otherDeclaration]
            .Select(value => query
                .ReplaceArgument(otherDeclaration, IArgument.Parse(value))
                .Do(currentSelect))
            .SelectMany(x => x)
            .Distinct();
        _declarationsMap[currentSelect] = _declarationsMap[currentSelect].Intersect(newCurrentSelect);

        var newOtherSelect = _declarationsMap[currentSelect]
            .Select(value => query.ReplaceArgument(currentSelect, IArgument.Parse(value))
                .Do(otherDeclaration))
            .SelectMany(x => x)
            .Distinct();
        _declarationsMap[otherDeclaration] = _declarationsMap[otherDeclaration].Intersect(newOtherSelect);
    }
}