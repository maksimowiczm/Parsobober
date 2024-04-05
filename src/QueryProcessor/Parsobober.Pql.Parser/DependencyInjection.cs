using Microsoft.Extensions.DependencyInjection;

namespace Parsobober.Pql.Parser;

public static class DependencyInjection
{
    public static IServiceCollection AddPqlParser(this IServiceCollection services)
    {
        services.AddSingleton<PqlParser>(); // singleton?

        return services;
    }
}