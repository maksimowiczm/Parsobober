using Parsobober.Pkb.Ast;
using static Parsobober.Pkb.Relations.Dto.Statements;

namespace Parsobober.Pkb.Relations.Abstractions.Accessors;

public interface IFollowsAccessor
{
    /// <summary>
    /// Returns the statement that follows the given statement => Follows(provided, returned)
    /// </summary>
    TreeNode GetFollows(Statement statement);

    /// <summary>
    /// Returns the procedure that follows the given procedure transitive => Follows*(provided, returned)
    /// </summary>
    IEnumerable<TreeNode> GetFollowsTransitive(Statement statement);
    
    /// <summary>
    /// Returns the procedure that precedes the given procedure => Follows(returned, provided)
    /// </summary>
    TreeNode GetFollowedBy(Statement statement);
    
    /// <summary>
    /// Returns the procedure that precedes the given procedure => Follows*(returned, returned)
    /// </summary>
    IEnumerable<TreeNode> GetFollowedByTransitive(Statement statement);
    
    bool IsFollowed(Statement preceding, Statement following);

    bool IsFollowedTransitive(Statement preceding, Statement following);
}