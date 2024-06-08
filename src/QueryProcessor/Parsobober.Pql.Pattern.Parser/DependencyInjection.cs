using Microsoft.Extensions.DependencyInjection;
using Parsobober.Pql.Pattern.Parser.Abstractions;

namespace Parsobober.Pql.Pattern.Parser;

public static class DependencyInjection
{
    public static IServiceCollection AddPatternParserBuilder(this IServiceCollection services)
    {
        services.AddTransient<IPatternParserBuilder, PatternParserBuilder>();

        return services;
    }
}