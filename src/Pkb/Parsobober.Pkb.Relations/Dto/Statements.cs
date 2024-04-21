using Parsobober.Pkb.Relations.Abstractions.Accessors;

namespace Parsobober.Pkb.Relations.Dto;

public abstract record Statement(int LineNumber) : IModifiesAccessor.IRequest, IUsesAccessor.IRequest;

public record Assign(int LineNumber) : Statement(LineNumber);

public record If(int LineNumber) : Statement(LineNumber);

public record While(int LineNumber) : Statement(LineNumber);