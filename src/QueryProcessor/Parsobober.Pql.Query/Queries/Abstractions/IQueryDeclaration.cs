using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Exceptions;

namespace Parsobober.Pql.Query.Queries.Abstractions;

public interface IQueryDeclaration
{
    IArgument Left { get; }

    IArgument Right { get; }

    /// <summary>
    /// Do the query without specifying left or right side.
    /// </summary>
    IEnumerable<IComparable> Do();

    /// <summary>
    /// Do the query using select.
    /// </summary>
    IEnumerable<IComparable> Do(IDeclaration select);

    /// <summary>
    /// Replace the argument in the query.
    /// </summary>
    /// <param name="select">The argument to replace.</param>
    /// <param name="replacement">The replacement.</param>
    /// <returns> Query with replaced argument. </returns>
    IQueryDeclaration ReplaceArgument(IDeclaration select, IArgument replacement);

    /// <summary>
    /// Get the other side of the query. If the select is not in the query, throw exception.
    /// </summary>
    /// <param name="select">The side that is the other side xD.</param>
    /// <returns>The other side of the query.</returns>
    /// <exception cref="DeclarationNotFoundException">If the select is not in the query.</exception>
    IDeclaration? GetAnotherSide(IDeclaration select)
    {
        if (select == Left)
        {
            return Right as IDeclaration;
        }

        if (select == Right)
        {
            return Left as IDeclaration;
        }

        throw new DeclarationNotFoundException(select, this);
    }
}