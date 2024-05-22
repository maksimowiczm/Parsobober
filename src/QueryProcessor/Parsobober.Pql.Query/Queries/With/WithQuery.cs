using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Queries.With;

internal abstract class WithQuery(IDeclaration declaration) : IAttributeQuery
{
    public IDeclaration Declaration => declaration;
    public abstract IEnumerable<IComparable> Do();
}