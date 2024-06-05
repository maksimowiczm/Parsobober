using Microsoft.Extensions.DependencyInjection;
using Parsobober.DesignExtractor.Extractors;
using Parsobober.Simple.Parser.Abstractions;

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
        services.AddScoped<ISimpleExtractor, CallsExtractor>();
        services.AddScoped<ISimpleExtractor, PostParseExtractor>();

        return services;
    }
}