using Parsobober.Pkb.Relations.Abstractions.Creators;

namespace Parsobober.Pkb.Relations.Abstractions;

public interface IPkbCreators
{
    IFollowsCreator Follows { get; }
    IModifiesCreator Modifies { get; }
    IParentCreator Parent { get; }
    IUsesCreator Uses { get; }
}