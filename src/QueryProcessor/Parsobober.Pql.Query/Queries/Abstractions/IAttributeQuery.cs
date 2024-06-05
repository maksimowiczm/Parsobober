using Parsobober.Pql.Query.Arguments;

namespace Parsobober.Pql.Query.Queries.Abstractions;

public interface IAttributeQuery
{
    IDeclaration Declaration { get; }

    IEnumerable<IComparable> Do();

    IQueryDeclaration ApplyAttribute(IQueryDeclaration queryDeclaration);
}