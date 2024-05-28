using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query.Tree.Exceptions;

public class QueryContainerException(string message) : QueryEvaluatorException(message);

internal class DeclarationNotFoundException() : QueryContainerException("Declaration not found in any query.");