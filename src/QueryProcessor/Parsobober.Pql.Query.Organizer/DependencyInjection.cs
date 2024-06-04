using Microsoft.Extensions.DependencyInjection;
using Parsobober.Pql.Query.Tree.Abstraction;

namespace Parsobober.Pql.Query.Organizer;

public static class DependencyInjection
{
    public static IServiceCollection AddPqlQueries(this IServiceCollection services)
    {
        services.AddTransient<IQueryOrganizerBuilder, QueryOrganizerBuilder>();
        Query.DependencyInjection.AddPqlQueries(services);

        return services;
    }
}