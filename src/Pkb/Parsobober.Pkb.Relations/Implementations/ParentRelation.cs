using Microsoft.Extensions.Logging;
using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Abstractions.Creators;

namespace Parsobober.Pkb.Relations.Implementations;

public class ParentRelation(
    ILogger<ParentRelation> logger,
    IProgramContextAccessor programContext
) : IParentCreator
{
    /// <summary>
    /// Stores parent relation between two statements using their line numbers.
    /// </summary>
    /// <remarks>[parent node line number, child node line number].</remarks>
    private readonly Dictionary<int, int> _parentsDictionary = new();

    public void SetParent(TreeNode parentNode, TreeNode childNode)
    {
        if (parentNode.Type.IsContainerStatement() == false || childNode.Type.IsStatement() == false)
        {
            logger.LogError(
                "Parent relation can only be established between container statement and statement node. ({parent} => {child})",
                parentNode, childNode);

            throw new ArgumentException(
                $"At least one of provided nodes type: {parentNode.Type}, {childNode.Type} is different than any of required: {EntityType.Statement} types.");
        }

        if (!_parentsDictionary.TryAdd(parentNode.LineNumber, childNode.LineNumber))
        {
            logger.LogError("Relation {parent} parents {child} already exists",
                parentNode.LineNumber, childNode.LineNumber);
        }
    }
}