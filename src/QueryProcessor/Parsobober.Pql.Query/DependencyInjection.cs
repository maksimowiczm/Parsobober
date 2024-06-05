using Microsoft.Extensions.DependencyInjection;
using Parsobober.Pql.Query.Abstractions;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Queries.Core;

namespace Parsobober.Pql.Query;

public static class DependencyInjection
{
    public static IServiceCollection AddPqlQueries(this IServiceCollection services)
    {
        services.AddTransient<IQueryBuilder, QueryBuilder>();
        services.AddTransient<IComparer<IQueryDeclaration>, QueryDeclarationComparer>();

        return services;
    }
}