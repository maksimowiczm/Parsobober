using System.Text;
using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query
{
    internal class Query(IEnumerable<object> query) : IQuery
    {
        public string Execute()
        {
            var stringBuilder = new StringBuilder();

            foreach (var item in query)
            {
                stringBuilder.AppendLine(item.ToString());
            }

            return stringBuilder.ToString();
        }
    }
}