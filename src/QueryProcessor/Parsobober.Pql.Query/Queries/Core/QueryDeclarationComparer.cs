using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Queries.Core;

internal class QueryDeclarationComparer : IComparer<IQueryDeclaration>
{
    public int Compare(IQueryDeclaration? x, IQueryDeclaration? y)
    {
        if (x == null || y == null)
            return 0;

        var xRating = RateQuery(x);
        var yRating = RateQuery(y);

        return xRating - yRating;
    }

    /// <summary>
    /// Returns a rating for the query. The higher the rating, the more complex the query is.
    /// </summary>
    private static int RateQuery(IQueryDeclaration query) =>
        query switch
        {
            { Left: not IDeclaration, Right: not IDeclaration } => RateTwoArguments(query),
            { Left: IDeclaration, Right: not IDeclaration } or
                { Left: not IDeclaration, Right: IDeclaration } => RateTwoDeclarations(query),
            _ => RateTwoArguments(query),
        };

    /// <summary>
    /// Rates a query with two arguments, example Parent(1, 2).
    /// </summary>
    private static int RateTwoArguments(IQueryDeclaration query) => 0;

    /// <summary>
    /// Rates a query with one argument, example Parent(s, 1).
    /// </summary>
    private static int RateOneArgument(IQueryDeclaration query) => 0;

    /// <summary>
    /// Rates a query with two declarations, example Parent(s, a).
    /// </summary>
    private static int RateTwoDeclarations(IQueryDeclaration query) => 0;
}