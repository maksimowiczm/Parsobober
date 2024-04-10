using Microsoft.Extensions.DependencyInjection;
using Parsobober.Pkb.Ast.Abstractions;

namespace Parsobober.Pkb.Ast;

public static class DependencyInjection
{
    public static IServiceCollection AddAst(this IServiceCollection services)
    {
        services.AddSingleton<IAst, Ast>();

        return services;
    }
}