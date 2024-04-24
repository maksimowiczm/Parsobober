using Parsobober.Pkb.Relations.Dto;

namespace Parsobober.Pkb.Relations.Abstractions.Accessors;

public interface IParentAccessor
{
    /// <summary>
    /// Returns the statements that are children of statement with given type <typeparamref name="TParentStatement"/>
    /// => Parent(providedType, returned)
    /// </summary>
    /// <typeparam name="TParentStatement">Parent statement type <see cref="Statement"/></typeparam>
    IEnumerable<Statement> GetChildren<TParentStatement>() where TParentStatement : Statement;

    /// <summary>
    /// Returns the statements that are children of statement with given line number
    /// => Parent(<paramref name="lineNumber"/>, returned)
    /// </summary>
    IEnumerable<Statement> GetChildren(int lineNumber);

    /// <summary>
    /// Returns the statements that are parents of statement with given type <typeparamref name="TChildStatement"/>
    /// => Parent(returned, providedType)
    /// </summary>
    /// <typeparam name="TChildStatement">Statement type <see cref="Statement"/></typeparam>
    IEnumerable<Statement> GetParents<TChildStatement>() where TChildStatement : Statement;

    /// <summary>
    /// Returns the statement that is parent of statement with given line number
    /// => Parent(returned, <paramref name="lineNumber"/>)
    /// </summary>
    Statement? GetParent(int lineNumber);

    /// <summary>
    /// Returns the statements that are transitive children of statement with given type <typeparamref name="TParentStatement"/>
    /// => Parent*(providedType, returned)
    /// </summary>
    /// <typeparam name="TParentStatement">Statement type <see cref="Statement"/></typeparam>
    IEnumerable<Statement> GetChildrenTransitive<TParentStatement>() where TParentStatement : Statement;

    /// <summary>
    /// Returns the statements that transitive children of statement with given line number
    /// => Parent*(<paramref name="lineNumber"/>, returned)
    /// </summary>
    IEnumerable<Statement> GetChildrenTransitive(int lineNumber);

    /// <summary>
    /// Returns the statements that are transitive parents of statement with given type <typeparamref name="TChildStatement"/>
    /// => Parent*(returned, providedType)
    /// </summary>
    /// <typeparam name="TChildStatement">Statement type <see cref="Statement"/></typeparam>
    IEnumerable<Statement> GetParentsTransitive<TChildStatement>() where TChildStatement : Statement;

    /// <summary>
    /// Returns the statements that are transitive parents of statement with given line number
    /// => Parent*(returned, <paramref name="lineNumber"/>)
    /// </summary>
    IEnumerable<Statement> GetParentsTransitive(int lineNumber);
}