using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query.QueryResult;

internal interface IQueryResultFactory
{
    IQueryResult Create(object query);

    public class QueryFactoryException(string message) : QueryEvaluatorException(message);
}