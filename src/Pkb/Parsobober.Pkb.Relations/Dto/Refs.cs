namespace Parsobober.Pkb.Relations.Dto;

public static class Refs
{
    public record Variable(string? Name = null);

    public record Constant(string? Value = null);
}

