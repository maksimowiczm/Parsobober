using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query.QueryResult;

internal class QueryBooleanResult(IEnumerable<IComparable> query) : IQueryResult
{
    public class Factory : IQueryResultFactory
    {
        public IQueryResult Create(IEnumerable<IComparable> query) => new QueryBooleanResult(query);
    }

    public string Execute() => query.Any() ? "TRUE" : "FALSE";
}