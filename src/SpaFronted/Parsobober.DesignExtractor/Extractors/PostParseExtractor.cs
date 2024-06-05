using Parsobober.Pkb.Relations.Abstractions;

namespace Parsobober.DesignExtractor.Extractors;

internal class PostParseExtractor(IEnumerable<IPostParseComputable> computableList) : SimpleExtractor
{
    public override void FinishedParsing()
    {
        foreach (var computable in computableList)
        {
            computable.Compute();
        }
    }
}