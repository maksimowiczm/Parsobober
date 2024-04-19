using Microsoft.Extensions.DependencyInjection;
using Parsobober.Simple.Parser.Abstractions;
using Parsobober.Simple.Parser.Extractor;

namespace Parsobober.Simple.Parser;

public static class DependencyInjection
{
    public static IServiceCollection AddDesignExtractor(this IServiceCollection services)
    {
        services.AddSingleton<ISimpleExtractor, FollowsExtractor>();
        services.AddSingleton<ISimpleExtractor, ModifiesExtractor>();
        services.AddSingleton<ISimpleExtractor, ParentExtractor>();
        services.AddSingleton<ISimpleExtractor, ProgramContextExtractor>();
        services.AddSingleton<ISimpleExtractor, UsesExtractor>();

        return services;
    }
}