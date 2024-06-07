using Parsobober.Pkb.Relations.Abstractions.Accessors;

namespace Parsobober.Pkb.Relations.Abstractions;

public interface IPkbAccessors
{
    IDtoProgramContextAccessor ProgramContext { get; }
    IFollowsAccessor Follows { get; }
    IParentAccessor Parent { get; }
    IModifiesAccessor Modifies { get; }
    IUsesAccessor Uses { get; }
    ICallsAccessor Calls { get; }
    INextAccessor Next { get; }
}