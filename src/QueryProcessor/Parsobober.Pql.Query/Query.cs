using System.Text;
using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query
{
    internal class Query(IEnumerable<IComparable> query) : IQuery
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
}