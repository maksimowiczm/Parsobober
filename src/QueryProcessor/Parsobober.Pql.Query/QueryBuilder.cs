using Parsobober.Pkb.Relations.Abstractions;
using Parsobober.Pql.Query.Abstractions;
using Parsobober.Pql.Query.Queries;

namespace Parsobober.Pql.Query;

public class QueryBuilder(IPkbAccessors accessor) : IQueryBuilder
{
    private string _select = string.Empty;

    // trzymanie deklaracji jako konkretne typy IDeclaration
    private readonly Dictionary<string, IDeclaration> _declarations = new();

    private readonly Dictionary<string, List<string>> _attributes = new();

    private readonly Parent.Builder _parentBuilder = new(accessor.Parent);

    private readonly ParentTransitive.Builder _parentTransitiveBuilder = new(accessor.Parent);

    private readonly Follows.Builder _followsBuilder = new(accessor.Follows);

    private readonly FollowsTransitive.Builder _followsTransitiveBuilder = new(accessor.Follows);

    private readonly Modifies.Builder _modifiesBuilder = new(accessor.Modifies);

    private readonly Uses.Builder _usesBuilder = new(accessor.Uses);

    public IQuery Build()
    {
        // rozwiązanie na pierwszą iterację

        var results = new List<IEnumerable<IComparable>?>
        {
            _parentBuilder.Build(_select, _declarations),
            _parentTransitiveBuilder.Build(_select, _declarations),
            _followsTransitiveBuilder.Build(_select, _declarations),
            _followsBuilder.Build(_select, _declarations),
            _modifiesBuilder.Build(_select, _declarations),
            _usesBuilder.Build(_select, _declarations)
        };

        var result = results.Single(r => r is not null);

        // todo przefiltrować atrybuty (with)

        return new Query(result!);
    }

    public IQueryBuilder AddSelect(string synonym)
    {
        _select = synonym;
        return this;
    }

    public IQueryBuilder AddDeclaration(string declaration)
    {
        var split = declaration.Split();
        var rest = string.Join("", split.Skip(1)).Split(',');

        var type = IDeclaration.Parse(split.First());

        foreach (var key in rest.Select(s => s.Replace(";", "")))
        {
            _declarations.Add(key, type);
        }

        return this;
    }

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

    #region Relation methods

    public IQueryBuilder AddFollows(string reference1, string reference2)
    {
        _followsBuilder.Add(reference1, reference2);
        return this;
    }

    public IQueryBuilder AddFollowsTransitive(string reference1, string reference2)
    {
        _followsTransitiveBuilder.Add(reference1, reference2);
        return this;
    }

    public IQueryBuilder AddParent(string parent, string child)
    {
        _parentBuilder.Add(parent, child);
        return this;
    }

    public IQueryBuilder AddParentTransitive(string parent, string child)
    {
        _parentTransitiveBuilder.Add(parent, child);
        return this;
    }

    public IQueryBuilder AddModifies(string reference1, string reference2)
    {
        _modifiesBuilder.Add(reference1, reference2);
        return this;
    }

    public IQueryBuilder AddUses(string reference1, string reference2)
    {
        _usesBuilder.Add(reference1, reference2);
        return this;
    }

    #endregion
}