using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Queries.Core;
using Parsobober.Pql.Query.Queries.Exceptions;

namespace Parsobober.Pql.Query.Queries;

internal static class CallsTransitive
{
    public class QueryDeclaration(IArgument caller, IArgument called, ICallsAccessor accessor)
        : ReplaceableArgumentQueryDeclaration<QueryDeclaration>, IQueryDeclaration
    {
        public override IArgument Left { get; } = caller;
        public override IArgument Right { get; } = called;

        public override IEnumerable<IPkbDto> Do()
        {
            var query = (Left, Right) switch
            {
                // Calls*('name', 'name')
                (Name left, Name right) =>
                    new BooleanCallsTransitiveQuery(accessor, left.Value, right.Value).Build(),

                _ => DoDeclaration()
            };

            return query;
        }

        public override IEnumerable<IPkbDto> Do(IDeclaration select)
        {
            // pattern matching argumentów
            var query = (Left, Right) switch
            {
                // Calls*(proc, 'name')
                (IProcedureDeclaration declaration, Name called) =>
                    new GetCallersTransitiveByCalledName(accessor, called.Value).Build(),

                // Calls*('name', proc)
                (Name caller, IProcedureDeclaration declaration) =>
                    new GetCalledTransitiveByCallerName(accessor, caller.Value).Build(),

                // Calls*(proc, proc)
                (IProcedureDeclaration caller, IProcedureDeclaration called) => BuildCallsTransitiveWithSelect(caller,
                    called),


                _ => throw new QueryNotSupported(this, $"Calls({Left}, {Right}) is not supported.")
            };

            return query;

            IEnumerable<IPkbDto> BuildCallsTransitiveWithSelect(IProcedureDeclaration caller,
                IProcedureDeclaration called)
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

        protected override QueryDeclaration CloneSelf(IArgument left, IArgument right) => new(left, right, accessor);
    }

    #region Queries

    /// <summary>
    /// Get procedures that call transitively given procedure.
    /// </summary>
    /// <param name="callsAccessor">Calls accessor.</param>
    /// <param name="callerName">Caller procedure name.</param>
    private class GetCalledTransitiveByCallerName(ICallsAccessor callsAccessor, string callerName)
        : CallsTransitiveQuery
    {
        public override IEnumerable<IPkbDto> Build() =>
            callsAccessor.GetCalledTransitive(callerName);
    }

    /// <summary>
    /// Get procedures that call transitively given procedure.
    /// </summary>
    /// <param name="callsAccessor">Calls accessor.</param>
    /// <param name="calledName">Called procedure name.</param>
    private class GetCallersTransitiveByCalledName(ICallsAccessor callsAccessor, string calledName)
        : CallsTransitiveQuery
    {
        public override IEnumerable<IPkbDto> Build() =>
            callsAccessor.GetCallersTransitive(calledName);
    }

    /// <summary>
    /// Get procedures that call any procedure.
    /// </summary>
    /// <param name="callsAccessor">Calls accessor.</param>
    private class GetCallers(ICallsAccessor callsAccessor) : CallsTransitiveQuery
    {
        public override IEnumerable<IPkbDto> Build() =>
            callsAccessor.GetCallers();
    }

    /// <summary>
    /// Get procedures that are called by any procedure.
    /// </summary>
    /// <param name="callsAccessor">Calls accessor.</param>
    private class GetCalled(ICallsAccessor callsAccessor) : CallsTransitiveQuery
    {
        public override IEnumerable<IPkbDto> Build() =>
            callsAccessor.GetCalled();
    }

    /// <summary>
    /// Check if caller calls called transitively.
    /// </summary>
    /// <param name="callsAccessor">Calls accessor.</param>
    /// <param name="callerName">Caller procedure name.</param>
    /// <param name="calledName">Called procedure name.</param>
    private class BooleanCallsTransitiveQuery(ICallsAccessor callsAccessor, string callerName, string calledName)
        : CallsTransitiveQuery
    {
        public override IEnumerable<IPkbDto> Build() =>
            IPkbDto.Boolean(callsAccessor.IsCalledTransitive(callerName, calledName));
    }

    /// <summary>
    /// Represents a calls query.
    /// </summary>
    private abstract class CallsTransitiveQuery
    {
        /// <summary>
        /// Builds a query.
        /// </summary>
        /// <returns> The query. </returns>
        public abstract IEnumerable<IPkbDto> Build();
    }

    #endregion
}