using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Organizer.Obsolete.Tree.Abstraction;

[Obsolete("Obsolete query organizer. Use Parsobober.Pql.Query.Organizer instead.")]
public interface IQueryContainer
{
    int Count { get; }

    IEnumerable<IQueryDeclaration> Declarations { get; }

    IEnumerable<IAttributeQuery> AttributeQueries { get; }

    /// <summary>
    /// Get query with given declaration.
    /// </summary>
    IQueryDeclaration? Get(IDeclaration declaration);

    /// <summary>
    /// Get any query.
    /// </summary>
    IQueryDeclaration GetAny();

    /// <summary>
    /// Gets argument from one of query declarations.
    /// </summary>
    IDeclaration? GetDeclaration();

    /// <summary>
    /// Check if there is any query with given declaration.
    /// </summary>
    /// <returns> True if there is any query with given declaration, false otherwise. </returns>
    bool HasQueryWith(IDeclaration declaration);

    void Remove(IQueryDeclaration query);
}