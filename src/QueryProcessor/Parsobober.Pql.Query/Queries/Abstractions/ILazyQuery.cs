using Parsobober.Pql.Query.Arguments;

namespace Parsobober.Pql.Query.Queries.Abstractions;

internal interface ILazyQuery
{
    IEnumerable<IComparable> Do(IDeclaration select);
}