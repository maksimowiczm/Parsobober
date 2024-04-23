using Parsobober.Pkb.Relations.Dto;

namespace Parsobober.Pkb.Relations.Abstractions.Accessors;

public interface IFollowsAccessor
{
    /// <summary>
    /// Returns the statements that FOLLOW statement with given type <typeparamref name="TStatement"/>
    /// => Follows(providedType, returned)
    /// </summary>
    /// <typeparam name="TStatement">Statement type <see cref="Statement"/></typeparam>
    IEnumerable<Statement> GetFollowers<TStatement>() where TStatement : Statement;

    /// <summary>
    /// Returns the statement which FOLLOWS statement with given line number
    /// => Follows(<paramref name="lineNumber"/>, returned)
    /// </summary>
    Statement? GetFollower(int lineNumber);

    /// <summary>
    /// Returns the statements that are FOLLOWED BY statement with given type <typeparamref name="TStatement"/>
    /// => Follows(returned, providedType)
    /// </summary>
    /// <typeparam name="TStatement">Statement type <see cref="Statement"/></typeparam>
    IEnumerable<Statement> GetFollowed<TStatement>() where TStatement : Statement;

    /// <summary>
    /// Returns the statement which are FOLLOWED BY statement with given line number
    /// => Follows(returned, <paramref name="lineNumber"/>)
    /// </summary>
    Statement? GetFollowed(int lineNumber);

    /// <summary>
    /// Returns the statements that FOLLOW TRANSITIVELY statement with given type <typeparamref name="TStatement"/>
    /// => Follows*(providedType, returned)
    /// </summary>
    /// <typeparam name="TStatement">Statement type <see cref="Statement"/></typeparam>
    IEnumerable<Statement> GetFollowersTransitive<TStatement>() where TStatement : Statement;

    /// <summary>
    /// Returns the statements that FOLLOW TRANSITIVELY statement with given line number
    /// => Follows*(<paramref name="lineNumber"/>, returned)
    /// </summary>
    IEnumerable<Statement> GetFollowersTransitive(int lineNumber);

    /// <summary>
    /// Returns the statements that are FOLLOWED BY TRANSITIVELY statement with given type <typeparamref name="TStatement"/>
    /// => Follows*(returned, providedType)
    /// </summary>
    /// <typeparam name="TStatement">Statement type <see cref="Statement"/></typeparam>
    IEnumerable<Statement> GetFollowedTransitive<TStatement>() where TStatement : Statement;

    /// <summary>
    /// Returns the statements that are FOLLOWED BY TRANSITIVELY statement with given line number
    /// => Follows*(returned, <paramref name="lineNumber"/>)
    /// </summary>
    IEnumerable<Statement> GetFollowedTransitive(int lineNumber);
}