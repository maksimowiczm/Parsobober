using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Queries;

internal class RawQuery(Func<IEnumerable<IComparable>> query) : ILazyQuery
{
    public IEnumerable<IComparable> Do(IDeclaration select) => query();
}