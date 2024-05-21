using Parsobober.Pql.Query.Abstractions;
using Parsobober.Pql.Query.Tree.Node;

namespace Parsobober.Pql.Query.Tree;

internal static class QueryExecutor
{
    public static IQueryResult Execute(IQueryNode root)
    {
        // na pierwsza iteracje wystarczy
        return new QueryResult(root.Query.Do(root.Select));
    }
}