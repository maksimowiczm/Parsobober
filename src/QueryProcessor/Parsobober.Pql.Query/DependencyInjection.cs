using Microsoft.Extensions.DependencyInjection;
using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query;

public static class DependencyInjection
{
    public static IServiceCollection AddPqlQueries(this IServiceCollection services)
    {
        services.AddTransient<IQueryBuilder, QueryBuilder>();

        return services;
    }
}