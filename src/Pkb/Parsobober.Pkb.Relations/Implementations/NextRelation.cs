using Parsobober.Pkb.Cfg.Core;
using Parsobober.Pkb.Cfg.Strategies;
using Parsobober.Pkb.Relations.Abstractions;
using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pkb.Relations.Utilities;

namespace Parsobober.Pkb.Relations.Implementations;

public class NextRelation(
    IProgramContextAccessor programContext
) : IPostParseComputable, INextAccessor
{
    private readonly Dictionary<string, ProcedureCfg> _procedureCfgsDictionary = new();

    public void Compute()
    {
        foreach (var (procedureName, procedureNode) in programContext.ProceduresDictionary)
        {
            _procedureCfgsDictionary.Add(procedureName, new ProcedureCfg(procedureNode));
        }
    }

    public IEnumerable<ProgramLine> GetPrevious(int line)
    {
        foreach (var (_, cfg) in _procedureCfgsDictionary)
        {
            var cfgNode = cfg.FindByLineNumber(line);
            if (cfgNode is null)
            {
                continue;
            }

            return cfgNode.Previous.Select(node => node.LineNumber.ToProgramLine());
        }

        return Enumerable.Empty<ProgramLine>();
    }

    public IEnumerable<ProgramLine> GetNext(int line)
    {
        foreach (var (_, cfg) in _procedureCfgsDictionary)
        {
            var cfgNode = cfg.FindByLineNumber(line);
            if (cfgNode is null)
            {
                continue;
            }

            return cfgNode.Next.Select(node => node.LineNumber.ToProgramLine());
        }

        return Enumerable.Empty<ProgramLine>();
    }

    public bool IsNext(int left, int right)
    {
        foreach (var (_, cfg) in _procedureCfgsDictionary)
        {
            var cfgNode = cfg.FindByLineNumber(left);
            if (cfgNode is null)
            {
                continue;
            }

            return cfgNode.Next.Any(node => node.LineNumber == right);
        }

        return false;
    }

    public IEnumerable<ProgramLine> GetPreviousTransitive(int line)
    {
        foreach (var (_, cfg) in _procedureCfgsDictionary)
        {
            var cfgNode = cfg.FindByLineNumber(line);
            if (cfgNode is null)
            {
                continue;
            }

            return cfg.Search(cfgNode, new IterativeDfsUp())
                .Select(node => node.LineNumber.ToProgramLine());
        }

        return Enumerable.Empty<ProgramLine>();
    }

    public IEnumerable<ProgramLine> GetNextTransitive(int line)
    {
        foreach (var (_, cfg) in _procedureCfgsDictionary)
        {
            var cfgNode = cfg.FindByLineNumber(line);
            if (cfgNode is null)
            {
                continue;
            }

            return cfg.Search(cfgNode, new IterativeDfs())
                .Select(node => node.LineNumber.ToProgramLine());
        }

        return Enumerable.Empty<ProgramLine>();
    }

    public bool IsNextTransitive(int left, int right) =>
        GetNextTransitive(left).Any(programLine => programLine.Line == right);
}