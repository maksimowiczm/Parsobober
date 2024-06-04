using Microsoft.Extensions.DependencyInjection;
using Parsobober.Pql.Query.Abstractions;
using Parsobober.Pql.Query.Tree;
using Parsobober.Pql.Query.Tree.Abstraction;

namespace Parsobober.Pql.Query;

public static class DependencyInjection
{
    public static IServiceCollection AddPqlQueries(this IServiceCollection services)
    {
        services.AddTransient<IQueryBuilder, QueryBuilder>();
        services.AddTransient<IQueryOrganizerBuilder, QueryOrganizerBuilder>();

        return services;
    }
}