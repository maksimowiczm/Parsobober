using Parsobober.Pkb.Relations.Abstractions.Creators;

namespace Parsobober.Pkb.Relations.Abstractions;

public interface IPkbCreators
{
    IProgramContextCreator ProgramContext { get; }
    IFollowsCreator Follows { get; }
    IModifiesCreator Modifies { get; }
    IParentCreator Parent { get; }
    IUsesCreator Uses { get; }
}