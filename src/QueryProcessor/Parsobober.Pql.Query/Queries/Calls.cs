using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Queries.Core;
using Parsobober.Pql.Query.Queries.Exceptions;

namespace Parsobober.Pql.Query.Queries;

internal static class Calls
{
    public class QueryDeclaration(
        IArgument caller,
        IArgument called,
        ICallsAccessor accessor,
        IDtoProgramContextAccessor context
    ) : ReplaceableArgumentQueryDeclaration<QueryDeclaration>, IQueryDeclaration
    {
        public override IArgument Left { get; } = caller;
        public override IArgument Right { get; } = called;

        public override IEnumerable<IPkbDto> Do()
        {
            var query = (Left, Right) switch
            {
                // Calls('name', 'name')
                (Name left, Name right) => new BooleanCallsQuery(accessor, left.Value, right.Value).Build(),
                (Any, _) or (_, Any) => HandleAny(),
                _ => DoDeclaration()
            };

            return query;
        }

        private IEnumerable<IPkbDto> HandleAny() => (Left, Right) switch
        {
            (Name procedure, Any) => accessor.GetCalled(procedure.Value),
            (Any, Name procedure) => accessor.GetCallers(procedure.Value),
            (Any, Any) => IPkbDto.Boolean(context.Calls.Any()),
            _ => Enumerable.Empty<Statement>()
        };

        public override IEnumerable<IPkbDto> Do(IDeclaration select)
        {
            var query = (Left, Right) switch
            {
                // Calls(proc, 'name')
                (IProcedureDeclaration declaration, Name called) =>
                    new GetCallersByCalledName(accessor, called.Value).Build(),

                // Calls('name', proc)
                (Name caller, IProcedureDeclaration declaration) =>
                    new GetCalledByCallerName(accessor, caller.Value).Build(),

                // Calls(proc, proc)
                (IProcedureDeclaration caller, IProcedureDeclaration called) => BuildCallsWithSelect(caller, called),

                _ => throw new QueryNotSupported(this, $"Calls({Left}, {Right}) is not supported.")
            };

            return query;

            IEnumerable<IPkbDto> BuildCallsWithSelect(IProcedureDeclaration caller, IProcedureDeclaration called)
            {
                if (caller == select)
                {
                    return new GetCallers(accessor).Build();
                }

                if (called == select)
                {
                    return new GetCalled(accessor).Build();
                }

                throw new DeclarationNotFoundException(select, this);
            }
        }

        protected override QueryDeclaration CloneSelf(IArgument left, IArgument right) =>
            new(left, right, accessor, context);
    }

    #region Queries

    /// <summary>
    /// Get procedures that call given procedure.
    /// </summary>
    /// <param name="callsAccessor">Calls accessor.</param>
    /// <param name="callerName">Caller procedure name.</param>
    private class GetCalledByCallerName(ICallsAccessor callsAccessor, string callerName) : CallsQuery
    {
        public override IEnumerable<IPkbDto> Build() =>
            callsAccessor.GetCalled(callerName);
    }

    /// <summary>
    /// Get procedures that call given procedure.
    /// </summary>
    /// <param name="callsAccessor">Calls accessor.</param>
    /// <param name="calledName">Called procedure name.</param>
    private class GetCallersByCalledName(ICallsAccessor callsAccessor, string calledName) : CallsQuery
    {
        public override IEnumerable<IPkbDto> Build() =>
            callsAccessor.GetCallers(calledName);
    }

    /// <summary>
    /// Get procedures that call any procedure.
    /// </summary>
    /// <param name="callsAccessor">Calls accessor.</param>
    private class GetCallers(ICallsAccessor callsAccessor) : CallsQuery
    {
        public override IEnumerable<IPkbDto> Build() =>
            callsAccessor.GetCallers();
    }

    /// <summary>
    /// Get procedures that are called by any procedure.
    /// </summary>
    /// <param name="callsAccessor">Calls accessor.</param>
    private class GetCalled(ICallsAccessor callsAccessor) : CallsQuery
    {
        public override IEnumerable<IPkbDto> Build() =>
            callsAccessor.GetCalled();
    }

    /// <summary>
    /// Check if caller calls called.
    /// </summary>
    /// <param name="callsAccessor">Calls accessor.</param>
    /// <param name="callerName">Caller procedure name.</param>
    /// <param name="calledName">Called procedure name.</param>
    private class BooleanCallsQuery(ICallsAccessor callsAccessor, string callerName, string calledName) : CallsQuery
    {
        public override IEnumerable<IPkbDto> Build() => IPkbDto.Boolean(callsAccessor.IsCalled(callerName, calledName));
    }

    /// <summary>
    /// Represents a calls query.
    /// </summary>
    private abstract class CallsQuery
    {
        /// <summary>
        /// Builds a query.
        /// </summary>
        /// <returns> The query. </returns>
        public abstract IEnumerable<IPkbDto> Build();
    }

    #endregion
}