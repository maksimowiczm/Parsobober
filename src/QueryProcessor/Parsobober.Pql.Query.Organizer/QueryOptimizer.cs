using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Organizer;

public class QueryOptimizerFactory(List<IQueryDeclaration> queries, IComparer<IQueryDeclaration> comparer)
{
    public QueryOptimizer Create() => new(queries.ToList(), comparer);
}

public class QueryOptimizer(List<IQueryDeclaration> queries, IComparer<IQueryDeclaration> comparer)
{
    private readonly List<IQueryDeclaration> _queries = queries;

    public int Count => _queries.Count;

    /// <summary>
    /// Returns the best query for given declaration
    /// </summary>
    public (IDeclaration, IQueryDeclaration)? GetBest(IDeclaration select)
    {
        var queries = _queries
            .Where(q => q.Left == select || q.Right == select)
            .ToList();

        queries.Sort(comparer);

        var query = queries.FirstOrDefault();

        if (query is null)
        {
            return null;
        }

        return (select, query);
    }

    /// <summary>
    /// Returns the best query and select declaration.
    /// </summary>
    public (IQueryDeclaration, IDeclaration) GetBest()
    {
        var queriesOneDeclaration = _queries
            .Where(q =>
                q is { Left: IDeclaration, Right: not IDeclaration } or
                    { Left: not IDeclaration, Right: IDeclaration });

        var query1 = queriesOneDeclaration.FirstOrDefault();
        if (query1 is not null)
        {
            var select1 = query1.Left as IDeclaration ?? query1.Right as IDeclaration;
            return (query1, select1!);
        }

        var queryWithTwoDeclarations = _queries
            .Where(q => q is { Left: IDeclaration, Right: IDeclaration })
            .ToList();

        queryWithTwoDeclarations.Sort(comparer);

        var query2 = queryWithTwoDeclarations.First();
        var select2 = query2.Left as IDeclaration ?? query2.Right as IDeclaration;

        return (query2, select2!);
    }

    public bool HasQueries => _queries.Count > 0;

    public void Consume(IQueryDeclaration query) => _queries.Remove(query);
}