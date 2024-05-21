using Parsobober.Pql.Query.Arguments;

namespace Parsobober.Pql.Query.Queries.Abstractions;

internal interface IQueryDeclaration : ILazyQuery
{
    IArgument Left { get; }

    IArgument Right { get; }
}