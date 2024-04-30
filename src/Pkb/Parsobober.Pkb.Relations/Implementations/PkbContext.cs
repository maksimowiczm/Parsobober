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
) : IPkbCreators, IPkbAccessors
{
    IProgramContextCreator IPkbCreators.ProgramContext => programContext;
    IFollowsCreator IPkbCreators.Follows => follows;
    IModifiesCreator IPkbCreators.Modifies => modifies;
    IParentCreator IPkbCreators.Parent => parent;
    IUsesCreator IPkbCreators.Uses => uses;
    IProgramContextAccessor IPkbAccessors.ProgramContext => programContext;
    IFollowsAccessor IPkbAccessors.Follows => follows;
    IParentAccessor IPkbAccessors.Parent => parent;
    IModifiesAccessor IPkbAccessors.Modifies => modifies;
    IUsesAccessor IPkbAccessors.Uses => uses;
}