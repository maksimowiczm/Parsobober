using Parsobober.Pql.Query.Arguments;

namespace Parsobober.Pql.Query.Tree.Abstraction;

internal interface IQueryOrganizer
{
    /// <summary>
    /// Organizes queries and select statement into query tree.
    /// </summary>
    /// <returns>Query tree</returns>
    IQueryNode Organize(IDeclaration select);

    /// <summary>
    /// Organizes boolean queries into query tree.
    /// </summary>
    IQueryNode OrganizeBoolean();
}