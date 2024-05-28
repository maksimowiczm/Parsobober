using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query.QueryResult;

internal interface IQueryResultFactory
{
    IQueryResult Create(IEnumerable<IComparable> query);
}