using System.Text;
using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query.QueryResult;

internal class QueryListResult(IEnumerable<IComparable> query) : IQueryResult
{
    public class Factory : IQueryResultFactory
    {
        public IQueryResult Create(object query)
        {
            if (query is not IEnumerable<IComparable> queryList)
            {
                throw new IQueryResultFactory.QueryFactoryException("Query must be a list of IComparable");
            }

            return new QueryListResult(queryList);
        }
    }

    public string Execute()
    {
        if (!query.Any())
        {
            return "None";
        }

        var stringBuilder = new StringBuilder();

        stringBuilder.AppendJoin(',', query.OrderBy(s => s));

        return stringBuilder.ToString();
    }
}