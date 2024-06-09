using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Queries.Core;
using Parsobober.Pql.Query.Queries.Exceptions;

namespace Parsobober.Pql.Query.Queries;

internal class NextTransitive(IArgument left, IArgument right, INextAccessor accessor)
    : ReplaceableArgumentQueryDeclaration<NextTransitive>, IQueryDeclaration
{
    public override IArgument Left { get; } = left;
    public override IArgument Right { get; } = right;

    public override IEnumerable<IPkbDto> Do()
    {
        var query = (Left, Right) switch
        {
            (Line left, Line right) => IPkbDto.Boolean(accessor.IsNextTransitive(left.Value, right.Value)),
            (Any, _) or (_, Any) => HandleAny(),
            _ => DoDeclaration()
        };

        return query;
    }

    private IEnumerable<IPkbDto> HandleAny() => (Left, Right) switch
    {
        (Line previous, Any) => accessor.GetNextTransitive(previous.Value),
        (Any, Line next) => accessor.GetPreviousTransitive(next.Value),
        (Any, Any) => IPkbDto.Boolean(accessor.ProcedureCfgs.Values.Any(d => d.EntryNode.Next.Any())),
        (IOtherDeclaration.ProgramLine, Any) => accessor.GetAllPreviousWithNext(),
        (Any, IOtherDeclaration.ProgramLine) => accessor.GetAllNextWithPrevious(),
        _ => Enumerable.Empty<Statement>()
    };

    public override IEnumerable<IPkbDto> Do(IDeclaration select)
    {
        var query = (Left, Right) switch
        {
            (IOtherDeclaration.ProgramLine, Line right) => accessor.GetPreviousTransitive(right.Value),

            (Line left, IOtherDeclaration.ProgramLine) => accessor.GetNextTransitive(left.Value),

            _ => throw new QueryNotSupported(this, $"Next({Left}, {Right}) is not supported.")
        };

        return query;
    }

    protected override NextTransitive CloneSelf(IArgument left, IArgument right) => new(left, right, accessor);
}