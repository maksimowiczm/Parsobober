using Microsoft.Extensions.DependencyInjection;

namespace Parsobober.Simple.Lexer;

public static class DependencyInjection
{
    public static IServiceCollection AddSimpleLexer(this IServiceCollection services)
    {
        services.AddScoped<SlyLexerAdapter>();

        return services;
    }
}