using System.Collections;
using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Abstractions;

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

        public IEnumerable<Statement> Build(string select, IReadOnlyDictionary<string, IDeclaration> declarations)
        {
            // todo aktualnie działa tylko dla jednego parenta i nie bierze pod uwagę atrybutów

            var parent = _parentRelations.First().parent;
            var child = _parentRelations.First().child;

            var query = new InnerBuilder(accessor, select, declarations).Build(parent, child);

            return query;
        }
    }

    private class InnerBuilder(
        IParentAccessor accessor,
        string select,
        IReadOnlyDictionary<string, IDeclaration> declarations
    )
    {
        public IEnumerable<Statement> Build(string parentStr, string childStr)
        {
            // parsowanie argumentów
            var parentArgument = IArgument.Parse(declarations, parentStr);
            var childArgument = IArgument.Parse(declarations, childStr);

            // pattern matching argumentów
            var query = (parentArgument, childArgument) switch
            {
                // Parent(stmt, 1)
                (IDeclaration.Statement, IArgument.Line child) =>
                    new GetParentByLineNumber<Statement>(accessor, child.Value),

                // Parent(assign, 1)
                (IDeclaration.Assign, IArgument.Line child) =>
                    new GetParentByLineNumber<Assign>(accessor, child.Value),

                // Parent(while, 1)
                (IDeclaration.While, IArgument.Line child) =>
                    new GetParentByLineNumber<While>(accessor, child.Value),

                // Parent(1, stmt)
                (IArgument.Line parent, IDeclaration.Statement) =>
                    new GetChildrenByLineNumber<Statement>(accessor, parent.Value),

                // Parent(1, assign)
                (IArgument.Line parent, IDeclaration.Assign) =>
                    new GetChildrenByLineNumber<Assign>(accessor, parent.Value),

                // Parent(1, while)
                (IArgument.Line parent, IDeclaration.While) =>
                    new GetChildrenByLineNumber<While>(accessor, parent.Value),

                // Parent(stmt, stmt)
                (IDeclaration parent, IDeclaration child) =>
                    BuildParentWithSelect((parentStr, parent), (childStr, child)),

                // Parent(1, 2) nie wspierane w tej wersji
                _ => throw new InvalidOperationException("Invalid query")
            };

            return query;
        }

        private IEnumerable<Statement> BuildParentWithSelect(
            (string key, IDeclaration type) parent,
            (string key, IDeclaration type) child
        )
        {
            // tu nastąpi samowywrotka przy zapytaniach, w których nie ma wartości z selecta
            // przykład: Select x such that Parent(a, b)

            var parentDto = parent.type.ToDtoStatementType();
            var childDto = child.type.ToDtoStatementType();

            // sprawdzamy o co pytamy
            var queryType = (parent.key == select) switch
            {
                // pytam o rodziców Parent(to chce, to mam)
                true => typeof(GetParentsByChildType<,>).MakeGenericType([parentDto, childDto]),
                // pytam o dzieci Parent(to mam, to chce)
                false => typeof(GetChildrenByParentType<,>).MakeGenericType([parentDto, childDto])
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