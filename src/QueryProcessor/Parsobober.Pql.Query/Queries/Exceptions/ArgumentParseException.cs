using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query.Queries.Exceptions;

public class ArgumentParseException(string message) : QueryEvaluatorException(message);