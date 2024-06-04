using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Tree.Abstraction;
using Parsobober.Pql.Query.Tree.Node;
using Parsobober.Shared;

namespace Parsobober.Pql.Query.Organizer;

public class QueryOrganizer : IQueryOrganizer
{
    private readonly List<IQueryDeclaration> _queries;
    private readonly List<IAttributeQuery> _attributes;
    private readonly IDtoProgramContextAccessor _context;

    public QueryOrganizer(
        List<IQueryDeclaration> queries,
        List<IAttributeQuery> attributes,
        IDtoProgramContextAccessor context
    )
    {
        _context = context;
        _queries = queries;
        _attributes = attributes;

        _declarations = _queries
            .SelectMany<IQueryDeclaration, IDeclaration?>(
                q => new[] { q.Left as IDeclaration, q.Right as IDeclaration })
            .Distinct()
            .WhereNotNull()
            .ToList();

        _declarationsMap = _declarations
            .ToDictionary(x => x, d => d switch
            {
                IStatementDeclaration.Statement => _context.Statements as IEnumerable<IComparable>,
                IStatementDeclaration.Assign => _context.Assigns,
                IStatementDeclaration.While => _context.Whiles,
                IStatementDeclaration.If => _context.Ifs,
                IStatementDeclaration.Call => _context.Calls,
                IVariableDeclaration.Variable => _context.Variables,
                _ => throw new NotImplementedException()
            });

        // apply attributes
        foreach (var attribute in _attributes)
        {
            if (_declarationsMap.TryGetValue(attribute.Declaration, out var values))
            {
                _declarationsMap[attribute.Declaration] = values.Intersect(attribute.Do());
            }
        }
    }

    private readonly List<IDeclaration> _declarations;
    private readonly Dictionary<IDeclaration, IEnumerable<IComparable>> _declarationsMap;

    private bool TryAddDeclarationToMap(IDeclaration declaration)
    {
        if (_declarationsMap.ContainsKey(declaration))
        {
            return false;
        }

        _declarationsMap[declaration] = declaration switch
        {
            IStatementDeclaration.Statement => _context.Statements as IEnumerable<IComparable>,
            IStatementDeclaration.Assign => _context.Assigns,
            IStatementDeclaration.While => _context.Whiles,
            IStatementDeclaration.If => _context.Ifs,
            IStatementDeclaration.Call => _context.Calls,
            IVariableDeclaration.Variable => _context.Variables,
            _ => throw new NotImplementedException()
        };

        // Apply attribute
        var attribute = _attributes.FirstOrDefault(a => a.Declaration == declaration);
        if (attribute is not null)
        {
            _declarationsMap[declaration] = _declarationsMap[declaration].Intersect(attribute.Do());
        }

        return true;
    }

    public IQueryNode Organize(IDeclaration select)
    {
        var selectNothing = TryAddDeclarationToMap(select);

        foreach (var _ in _queries)
        {
            Iterate(_queries.ToList(), _declarations.ToList());
        }

        if (selectNothing)
        {
            if (_declarationsMap.Values.All(v => v.Any()))
            {
                if (_declarationsMap.TryGetValue(select, out var result1))
                {
                    return new EnumerableQueryNode(result1);
                }
            }

            return new EnumerableQueryNode([]);
        }

        if (_declarationsMap.TryGetValue(select, out var result))
        {
            return new EnumerableQueryNode(result);
        }

        if (_declarationsMap.Values.All(v => !v.Any()))
        {
            return new EnumerableQueryNode([]);
        }

        return new PkbQueryNode(select, _context);
    }

    private void Iterate(List<IQueryDeclaration> queries, List<IDeclaration> declarations)
    {
        while (queries.Count > 0)
        {
            IQueryDeclaration? query = null;
            IDeclaration? currentSelect = null;
            var breakLoop = false;
            while (query is null)
            {
                currentSelect = declarations.FirstOrDefault();
                if (currentSelect is null)
                {
                    breakLoop = true;
                    break;
                }

                query = queries.FirstOrDefault(q => q.Left == currentSelect || q.Right == currentSelect);
                if (query is null)
                {
                    declarations.Remove(currentSelect);
                }
            }

            if (breakLoop)
            {
                break;
            }

            var (left, right) = (query!.Left, query.Right);

            if (left == currentSelect || right == currentSelect)
            {
                var other = (left == currentSelect) ? right : left;

                if (other is not IDeclaration otherDeclaration)
                {
                    _declarationsMap[currentSelect] = _declarationsMap[currentSelect].Intersect(query.Do());
                }
                else if (_declarationsMap.TryGetValue(otherDeclaration, out var values))
                {
                    List<IComparable> newValues = [];
                    foreach (var value in values)
                    {
                        var replaced = query.ReplaceArgument(otherDeclaration, IArgument.Parse(value));
                        newValues.AddRange(replaced.Do(currentSelect));
                    }

                    newValues = newValues.Distinct().ToList();
                    _declarationsMap[currentSelect] = _declarationsMap[currentSelect].Intersect(newValues);

                    List<IComparable> updatedValues = [];
                    foreach (var value in _declarationsMap[currentSelect])
                    {
                        var replaced = query.ReplaceArgument(currentSelect, IArgument.Parse(value));
                        updatedValues.AddRange(replaced.Do(otherDeclaration));
                    }

                    updatedValues = updatedValues.Distinct().ToList();
                    _declarationsMap[otherDeclaration] = _declarationsMap[otherDeclaration].Intersect(updatedValues);
                }
            }

            queries.Remove(query);
        }
    }

    public IQueryNode OrganizeBoolean()
    {
        var select = _declarations.FirstOrDefault();

        // if there is select in any query declaration => Organize
        if (select is not null)
        {
            return Organize(select);
        }

        // otherwise just do every query in place
        var result = _queries
            .Select(q => q.Do())
            .All(r => r.Any());

        return new BooleanQueryNode(result);
    }
}