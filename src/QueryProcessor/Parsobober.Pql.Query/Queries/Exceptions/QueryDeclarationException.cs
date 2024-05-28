using Parsobober.Pql.Query.Abstractions;
using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Queries.Exceptions;

public abstract class QueryDeclarationException(string message) : QueryEvaluatorException(message);

internal class AmbiguousQueryDeclarationException(IQueryDeclaration declaration)
    : QueryDeclarationException($"{declaration} is ambiguous. You have to specify which declaration you want to use.");