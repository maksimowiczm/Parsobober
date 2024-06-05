using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Queries.Core;
using Parsobober.Pql.Query.Queries.Exceptions;

namespace Parsobober.Pql.Query.Queries;

internal static class Calls
{
    public class QueryDeclaration(IArgument caller, IArgument called, ICallsAccessor accessor)
        : ReplaceableArgumentQueryDeclaration<QueryDeclaration>, IQueryDeclaration
    {
        public override IArgument Left { get; } = caller;
        public override IArgument Right { get; } = called;

        public override IEnumerable<IComparable> Do()
        {
            var query = (Left, Right) switch
            {
                // Calls('name', 'name')
                (IArgument.Name left, IArgument.Name right) =>
                    new BooleanCallsQuery(accessor, left.Value, right.Value).Build(),

                _ => DoDeclaration()
            };

            return query;
        }

        public override IEnumerable<IComparable> Do(IDeclaration select)
        {
            var query = (Left, Right) switch
            {
                // Calls(proc, 'name')
                (IProcedureDeclaration declaration, IArgument.Name called) =>
                    new GetCallersByCalledName(accessor, called.Value).Build(),

                // Calls('name', proc)
                (IArgument.Name caller, IProcedureDeclaration declaration) =>
                    new GetCalledByCallerName(accessor, caller.Value).Build(),

                // Calls(proc, proc)
                (IProcedureDeclaration caller, IProcedureDeclaration called) => BuildCallsWithSelect(caller, called),

                _ => throw new QueryNotSupported(this, $"Calls({Left}, {Right}) is not supported.")
            };

            return query;

            IEnumerable<IComparable> BuildCallsWithSelect(IProcedureDeclaration caller, IProcedureDeclaration called)
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
    /// Get procedures that call given procedure.
    /// </summary>
    /// <param name="callsAccessor">Calls accessor.</param>
    /// <param name="callerName">Caller procedure name.</param>
    private class GetCalledByCallerName(ICallsAccessor callsAccessor, string callerName) : CallsQuery
    {
        public override IEnumerable<IComparable> Build() =>
            callsAccessor.GetCalled(callerName);
    }

    /// <summary>
    /// Get procedures that call given procedure.
    /// </summary>
    /// <param name="callsAccessor">Calls accessor.</param>
    /// <param name="calledName">Called procedure name.</param>
    private class GetCallersByCalledName(ICallsAccessor callsAccessor, string calledName) : CallsQuery
    {
        public override IEnumerable<IComparable> Build() =>
            callsAccessor.GetCallers(calledName);
    }

    /// <summary>
    /// Get procedures that call any procedure.
    /// </summary>
    /// <param name="callsAccessor">Calls accessor.</param>
    private class GetCallers(ICallsAccessor callsAccessor) : CallsQuery
    {
        public override IEnumerable<IComparable> Build() =>
            callsAccessor.GetCallers();
    }

    /// <summary>
    /// Get procedures that are called by any procedure.
    /// </summary>
    /// <param name="callsAccessor">Calls accessor.</param>
    private class GetCalled(ICallsAccessor callsAccessor) : CallsQuery
    {
        public override IEnumerable<IComparable> Build() =>
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
        public override IEnumerable<IComparable> Build() =>
            callsAccessor.IsCalled(callerName, calledName)
                ? Enumerable.Repeat<IComparable>(true, 1)
                : Enumerable.Empty<IComparable>();
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
        public abstract IEnumerable<IComparable> Build();
    }

    #endregion
}