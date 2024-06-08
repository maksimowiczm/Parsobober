﻿using System.Collections.Immutable;
using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Abstractions;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries;
using Parsobober.Shared;
using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Organizer.Night;

public class Query
{
    public IReadOnlySet<IQueryDeclaration> Queries => _queries;
    public IReadOnlyDictionary<IDeclaration, IPkbDto> Table => _table;

    private readonly Dictionary<IDeclaration, IPkbDto> _table;
    private readonly ImmutableHashSet<IQueryDeclaration> _queries;

    public Query(Dictionary<IDeclaration, IPkbDto> table, IEnumerable<IQueryDeclaration> queries)
    {
        _table = table;

        _queries = queries
            .Select(q =>
            {
                var query = q;
                if (q.Left is IDeclaration left && _table.TryGetValue(left, out var valueLeft))
                {
                    query = query.ReplaceArgument(left, IArgument.Parse(valueLeft));
                }

                if (q.Right is IDeclaration right && _table.TryGetValue(right, out var valueRight))
                {
                    query = query.ReplaceArgument(right, IArgument.Parse(valueRight));
                }

                return query;
            })
            .ToImmutableHashSet();
    }

    private List<Query> Deeper()
    {
        var declarations = _queries
            .Where(q => q is { Left: IDeclaration } or { Right: IDeclaration })
            .Select(q => q.Left as IDeclaration ?? (q.Right as IDeclaration)!)
            .ToHashSet();

        var resolvable = _queries
            .Where(q =>
                (q is { Left: not IDeclaration, Right: IDeclaration right, } && declarations.Contains(right)) ||
                (q is { Left: IDeclaration left, Right: not IDeclaration } && declarations.Contains(left))
            )
            .Select(q => (q.Left as IDeclaration ?? (q.Right as IDeclaration)!, q))
            .ToList();

        var restOfQueries = _queries.Except(resolvable.Select(r => r.q));

        var pairs = resolvable
            .Select(x =>
            {
                var (d, q) = x;
                return (d, q.Do());
            })
            .GroupBy(x => x.d, x => x.Item2)
            .Select(x => (x.Key, EnumerableExtensions.IntersectMany(x.ToList()).Distinct()))
            .ToDictionary(x => x.Key, x => x.Item2.Distinct().ToList())
            .Select(x => x.Value.Select(z => (x.Key, z)))
            .ToList();

        var cartesian = pairs.CartesianProduct();

        var result = new List<Query>();
        foreach (var values in cartesian)
        {
            var table = _table.ToDictionary(x => x.Key, x => x.Value);
            foreach (var (k, v) in values)
            {
                table.Add(k, v);
            }

            var query = new Query(table, restOfQueries);
            result.Add(query);
        }

        return result;
    }

    public class IrresolvableQuery(Query query) : Exception
    {
        public Query Query { get; } = query;
    }

    public IEnumerable<Dictionary<IDeclaration, IPkbDto>>? Execute()
    {
        var booleanQueries = _queries
            .Where(QueryDeclarationExtensions.IsBooleanQuery)
            .ToList();

        if (booleanQueries.Count > 0)
        {
            if (booleanQueries.Any(q => !q.Do().Any()))
            {
                return null;
            }
        }

        var queries = _queries
            .Where(q => !booleanQueries.Contains(q))
            .ToList();

        if (Irresolvable())
        {
            throw new IrresolvableQuery(this);
        }

        if (queries.All(QueryDeclarationExtensions.IsBooleanQuery))
        {
            if (queries.All(q => q.Do().Any()))
            {
                return Enumerable.Repeat(_table, 1);
            }

            return null;
        }

        var deeper = new Query(_table, queries).Deeper().ToList();
        var result = deeper
            .Select(q => q.Execute())
            .WhereNotNull()
            .SelectMany(x => x)
            .ToList();

        return result;
    }

    public bool Irresolvable()
    {
        if (_queries.Count == 0)
        {
            return false;
        }

        if (_queries.Any(q => q is
                { Left: not IDeclaration, Right: not null } or
                { Left: not null, Right: not IDeclaration }))
        {
            return false;
        }

        var result = _queries.All(q =>
            (q is { Left: IDeclaration left } && !_table.ContainsKey(left)) ||
            (q is { Right: IDeclaration right } && !_table.ContainsKey(right))
        );

        return result;
    }

#if DEBUG
    private bool IsIrresolvable => Irresolvable();
    private bool HasResult => _queries
        .Where(QueryDeclarationExtensions.IsBooleanQuery)
        .ToList()
        .Any(q => q.Do().Any());
#endif
}

public class QueryOrganizer : IQueryOrganizer
{
    private readonly List<IQueryDeclaration> _queries;
    private readonly List<IAttributeQuery> _attributes;
    private readonly IDtoProgramContextAccessor _context;

    internal QueryOrganizer(
        List<IQueryDeclaration> queries,
        List<IAttributeQuery> attributes,
        IDtoProgramContextAccessor context
    )
    {
        _context = context;
        _queries = queries;
        _attributes = attributes;
    }

    public IEnumerable<IPkbDto> Organize(IDeclaration select)
    {
        var inputs = select.ExtractFromContext(_context);

        // there is no query with select
        if (_queries.All(q => q.Left != select && q.Right != select))
        {
            if (OrganizeBoolean())
            {
                return inputs;
            }

            return Enumerable.Empty<IPkbDto>();
        }

        var results = new List<Dictionary<IDeclaration, IPkbDto>>();
        foreach (var input in inputs)
        {
            var table = new Dictionary<IDeclaration, IPkbDto> { { select, input } };
            var query = new Query(table, _queries);

            if (query.Irresolvable())
            {
                var organizer = new QueryOrganizer(query.Queries.ToList(), _attributes, _context);
                if (organizer.OrganizeBoolean())
                {
                    results.Add(table);
                }
            }

            try
            {
                var result = query.Execute()?.ToList();
                if (result is not null && result.Count > 0)
                {
                    results.AddRange(result);
                }
            }
            catch (Query.IrresolvableQuery e)
            {
                var organizer = new QueryOrganizer(e.Query.Queries.ToList(), _attributes, _context);
                if (organizer.OrganizeBoolean())
                {
                    results.Add(table);
                }
            }
        }

        var selectValues = results
            .Select(x => x[select])
            .ToHashSet();

        return selectValues;
    }

    public bool OrganizeBoolean()
    {
        var rootQuery = _queries.FirstOrDefault(q => q is { Left: IDeclaration, Right: IDeclaration });

        if (rootQuery is not null)
        {
            var select = (rootQuery.Left as IDeclaration)!;
            var result = Organize(select);
            return result.Any();
        }

        var query = new Query(new Dictionary<IDeclaration, IPkbDto>(), _queries);

        return query.Execute() is not null;
    }
}