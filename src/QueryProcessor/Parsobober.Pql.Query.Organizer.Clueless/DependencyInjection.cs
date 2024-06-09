using Microsoft.Extensions.DependencyInjection;
using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query.Organizer.Clueless;

[Obsolete("Use Parsobober.Pql.Query.Organizer.Night instead.")]
public static class DependencyInjection
{
    public static IServiceCollection AddPqlQueries(this IServiceCollection services)
    {
        services.AddTransient<IQueryOrganizerBuilder, QueryOrganizerBuilder>();
        Query.DependencyInjection.AddPqlQueries(services);

        return services;
    }
}