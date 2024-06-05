using System.Collections;

namespace Parsobober.Pkb.Relations.Dto;

public class PkbDtoComparer : IEqualityComparer<IPkbDto>, IComparer<IPkbDto>
{
    public bool Equals(IPkbDto? x, IPkbDto? y)
    {
        if (x is null || y is null)
            return false;

        var result = (x, y) switch
        {
            (Statement xStatement, Statement yStatement) => xStatement.Line == yStatement.Line,
            (Statement xStatement, ProgramLine yProgramLine) => xStatement.Line == yProgramLine.Line,
            (ProgramLine xProgramLine, Statement yStatement) => xProgramLine.Line == yStatement.Line,
            (Variable xVariable, Variable yVariable) => xVariable.Name == yVariable.Name,
            (Procedure xProcedure, Procedure yProcedure) => xProcedure.Name == yProcedure.Name,
            _ => false
        };

        return result;
    }

    public int GetHashCode(IPkbDto obj) => obj.GetHashCode();

    public int Compare(IPkbDto? x, IPkbDto? y)
    {
        if (x is null || y is null)
            return 0;

        var result = (x, y) switch
        {
            (Statement xStatement, Statement yStatement) =>
                xStatement.Line.CompareTo(yStatement.Line),
            (Statement xStatement, ProgramLine yProgramLine) =>
                xStatement.Line.CompareTo(yProgramLine.Line),
            (ProgramLine xProgramLine, Statement yStatement) =>
                xProgramLine.Line.CompareTo(yStatement.Line),
            (Variable xVariable, Variable yVariable) =>
                string.Compare(xVariable.Name, yVariable.Name, StringComparison.Ordinal),
            (Procedure xProcedure, Procedure yProcedure) =>
                string.Compare(xProcedure.Name, yProcedure.Name, StringComparison.Ordinal),
            (Constant xConstant, Constant yConstant) =>
                xConstant.Value.CompareTo(yConstant.Value),
            (ProgramLine xProgramLine, ProgramLine yProgramLine) =>
                xProgramLine.Line.CompareTo(yProgramLine.Line),
            (StatementList xStatementList, StatementList yStatementList) =>
                xStatementList.Line.CompareTo(yStatementList.Line),
            _ => 0
        };

        return result;
    }
}