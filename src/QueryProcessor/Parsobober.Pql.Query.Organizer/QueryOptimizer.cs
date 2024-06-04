using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Organizer;

public class QueryOptimizer(List<IQueryDeclaration> queries)
{
    public int Count => queries.Count;

    /// <summary>
    /// Returns the best query and select declaration.
    /// </summary>
    public (IQueryDeclaration, IDeclaration) GetBest()
    {
        var query = queries.First();
        var select = query.Left as IDeclaration ?? query.Right as IDeclaration;

        return (query, select!);
    }

    public bool HasQueries => queries.Count > 0;

    public void Consume(IQueryDeclaration query) => queries.Remove(query);
}