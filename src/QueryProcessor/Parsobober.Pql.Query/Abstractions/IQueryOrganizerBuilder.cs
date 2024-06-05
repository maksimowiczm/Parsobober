using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Abstractions;

public interface IQueryOrganizerBuilder
{
    void AddQuery(IQueryDeclaration query);

    void AddAttribute(IAttributeQuery attributeQuery);

    IQueryOrganizer Build();
}