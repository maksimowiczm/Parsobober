using System.Text;

namespace Parsobober.Pql.Query
{
    internal class Query(IEnumerable<object> query) : IQuery
    {
        public string Execute()
        {
            var stringBuilder = new StringBuilder();

            foreach (var item in query)
            {
                stringBuilder.Append(item);
            }

            return stringBuilder.ToString();
        }
    }
}