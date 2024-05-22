using System.Text;
using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query;

internal class QueryResult(IEnumerable<IComparable> query) : IQueryResult
{
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