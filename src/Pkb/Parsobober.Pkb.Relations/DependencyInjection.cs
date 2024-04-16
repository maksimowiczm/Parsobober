using Microsoft.Extensions.DependencyInjection;
using Parsobober.Pkb.Relations.Abstractions;
using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Abstractions.Creators;
using Parsobober.Pkb.Relations.Implementations;

namespace Parsobober.Pkb.Relations;

public static class DependencyInjection
{
    public static IServiceCollection AddRelations(this IServiceCollection services)
    {
        services.AddSingleton<IProgramContextCreator, ProgramContext>();
        services.AddSingleton<IProgramContextAccessor, ProgramContext>();

        services.AddSingleton<IFollowsCreator, FollowsRelation>();
        services.AddSingleton<IParentCreator, ParentRelation>();
        services.AddSingleton<IModifiesCreator, ModifiesRelation>();
        services.AddSingleton<IUsesCreator, UsesRelation>();

        services.AddSingleton<IPkbCreators, PkbContext>();

        return services;
    }
}