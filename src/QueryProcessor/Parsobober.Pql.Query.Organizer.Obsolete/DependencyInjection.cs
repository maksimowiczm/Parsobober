using Microsoft.Extensions.DependencyInjection;
using Parsobober.Pql.Query.Abstractions;
using Parsobober.Pql.Query.Organizer.Obsolete.Tree;

namespace Parsobober.Pql.Query.Organizer.Obsolete;

public static class DependencyInjection
{
    [Obsolete("Obsolete query organizer. Use Parsobober.Pql.Query.Organizer instead.")]
    public static IServiceCollection AddPqlQueries(this IServiceCollection services)
    {
        services.AddTransient<IQueryOrganizerBuilder, QueryOrganizerBuilder>();

        return services;
    }
}