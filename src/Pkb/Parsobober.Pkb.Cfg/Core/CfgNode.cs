using Parsobober.Pkb.Cfg.Abstractions;

namespace Parsobober.Pkb.Cfg.Core;

internal class CfgNode : ICfgNode
{
    public int LineNumber { get; }
    public IEnumerable<ICfgNode> Previous => _previous;
    public IEnumerable<ICfgNode> Next => _next;

    private readonly HashSet<ICfgNode> _previous;
    private readonly HashSet<ICfgNode> _next;

    internal CfgNode(int lineNumber)
    {
        LineNumber = lineNumber;
        _previous = new HashSet<ICfgNode>();
        _next = new HashSet<ICfgNode>();
    }

    public void AddNext(ICfgNode nextNode)
    {
        _next.Add(nextNode);
        if (nextNode is CfgNode cfgNode)
        {
            cfgNode._previous.Add(this);
        }
    }
}