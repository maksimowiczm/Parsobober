using Parsobober.Pql.Query.Abstractions;
using Parsobober.Pql.Query.Arguments;

namespace Parsobober.Pql.Query.Organizer.Obsolete.Tree.Exceptions;

public abstract class QueryNodeException(string message) : QueryEvaluatorException(message);

internal class DeclarationNotSupported(IDeclaration declaration, string message) : QueryNodeException(message)
{
    public IDeclaration Declaration { get; } = declaration;
}