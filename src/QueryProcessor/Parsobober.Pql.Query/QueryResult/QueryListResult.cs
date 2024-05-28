using System.Text;
using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query.QueryResult;

internal class QueryListResult(IEnumerable<IComparable> query) : IQueryResult
{
    public class Factory : IQueryResultFactory
    {
        public IQueryResult Create(IEnumerable<IComparable> query) => new QueryListResult(query);
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