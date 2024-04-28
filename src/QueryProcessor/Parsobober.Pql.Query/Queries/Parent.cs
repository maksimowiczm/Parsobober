using System.Collections;
using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Shared;

namespace Parsobober.Pql.Query.Queries;

internal static class Parent
{
    #region Builder

    public class Builder(IParentAccessor accessor)
    {
        private readonly List<(string parent, string child)> _parentRelations = [];

        public void Add(string parent, string child)
        {
            _parentRelations.Add((parent, child));
        }

        public IEnumerable<Statement> Build(string select, IReadOnlyDictionary<string, Type> declarations)
        {
            // todo aktualnie działa tylko dla jednego parenta i nie bierze pod uwagę atrybutów

            var parent = _parentRelations.First().parent;
            var child = _parentRelations.First().child;

            var query = new InnerBuilder(accessor, select, declarations).Build(parent, child);

            return query;
        }
    }

    private class InnerBuilder(IParentAccessor accessor, string select, IReadOnlyDictionary<string, Type> declarations)
    {
        public IEnumerable<Statement> Build(string parentStr, string childStr)
        {
            // parsowanie argumentów
            var nullableParentLine = parentStr.ParseOrNull<int>();
            var nullableChildLine = childStr.ParseOrNull<int>();

            var nullableParentType = declarations.GetValueOrDefault(parentStr);
            var nullableChildType = declarations.GetValueOrDefault(childStr);

            // pattern matching argumentów
            var query = ((nullableParentType, nullableParentLine), (nullableChildType, nullableChildLine)) switch
            {
                // Parent(T, T)
                (({ } parentType, null), ({ } childType, null)) =>
                    BuildParentWithSelect((parentStr, parentType), (childStr, childType)),
                // Parent(T, 1)
                (({ } parentType, null), (null, { } childLine)) =>
                    Activator.CreateInstance(
                        typeof(GetParentByLineNumber<>).MakeGenericType(parentType),
                        accessor,
                        childLine
                    ),
                // Parent(1, T)
                ((null, { } parentLine), ({ } childType, null)) =>
                    Activator.CreateInstance(
                        typeof(GetChildrenByLineNumber<>).MakeGenericType(childType),
                        accessor,
                        parentLine
                    ),
                // Parent(1, 2) nie wspierane w tej wersji
                _ => throw new InvalidOperationException("Invalid query")
            } as IEnumerable<Statement>;

            return query!;
        }

        private IEnumerable<Statement> BuildParentWithSelect(
            (string key, Type type) parent,
            (string key, Type type) child
        )
        {
            // tu nastąpi samowywrotka przy zapytaniach, w których nie ma wartości z selecta
            // przykład: Select x such that Parent(a, b)

            // sprawdzamy o co pytamy
            var queryType = (parent.key == select) switch
            {
                // pytam o rodziców Parent(to chce, to mam)
                true => typeof(GetParentsByChildType<,>).MakeGenericType([parent.type, child.type]),
                // pytam o dzieci Parent(to mam, to chce)
                false => typeof(GetChildrenByParentType<,>).MakeGenericType([parent.type, child.type])
            };

            var query = Activator.CreateInstance(queryType, accessor) as IEnumerable<Statement>;
            return query!;
        }
    }

    #endregion

    #region Queries

    /// <summary>
    /// Gets parents of given type by child type.
    /// </summary>
    /// <param name="parentAccessor">Parent accessor.</param>
    /// <typeparam name="TParent">Parent type.</typeparam>
    /// <typeparam name="TChild">Child type.</typeparam>
    private class GetParentsByChildType<TParent, TChild>(IParentAccessor parentAccessor)
        : ParentBase<TParent>(parentAccessor)
        where TParent : Statement
        where TChild : Statement
    {
        protected override IEnumerable<TParent> Query(IParentAccessor accessor) =>
            accessor.GetParents<TChild>().OfType<TParent>();
    }

    /// <summary>
    /// Get parent of given type by child line number.
    /// </summary>
    /// <param name="parentAccessor">Parent accessor.</param>
    /// <param name="line">Line number.</param>
    /// <typeparam name="TParent">Parent type.</typeparam>
    private class GetParentByLineNumber<TParent>(IParentAccessor parentAccessor, int line)
        : ParentBase<TParent>(parentAccessor)
        where TParent : Statement
    {
        protected override IEnumerable<TParent> Query(IParentAccessor accessor)
        {
            if (accessor.GetParent(line) is not TParent parent)
            {
                return Enumerable.Empty<TParent>();
            }

            return Enumerable.Repeat(parent, 1);
        }
    }

    /// <summary>
    /// Gets children of given type by parent line number.
    /// </summary>
    /// <param name="parentAccessor">Parent accessor.</param>
    /// <param name="line">Line number.</param>
    /// <typeparam name="TChild">Child type.</typeparam>
    private class GetChildrenByLineNumber<TChild>(IParentAccessor parentAccessor, int line)
        : ParentBase<TChild>(parentAccessor)
        where TChild : Statement
    {
        protected override IEnumerable<TChild> Query(IParentAccessor accessor) =>
            accessor.GetChildren(line).OfType<TChild>();
    }

    /// <summary>
    /// Gets children of given type by parent type.
    /// </summary>
    /// <param name="parentAccessor">Parent accessor.</param>
    /// <typeparam name="TParent">Parent type.</typeparam>
    /// <typeparam name="TChild">Child type.</typeparam>
    private class GetChildrenByParentType<TParent, TChild>(IParentAccessor parentAccessor)
        : ParentBase<TChild>(parentAccessor)
        where TChild : Statement
        where TParent : Statement
    {
        protected override IEnumerable<TChild> Query(IParentAccessor accessor) =>
            accessor.GetChildren<TParent>().OfType<TChild>();
    }

    private abstract class ParentBase<TOut>(IParentAccessor parentAccessor) : IEnumerable<TOut>
    {
        protected abstract IEnumerable<TOut> Query(IParentAccessor accessor);

        public IEnumerator<TOut> GetEnumerator() => Query(parentAccessor).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    #endregion
}