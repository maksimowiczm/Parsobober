using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Queries.Core;

internal class QueryDeclarationComparer : IComparer<IQueryDeclaration>
{
    public int Compare(IQueryDeclaration? x, IQueryDeclaration? y)
    {
        return 1;
    }
}