namespace Parsobober.Pql.Query.Arguments.Exceptions;

internal class DeclarationParseException(string type, string name)
    : ArgumentException($"Declaration '{type} {name}' is not supported.");