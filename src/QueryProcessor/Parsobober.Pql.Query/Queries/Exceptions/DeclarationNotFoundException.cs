using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Queries.Exceptions;

internal class DeclarationNotFoundException(IDeclaration declaration, IQueryDeclaration query)
    : QueryDeclarationException($"{declaration} not found in {query}.")
{
#if DEBUG
    public IDeclaration Declaration { get; } = declaration;
    public IQueryDeclaration Query { get; } = query;
#endif
}