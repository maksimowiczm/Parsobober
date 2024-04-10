namespace Parsobober.Pkb.Relations.Dto;

public static class Statements
{
    public record Statement(int? LineNumber = null);

    public record Assign(int? LineNumber = null) : Statement(LineNumber);
    
    public record If(int? LineNumber = null) : Statement(LineNumber);

    public record While(int? LineNumber = null) : Statement(LineNumber);
    
    public record Call(int? LineNumber = null, string? ProcName = null) : Statement(LineNumber);
}