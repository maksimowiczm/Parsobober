using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query.Organizer.Obsolete.Tree.Exceptions;

public class QueryOrganizerException(string message) : QueryEvaluatorException(message);

internal class NotAllRelationsUsedException() : QueryOrganizerException("Not all relations were used in query.");

internal class NotAllAttributesUsedException() : QueryOrganizerException("Not all attributes were used in query.");