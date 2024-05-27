using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Queries.Exceptions;

internal class QueryNotSupported(IQueryDeclaration query, string message) : QueryDeclarationException(message)
{
    public IQueryDeclaration Query { get; } = query;
}