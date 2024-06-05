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
        services.AddScoped<ProgramContext>();
        services.AddScoped<IProgramContextCreator>(s => s.GetRequiredService<ProgramContext>());
        services.AddScoped<IProgramContextAccessor>(s => s.GetRequiredService<ProgramContext>());
        services.AddScoped<IDtoProgramContextAccessor>(s => s.GetRequiredService<ProgramContext>());

        services.AddScoped<FollowsRelation>();
        services.AddScoped<ParentRelation>();
        services.AddScoped<ModifiesRelation>();
        services.AddScoped<UsesRelation>();
        services.AddScoped<CallsRelation>();

        services.AddScoped<PkbContext>();
        services.AddScoped<IPkbCreators>(s => s.GetRequiredService<PkbContext>());
        services.AddScoped<IPkbAccessors>(s => s.GetRequiredService<PkbContext>());

        services.AddScoped<IFollowsCreator>(s => s.GetRequiredService<FollowsRelation>());
        services.AddScoped<IParentCreator>(s => s.GetRequiredService<ParentRelation>());
        services.AddScoped<IModifiesCreator>(s => s.GetRequiredService<ModifiesRelation>());
        services.AddScoped<IUsesCreator>(s => s.GetRequiredService<UsesRelation>());
        services.AddScoped<ICallsCreator>(s => s.GetRequiredService<CallsRelation>());

        services.AddScoped<IPostParseComputable>(s => s.GetRequiredService<ModifiesRelation>());

        return services;
    }
}