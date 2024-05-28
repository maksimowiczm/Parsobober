using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query.Queries.Exceptions;

public abstract class QueryDeclarationException(string message) : QueryEvaluatorException(message);