using Microsoft.Extensions.DependencyInjection;
using Parsobober.Simple.Parser.Abstractions;
using Parsobober.Simple.Parser.Extractor;

namespace Parsobober.DesignExtractor;

public static class DependencyInjection
{
    public static IServiceCollection AddDesignExtractor(this IServiceCollection services)
    {
        services.AddScoped<ISimpleExtractor, FollowsExtractor>();
        services.AddScoped<ISimpleExtractor, ModifiesExtractor>();
        services.AddScoped<ISimpleExtractor, ParentExtractor>();
        services.AddScoped<ISimpleExtractor, ProgramContextExtractor>();
        services.AddScoped<ISimpleExtractor, UsesExtractor>();

        return services;
    }
}