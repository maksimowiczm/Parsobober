using Parsobober.Pkb.Relations.Abstractions;
using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Abstractions.Creators;

namespace Parsobober.Pkb.Relations.Implementations;

internal class PkbContext(
    ProgramContext programContext,
    FollowsRelation follows,
    ModifiesRelation modifies,
    ParentRelation parent,
    UsesRelation uses
) : IPkbCreators, IPkbAccessor
{
    IProgramContextCreator IPkbCreators.ProgramContext => programContext;
    IFollowsCreator IPkbCreators.Follows => follows;
    IModifiesCreator IPkbCreators.Modifies => modifies;
    IParentCreator IPkbCreators.Parent => parent;
    IUsesCreator IPkbCreators.Uses => uses;
    IProgramContextAccessor IPkbAccessor.ProgramContext => programContext;
    IFollowsAccessor IPkbAccessor.Follows => follows;
    IParentAccessor IPkbAccessor.Parent => parent;
    IModifiesAccessor IPkbAccessor.Modifies => modifies;
    IUsesAccessor IPkbAccessor.Uses => uses;
}