using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query.Arguments.Exceptions;

public abstract class ArgumentException(string message) : QueryEvaluatorException(message);