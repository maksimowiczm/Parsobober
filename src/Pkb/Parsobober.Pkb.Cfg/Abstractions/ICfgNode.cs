namespace Parsobober.Pkb.Cfg.Abstractions;

public interface ICfgNode
{
    int LineNumber { get; }
    IEnumerable<ICfgNode> Previous { get; }
    IEnumerable<ICfgNode> Next { get; }
    void AddNext(ICfgNode nextNode);
}