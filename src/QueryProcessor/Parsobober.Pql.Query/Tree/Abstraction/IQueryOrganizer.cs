using Parsobober.Pql.Query.Arguments;

namespace Parsobober.Pql.Query.Tree.Abstraction;

public interface IQueryOrganizer
{
    /// <summary>
    /// Organizes queries and select statement into query tree.
    /// </summary>
    /// <returns>Query tree</returns>
    IQueryNode Organize(IDeclaration select);

    IQueryNode OrganizeBoolean();
}