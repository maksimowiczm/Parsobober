using Parsobober.Pkb.Relations.Dto;

namespace Parsobober.Pkb.Relations.Abstractions.Accessors;

public interface IDtoProgramContextAccessor
{
    IEnumerable<Statement> Statements { get; }
    IEnumerable<Assign> Assigns { get; }
    IEnumerable<While> Whiles { get; }
    IEnumerable<If> Ifs { get; }
    IEnumerable<Call> Calls { get; }
    IEnumerable<Variable> Variables { get; }
    IEnumerable<Procedure> Procedures { get; }
    IEnumerable<Statement> StatementLists { get; }
    IEnumerable<IComparable> Constants { get; }
    IEnumerable<IComparable> ProgramLines { get; }
}