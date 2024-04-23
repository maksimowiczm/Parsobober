using Microsoft.Extensions.DependencyInjection;
using Parsobober.Pkb.Ast.Abstractions;

namespace Parsobober.Pkb.Ast;

public static class DependencyInjection
{
    public static IServiceCollection AddAst(this IServiceCollection services)
    {
        services.AddScoped<IAst, Ast>();

        return services;
    }
}