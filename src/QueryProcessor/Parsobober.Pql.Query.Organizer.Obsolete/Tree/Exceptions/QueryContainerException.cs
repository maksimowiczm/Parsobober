using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query.Organizer.Obsolete.Tree.Exceptions;

public class QueryContainerException(string message) : QueryEvaluatorException(message);

internal class DeclarationNotFoundException() : QueryContainerException("Declaration not found in any query.");