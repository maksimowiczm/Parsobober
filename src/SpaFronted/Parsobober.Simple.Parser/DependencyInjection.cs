using Microsoft.Extensions.DependencyInjection;

namespace Parsobober.Simple.Parser;

public static class DependencyInjection
{
    public static IServiceCollection AddSimpleParserBuilder(this IServiceCollection services)
    {
        services.AddSingleton<IParserBuilder, SimpleParserBuilder>(); // singleton?

        return services;
    }
}