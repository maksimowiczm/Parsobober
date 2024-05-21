using Parsobober.Pql.Query.Arguments;

namespace Parsobober.Pql.Query.Queries.Abstractions;

internal interface IAttributeQuery 
{
    IDeclaration Declaration { get; }
    
    IEnumerable<IComparable> Do();
}