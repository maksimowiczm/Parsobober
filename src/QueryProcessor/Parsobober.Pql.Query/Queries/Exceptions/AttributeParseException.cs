using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query.Queries.Exceptions;

internal class AttributeParseException(string attribute) : QueryEvaluatorException($"Unknown attribute: {attribute}");