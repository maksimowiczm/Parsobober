using Microsoft.Extensions.DependencyInjection;
using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Ast.Abstractions;

namespace Parsobober.Pql.Query;

public static class DependencyInjection
{
    public static IServiceCollection AddPqlQueries(this IServiceCollection services)
    {
        services.AddTransient<IQueryBuilder, QueryBuilder>();

        return services;
    }
}