using Parsobober.Pkb.Relations.Abstractions;
using Parsobober.Pkb.Relations.Abstractions.Creators;

namespace Parsobober.Pkb.Relations.Implementations;

public class PkbContext(
    IProgramContextCreator programContext,
    IFollowsCreator follows,
    IModifiesCreator modifies,
    IParentCreator parent,
    IUsesCreator uses
) : IPkbCreators
{
    public IProgramContextCreator ProgramContext => programContext;
    public IFollowsCreator Follows => follows;
    public IModifiesCreator Modifies => modifies;
    public IParentCreator Parent => parent;
    public IUsesCreator Uses => uses;
}