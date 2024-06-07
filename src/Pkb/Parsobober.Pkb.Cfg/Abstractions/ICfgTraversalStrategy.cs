namespace Parsobober.Pkb.Cfg.Abstractions;

public interface ICfgTraversalStrategy
{
    IEnumerable<ICfgNode> Traverse(ICfgNode startNode);
}