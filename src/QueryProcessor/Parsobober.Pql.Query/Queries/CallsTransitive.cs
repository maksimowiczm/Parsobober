using Parsobober.Pkb.Relations.Abstractions.Accessors;
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

        public override IEnumerable<IComparable> Do(IDeclaration select)
        {
            // pattern matching argumentów
            var query = (Left, Right) switch
            {
                // Calls*(proc, 'name')
                (IProcedureDeclaration declaration, IArgument.VarName called) =>
                    new GetCallersTransitiveByCalledName(accessor, called.Value).Build(),

                // Calls*('name', proc)
                (IArgument.VarName caller, IProcedureDeclaration declaration) =>
                    new GetCalledTransitiveByCallerName(accessor, caller.Value).Build(),

                // Calls*(proc, proc)
                (IProcedureDeclaration caller, IProcedureDeclaration called) => BuildCallsTransitiveWithSelect(caller, called),

                // Calls*('name', 'name')
                (IArgument.VarName caller, IArgument.VarName called) =>
                    new IsCalledTransitive(accessor, caller.Value, called.Value).Build(),

                _ => throw new QueryNotSupported(this, $"Calls({Left}, {Right}) is not supported.")
            };

            return query;

            IEnumerable<IComparable> BuildCallsTransitiveWithSelect(IProcedureDeclaration caller, IProcedureDeclaration called)
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
    private class GetCalledTransitiveByCallerName(ICallsAccessor callsAccessor, string callerName) : CallsTransitiveQuery
    {
        public override IEnumerable<IComparable> Build() =>
            callsAccessor.GetCalledTransitive(callerName);
    }

    /// <summary>
    /// Get procedures that call transitively given procedure.
    /// </summary>
    /// <param name="callsAccessor">Calls accessor.</param>
    /// <param name="calledName">Called procedure name.</param>
    private class GetCallersTransitiveByCalledName(ICallsAccessor callsAccessor, string calledName) : CallsTransitiveQuery
    {
        public override IEnumerable<IComparable> Build() =>
            callsAccessor.GetCallersTransitive(calledName);
    }

    /// <summary>
    /// Get procedures that call any procedure.
    /// </summary>
    /// <param name="callsAccessor">Calls accessor.</param>
    private class GetCallers(ICallsAccessor callsAccessor) : CallsTransitiveQuery
    {
        public override IEnumerable<IComparable> Build() =>
            callsAccessor.GetCallers();
    }

    /// <summary>
    /// Get procedures that are called by any procedure.
    /// </summary>
    /// <param name="callsAccessor">Calls accessor.</param>
    private class GetCalled(ICallsAccessor callsAccessor) : CallsTransitiveQuery
    {
        public override IEnumerable<IComparable> Build() =>
            callsAccessor.GetCalled();
    }

    /// <summary>
    /// Check if caller calls called transitively.
    /// </summary>
    /// <param name="callsAccessor">Calls accessor.</param>
    /// <param name="callerName">Caller procedure name.</param>
    /// <param name="calledName">Called procedure name.</param>
    private class IsCalledTransitive(ICallsAccessor callsAccessor, string callerName, string calledName) : CallsTransitiveQuery
    {
        public override IEnumerable<IComparable> Build() =>
            callsAccessor.IsCalledTransitive(callerName, calledName)
                ? Enumerable.Repeat<IComparable>(true, 1)
                : Enumerable.Empty<IComparable>();
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
        public abstract IEnumerable<IComparable> Build();
    }

    #endregion
}