using Parsobober.Pkb.Relations.Abstractions;
using Parsobober.Pkb.Relations.Dto;

namespace Parsobober.Pql.Query;

public class QueryBuilder(IPkbAccessor accesor) : IQueryBuilder
{

    List<(string parent, string child)> parentRelations = new();
    public IQuery Build()
    {

        List<Func<IPkbAccessor, IEnumerable<Statement>>> actions = new();

        foreach (var item in parentRelations)
        {
            if (int.TryParse(item.parent, out int parentInt))
            {

                if (int.TryParse(item.child, out int childInt))
                {
                    throw new NotImplementedException();
                }
                else if (_declarations.ContainsKey(item.child))
                {
                    actions.Add(ex => ex.Parent.GetChildren(parentInt));
                }
            }
            else if (_declarations.ContainsKey(item.parent))
            {

                if (int.TryParse(item.child, out int childInt))
                {
                    actions.Add(ex => [ex.Parent.GetParent(childInt)]);
                }
                else if (_declarations.ContainsKey(item.child))
                {
                    string arg;
                    if (item.child == _select)
                    {
                        arg = item.parent;
                    }
                    else
                    {
                        arg = item.child;
                    }
                    _declarations.TryGetValue(arg, out var type);
                    _declarations.TryGetValue(_select, out var resultType);

                    if (type == typeof(Statement))
                    {
                        actions.Add(ex => ex.Parent.GetParents<Statement>());
                    }
                    else if (type == typeof(While))
                    {
                        actions.Add(ex => ex.Parent.GetParents<While>());
                    }
                    else if (type == typeof(Assign))
                    {
                        actions.Add(ex => ex.Parent.GetParents<Assign>());
                    }

                }
            }
        }
        Query query = new Query(_declarations, _select, actions, accesor);
        return query;
    }

    public IQueryBuilder AddFollows(string reference1, string reference2)
    {
        return this;
    }

    public IQueryBuilder AddFollowsTransitive(string reference1, string reference2)
    {
        return this;
    }

    public IQueryBuilder AddParent(string parent, string child)
    {
        parentRelations.Add((parent, child));
        return this;
    }

    public IQueryBuilder AddParentTransitive(string parent, string child)
    {
        return this;
    }

    public IQueryBuilder AddModifies(string reference1, string reference2)
    {
        return this;
    }

    public IQueryBuilder AddUses(string reference1, string reference2)
    {
        return this;
    }

    private readonly Dictionary<string, List<string>> _attributes = new();

    public IQueryBuilder With(string attribute, string reference)
    {
        if (_attributes.TryGetValue(attribute, out var attributes))
        {
            attributes.Add(reference);
            return this;
        }

        _attributes.Add(attribute, [reference]);

        return this;
    }

    private readonly Dictionary<string, Type> _declarations = new();

    public IQueryBuilder AddDeclaration(string declaration)
    {
        var split = declaration.Split();
        var rest = string.Join("", split.Skip(1)).Split(',');

        var declarations = split.First() switch
        {
            "stmt" => rest.Select(s => (s.Replace(";", ""), typeof(Statement))),
            "assign" => rest.Select(s => (s.Replace(";", ""), typeof(Assign))),
            "while" => rest.Select(s => (s.Replace(";", ""), typeof(While))),
            "variable" => rest.Select(s => (s.Replace(";", ""), typeof(Variable))),
            "procedure" => rest.Select(s => (s.Replace(";", ""), typeof(Procedure))),
            _ => throw new ArgumentOutOfRangeException()
        };

        foreach (var (key, type) in declarations)
        {
            _declarations.Add(key, type);
        }

        return this;
    }

    private string _select = string.Empty;

    public IQueryBuilder AddSelect(string synonym)
    {
        _select = synonym;
        return this;
    }
}