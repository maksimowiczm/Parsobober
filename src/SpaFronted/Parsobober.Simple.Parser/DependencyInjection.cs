using Microsoft.Extensions.DependencyInjection;
using Parsobober.Simple.Parser.Abstractions;

namespace Parsobober.Simple.Parser;

public static class DependencyInjection
{
    public static IServiceCollection AddSimpleParserBuilder(this IServiceCollection services)
    {
        services.AddSingleton<IParserBuilder, SimpleParserBuilder>(); // singleton?

        return services;
    }
}