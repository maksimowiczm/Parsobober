using System.Collections;
using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;

namespace Parsobober.Pql.Query.Queries;

internal static class Parent
{
    /// <summary>
    /// Gets parents of given type by child type.
    /// </summary>
    /// <param name="parentAccessor">Parent accessor.</param>
    /// <typeparam name="TParent">Parent type.</typeparam>
    /// <typeparam name="TChild">Child type.</typeparam>
    public class GetParentsByChildType<TParent, TChild>(IParentAccessor parentAccessor)
        : ParentBase<TParent>(
            parentAccessor,
            ex => ex.GetParents<TChild>().OfType<TParent>()
        )
        where TParent : Statement
        where TChild : Statement;

    /// <summary>
    /// Get parent of given type by child line number.
    /// </summary>
    /// <param name="parentAccessor">Parent accessor.</param>
    /// <param name="line">Line number.</param>
    /// <typeparam name="TParent">Parent type.</typeparam>
    public class GetParentByLineNumber<TParent>(IParentAccessor parentAccessor, int line)
        : ParentBase<TParent>(
            parentAccessor,
            ex =>
            {
                if (ex.GetParent(line) is not TParent parent)
                {
                    return Enumerable.Empty<TParent>();
                }

                return Enumerable.Repeat(parent, 1);
            })
        where TParent : Statement;

    /// <summary>
    /// Gets children of given type by parent line number.
    /// </summary>
    /// <param name="parentAccessor">Parent accessor.</param>
    /// <param name="line">Line number.</param>
    /// <typeparam name="TChild">Child type.</typeparam>
    public class GetChildrenByLineNumber<TChild>(IParentAccessor parentAccessor, int line)
        : ParentBase<TChild>(
            parentAccessor,
            ex => ex.GetChildren(line).OfType<TChild>()
        )
        where TChild : Statement;

    /// <summary>
    /// Gets children of given type by parent type.
    /// </summary>
    /// <param name="parentAccessor">Parent accessor.</param>
    /// <typeparam name="TParent">Parent type.</typeparam>
    /// <typeparam name="TChild">Child type.</typeparam>
    public class GetChildrenByParentType<TParent, TChild>(IParentAccessor parentAccessor)
        : ParentBase<TChild>(
            parentAccessor,
            ex => ex.GetChildren<TParent>().OfType<TChild>()
        )
        where TChild : Statement
        where TParent : Statement;

    public abstract class ParentBase<TOut>(
        IParentAccessor parentAccessor,
        Func<IParentAccessor, IEnumerable<TOut>> query
    ) : IEnumerable<TOut>
    {
        public IEnumerator<TOut> GetEnumerator() => query(parentAccessor).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}