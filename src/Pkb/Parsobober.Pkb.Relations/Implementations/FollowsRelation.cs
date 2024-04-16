using Microsoft.Extensions.Logging;
using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Abstractions.Creators;

namespace Parsobober.Pkb.Relations.Implementations;

public class FollowsRelation(
    ILogger<FollowsRelation> logger,
    IProgramContextAccessor programContext
) : IFollowsCreator
{
    /// <summary>
    /// Stores follows relation between two statements using their line numbers.
    /// </summary>
    /// <remarks>[following node line number, preceding node line number].</remarks>
    private readonly Dictionary<int, int> _followsDictionary = new();

    public void SetFollows(TreeNode node, TreeNode parent)
    {
        if (!node.Type.IsStatement() || !parent.Type.IsStatement())
        {
            logger.LogError(
                "Follows relation can only be established between two statement nodes. ({parent} => {child})",
                parent, node);

            throw new ArgumentException(
                $"At least one of provided nodes type: {parent.Type}, {node.Type} is different than any of required: {EntityType.Statement} types.");
        }

        if (!_followsDictionary.TryAdd(node.LineNumber, parent.LineNumber))
        {
            logger.LogWarning("Relation {node} follows {parent} already exists",
                node.LineNumber, parent.LineNumber);
        }
    }
}