using Parsobober.Pkb.Relations.Dto;

namespace Parsobober.Pql.Query;

public class QueryBuilder : IQueryBuilder
{
    public IQuery Build()
    {
        throw new NotImplementedException();
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