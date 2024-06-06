using System.Collections;

namespace Parsobober.Pkb.Relations.Dto;

public class PkbDtoComparer : IEqualityComparer<IPkbDto>, IComparer<IPkbDto>
{
    // equal
    public bool Equals(IPkbDto? xdto, IPkbDto? ydto)
    {
        if (xdto is null || ydto is null)
            return false;

        var result = (xdto, ydto) switch
        {
            (Statement x, Statement y) => x.Line == y.Line,
            (Statement x, ProgramLine y) => x.Line == y.Line,
            (ProgramLine x, Statement y) => x.Line == y.Line,
            (Variable x, Variable y) => x.Name == y.Name,
            (Procedure x, Procedure y) => x.Name == y.Name,
            _ => false
        };

        return result;
    }

    public int GetHashCode(IPkbDto obj) => 0;

    // order
    public int Compare(IPkbDto? xdto, IPkbDto? ydto)
    {
        if (xdto is null || ydto is null)
            return 0;

        var result = (xdto, ydto) switch
        {
            (Statement x, Statement y) =>
                x.Line.CompareTo(y.Line),
            (Statement x, ProgramLine y) =>
                x.Line.CompareTo(y.Line),
            (ProgramLine x, Statement y) =>
                x.Line.CompareTo(y.Line),
            (Variable x, Variable y) =>
                string.Compare(x.Name, y.Name, StringComparison.Ordinal),
            (Procedure x, Procedure y) =>
                string.Compare(x.Name, y.Name, StringComparison.Ordinal),
            (Constant x, Constant y) =>
                x.Value.CompareTo(y.Value),
            (ProgramLine x, ProgramLine y) =>
                x.Line.CompareTo(y.Line),
            (StatementList x, StatementList y) =>
                x.Line.CompareTo(y.Line),
            _ => 0
        };

        return result;
    }
}