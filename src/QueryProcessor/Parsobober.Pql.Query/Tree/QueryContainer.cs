using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Tree.Abstraction;
using Parsobober.Pql.Query.Tree.Exceptions;

namespace Parsobober.Pql.Query.Tree;

internal class QueryContainer : IQueryContainer
{
    internal class QueryContainerBuilder : IQueryContainer.IQueryContainerBuilder
    {
        private readonly List<IQueryDeclaration> _queries = [];
        public void Add(IQueryDeclaration query) => _queries.Add(query);

        public IQueryContainer Build() => new QueryContainer(_queries);
    }

    private readonly List<IQueryDeclaration> _queries;

    private QueryContainer(List<IQueryDeclaration> queries)
    {
        _queries = queries;
    }

    public int Count => _queries.Count;
    public IEnumerable<IQueryDeclaration> Declarations => _queries;

    public IQueryDeclaration? Get(IDeclaration declaration) =>
        _queries.FirstOrDefault(q => q.Left == declaration || q.Right == declaration);

    public IQueryDeclaration GetAny() => _queries.First();

    public IDeclaration? GetDeclaration()
    {
        var query = _queries
            .FirstOrDefault(q => q.Left is IDeclaration || q.Right is IDeclaration);

        if (query is null)
        {
            return null;
        }

        return query switch
        {
            { Left: IDeclaration declaration } => declaration,
            { Right: IDeclaration declaration } => declaration,
            _ => throw new DeclarationNotFoundException()
        };
    }

    public bool HasQueryWith(IDeclaration declaration) =>
        _queries.Any(q => q.Left == declaration || q.Right == declaration);

    public void Remove(IQueryDeclaration query) => _queries.Remove(query);
}