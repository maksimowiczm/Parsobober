namespace Parsobober.Pql.Query.Abstractions;

public abstract class QueryEvaluatorException(string message) : Exception(message);

internal class WtfException(string message) : QueryEvaluatorException(message);