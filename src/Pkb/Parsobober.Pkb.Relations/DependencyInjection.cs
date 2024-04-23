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
        services.AddScoped<IProgramContextCreator, ProgramContext>();
        services.AddScoped<IProgramContextAccessor, ProgramContext>();

        services.AddScoped<IFollowsCreator, FollowsRelation>();
        services.AddScoped<IParentCreator, ParentRelation>();
        services.AddScoped<IModifiesCreator, ModifiesRelation>();
        services.AddScoped<IUsesCreator, UsesRelation>();

        services.AddScoped<IPkbCreators, PkbContext>();

        return services;
    }
}