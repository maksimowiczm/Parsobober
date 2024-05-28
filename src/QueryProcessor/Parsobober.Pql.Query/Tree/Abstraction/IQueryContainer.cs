using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Tree.Abstraction;

internal interface IQueryContainer
{
    interface IQueryContainerBuilder
    {
        void Add(IQueryDeclaration query);

        IQueryContainer Build();
    }

    int Count { get; }

    /// <summary>
    /// Get query with given declaration.
    /// </summary>
    IQueryDeclaration? Get(IDeclaration declaration);

    /// <summary>
    /// Get any query.
    /// </summary>
    IQueryDeclaration GetAny();

    /// <summary>
    /// Check if there is any query with given declaration.
    /// </summary>
    /// <returns> True if there is any query with given declaration, false otherwise. </returns>
    bool HasQueryWith(IDeclaration declaration);

    void Remove(IQueryDeclaration query);
}