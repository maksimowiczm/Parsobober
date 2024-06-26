using Parsobober.Pkb.Cfg.Core;
using Parsobober.Pkb.Relations.Dto;

namespace Parsobober.Pkb.Relations.Abstractions.Accessors;

public interface INextAccessor
{
    public IReadOnlyDictionary<string, ProcedureCfg> ProcedureCfgs { get; }

    IEnumerable<ProgramLine> GetPrevious(int line);

    IEnumerable<ProgramLine> GetNext(int line);

    bool IsNext(int left, int right);

    IEnumerable<ProgramLine> GetPreviousTransitive(int line);

    IEnumerable<ProgramLine> GetNextTransitive(int line);

    bool IsNextTransitive(int left, int right);

    IEnumerable<ProgramLine> GetAllNextWithPrevious();

    IEnumerable<ProgramLine> GetAllPreviousWithNext();
}