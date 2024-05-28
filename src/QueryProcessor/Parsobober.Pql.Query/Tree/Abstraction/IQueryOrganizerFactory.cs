using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Tree.Abstraction;

internal interface IQueryOrganizerFactory
{
    public IQueryOrganizer Create(IQueryContainer container, List<IAttributeQuery> attributes);
}