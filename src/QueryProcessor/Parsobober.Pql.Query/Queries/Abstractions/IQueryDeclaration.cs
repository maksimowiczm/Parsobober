using Parsobober.Pql.Query.Arguments;

namespace Parsobober.Pql.Query.Queries.Abstractions;

internal interface IQueryDeclaration
{
    IArgument Left { get; }

    IArgument Right { get; }

    /// <summary>
    /// Do the query using select.
    /// </summary>
    IEnumerable<IComparable> Do(IDeclaration select);

    /// <summary>
    /// Do the query without specifying left or right side.
    /// </summary>
    IEnumerable<IComparable> Do() => DoLeft();

    /// <summary>
    /// If able do the query using left side. If not try to do it using right side.
    /// If both sides are not able to do the query, return empty list.
    /// </summary>
    /// <returns></returns>
    IEnumerable<IComparable> DoLeft() =>
        (Left, Right) switch
        {
            (Left: IDeclaration left, Right: not IDeclaration) => Do(left),
            (Left: not IDeclaration, Right: IDeclaration right) => Do(right),
            (Left: IDeclaration left, Right: IDeclaration) => Do(left),
            _ => []
        };
}