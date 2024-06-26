﻿using System.Collections.Immutable;
using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Abstractions;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries;
using Parsobober.Shared;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Queries.With;
using Alias = (Parsobober.Pql.Query.Arguments.IDeclaration from, Parsobober.Pql.Query.Arguments.IDeclaration to);

namespace Parsobober.Pql.Query.Organizer.Night;

public class Query
{
    public IReadOnlySet<IQueryDeclaration> Queries => _queries;
    public IReadOnlyDictionary<IDeclaration, IPkbDto> Table => _table;

    private readonly Dictionary<IDeclaration, IPkbDto> _table;
    private readonly ImmutableHashSet<IQueryDeclaration> _queries;
    private readonly List<IAttributeQuery> _attributes;
    private readonly List<Alias> _aliases;
    private readonly IDtoProgramContextAccessor _dtos;

    public Query(
        Dictionary<IDeclaration, IPkbDto> table,
        IEnumerable<IQueryDeclaration> queries,
        List<IAttributeQuery> attributes,
        List<Alias> aliases,
        IDtoProgramContextAccessor dtos
    )
    {
        _table = table;
        _attributes = attributes;
        _aliases = aliases;
        _dtos = dtos;

        _queries = queries
            .Select(q =>
            {
                _attributes.ForEach(a =>
                {
                    if (a.Declaration is IStatementDeclaration.Call)
                    {
                        return;
                    }

                    q = a.ApplyAttribute(q);
                });

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

            var query = new Query(table, restOfQueries, _attributes, _aliases, _dtos);
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

        var existingAliases = _aliases
            .Where(a =>
            {
                var (from, to) = a;
                return _table.ContainsKey(from) || _table.ContainsKey(to);
            })
            .ToList();

        var newCache = existingAliases.Select(a =>
            {
                var (from, to) = a;
                if (_table.TryGetValue(from, out var fromValue) && !_table.ContainsKey(to))
                {
                    var toCache = to.Translate(fromValue, _dtos);
                    return toCache
                        .Select(c => new Dictionary<IDeclaration, IPkbDto>(_table) { { to, c } })
                        .ToList();
                }

                if (_table.TryGetValue(to, out var toValue) && !_table.ContainsKey(from))
                {
                    var fromCache = from.Translate(toValue, _dtos);
                    return fromCache
                        .Select(c => new Dictionary<IDeclaration, IPkbDto>(_table) { { from, c } })
                        .ToList();
                }

                return null;
            })
            .WhereNotNull()
            .SelectMany(x => x)
            .ToList();

        if (Irresolvable())
        {
            if (newCache.Count > 0)
            {
                var xd = newCache.Select(c => new Query(c, _queries, _attributes, _aliases, _dtos)).ToList();

                var results = xd.Select(x => x.Execute()).WhereNotNull().SelectMany(x => x).ToList();

                return results;
            }

            if (newCache is null || existingAliases.Count == 0)
            {
                throw new IrresolvableQuery(this);
            }

            return null;
        }

        if (queries.All(QueryDeclarationExtensions.IsBooleanQuery))
        {
            if (queries.All(q => q.Do().Any()))
            {
                return Enumerable.Repeat(_table, 1);
            }

            return null;
        }

        var deeper = new Query(_table, queries, _attributes, _aliases, _dtos).Deeper().ToList();
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
    private readonly List<Alias> _aliases;

    internal QueryOrganizer(
        List<IQueryDeclaration> queries,
        List<IAttributeQuery> attributes,
        IDtoProgramContextAccessor context,
        List<Alias> aliases
    )
    {
        _context = context;
        _aliases = aliases;
        _queries = queries;
        _attributes = attributes;
    }

    private IEnumerable<IPkbDto> ApplyAttribute(IDeclaration select, IEnumerable<IPkbDto> input)
    {
        var attribute = _attributes.FirstOrDefault(a => a.Declaration == select);
        if (attribute is null)
        {
            return input;
        }

        return attribute.ApplyAttribute(input);
    }

    public IEnumerable<IPkbDto> Organize(IDeclaration select)
    {
        // special case with only aliases
        if (_queries.Count == 0 && _aliases.Count == 1)
        {
            // only single alias
            var (from, to) = _aliases.First();
            var fromInput = ApplyAttribute(from, from.ExtractFromContext(_context).ToList());
            var toInput = ApplyAttribute(to, to.ExtractFromContext(_context).ToList());

            var pairs = EnumerableExtensions
                .CartesianProduct([fromInput, toInput])
                .Where(x => new PkbDtoComparer().Equals(x[0], x[1]));

            var result = (select == from) switch
            {
                true => pairs.Select(x => x[0]),
                false => pairs.Select(x => x[1]),
            };

            return result.Distinct();
        }

        if (_queries.Count == 0 && _aliases.Count > 1)
        {
            throw new NotImplementedException();
        }

        var inputs = ApplyAttribute(select, select.ExtractFromContext(_context));

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
            var query = new Query(table, _queries, _attributes, _aliases, _context);

            if (query.Irresolvable())
            {
                var organizer = new QueryOrganizer(query.Queries.ToList(), _attributes, _context, _aliases);
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
                var organizer = new QueryOrganizer(e.Query.Queries.ToList(), _attributes, _context, _aliases);
                if (organizer.OrganizeBoolean())
                {
                    results.Add(table);
                }
            }
        }

        // manualne objeście call.procName
        var callAttributes = _attributes
            .Where(a => a.Declaration is IStatementDeclaration.Call);

        foreach (var a in callAttributes)
        {
            if (a is WithQueryFactory.ProcedureName p)
            {
                results = results
                    .Where(x => x.Values.Any(v => v is Call c && c.ProcedureName == p.Name))
                    .ToList();
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

        // special case with only aliases
        if (_queries.Count == 0 && _aliases.Count == 1)
        {
            // only single alias
            var (from, to) = _aliases.First();
            var fromInput = ApplyAttribute(from, from.ExtractFromContext(_context));
            var toInput = ApplyAttribute(to, to.ExtractFromContext(_context));

            var pairs = EnumerableExtensions
                .CartesianProduct([fromInput, toInput])
                .Where(x => new PkbDtoComparer().Equals(x[0], x[1]));

            return pairs.Any();
        }

        if (_queries.Count == 0 && _aliases.Count > 1)
        {
            throw new NotImplementedException();
        }

        var query = new Query(new Dictionary<IDeclaration, IPkbDto>(), _queries, _attributes, _aliases, _context);

        return query.Execute() is not null;
    }

    public IEnumerable<Dictionary<IDeclaration, IPkbDto>> OrganizerTuple(IEnumerable<IDeclaration> selects)
    {
        // special case with only aliases
        if (_queries.Count == 0 && _aliases.Count == 1)
        {
            // only single alias
            var (from, to) = _aliases.First();
            var fromInput = ApplyAttribute(from, from.ExtractFromContext(_context).ToList());
            var toInput = ApplyAttribute(to, to.ExtractFromContext(_context).ToList());

            var pairs = EnumerableExtensions
                .CartesianProduct([fromInput, toInput])
                .Where(x => new PkbDtoComparer().Equals(x[0], x[1]));

            throw new NotImplementedException();
            // var result = (select == from) switch
            // {
            //     true => pairs.Select(x => x[0]),
            //     false => pairs.Select(x => x[1]),
            // };
            //
            // return result.Distinct();
        }

        if (_queries.Count == 0 && _aliases.Count > 1)
        {
            throw new NotImplementedException();
        }

        var select = selects.First();

        var inputs = ApplyAttribute(select, select.ExtractFromContext(_context));

        // there is no query with select
        if (_queries.All(q => q.Left != select && q.Right != select))
        {
            if (OrganizeBoolean())
            {
                throw new NotImplementedException();
                // return inputs;
            }

            return [];
        }

        var results = new List<Dictionary<IDeclaration, IPkbDto>>();
        foreach (var input in inputs)
        {
            var table = new Dictionary<IDeclaration, IPkbDto> { { select, input } };
            var query = new Query(table, _queries, _attributes, _aliases, _context);

            if (query.Irresolvable())
            {
                var organizer = new QueryOrganizer(query.Queries.ToList(), _attributes, _context, _aliases);
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
                var organizer = new QueryOrganizer(e.Query.Queries.ToList(), _attributes, _context, _aliases);
                if (organizer.OrganizeBoolean())
                {
                    results.Add(table);
                }
            }
        }

        // manualne objeście call.procName
        var callAttributes = _attributes
            .Where(a => a.Declaration is IStatementDeclaration.Call);

        foreach (var a in callAttributes)
        {
            if (a is WithQueryFactory.ProcedureName p)
            {
                results = results
                    .Where(x => x.Values.Any(v => v is Call c && c.ProcedureName == p.Name))
                    .ToList();
            }
        }

        return results;
    }
}