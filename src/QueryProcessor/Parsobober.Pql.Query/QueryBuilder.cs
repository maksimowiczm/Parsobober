using System.Text.RegularExpressions;
using Parsobober.Pkb.Relations.Abstractions;
using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pql.Query.Abstractions;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Queries.With;
using Parsobober.Pql.Query.Tree;

namespace Parsobober.Pql.Query;

internal partial class QueryBuilder(IPkbAccessors accessor, IProgramContextAccessor programContext) : IQueryBuilder
{
    private string _select = string.Empty;
    private IDeclaration Select => _declarations[_select];

    // trzymanie deklaracji jako konkretne typy IDeclaration
    private readonly Dictionary<string, IDeclaration> _declarations = new();

    private readonly Dictionary<string, (string attribute, string value)> _attributes = new();

    private readonly List<(string left, string right)> _parent = [];

    private readonly List<(string left, string right)> _parentTransitive = [];

    private readonly List<(string left, string right)> _follows = [];

    private readonly List<(string left, string right)> _followsTransitive = [];

    private readonly List<(string left, string right)> _modifies = [];

    private readonly List<(string left, string right)> _uses = [];

    private readonly List<IQueryDeclaration> _queries = [];

    private void AddQueries<T>(List<(string, string)> relations, Func<IArgument, IArgument, T> queryCreator)
        where T : IQueryDeclaration
    {
        foreach (var (l, r) in relations)
        {
            var left = IArgument.Parse(_declarations, l);
            var right = IArgument.Parse(_declarations, r);
            _queries.Add(queryCreator(left, right));
        }
    }

    public IQueryResult Build()
    {
        AddQueries(_parent, (l, r) => new Parent.QueryDeclaration(l, r, accessor.Parent));
        AddQueries(_parentTransitive, (l, r) => new ParentTransitive.QueryDeclaration(l, r, accessor.Parent));
        AddQueries(_follows, (l, r) => new Follows.QueryDeclaration(l, r, accessor.Follows));
        AddQueries(_followsTransitive,
            (l, r) => new FollowsTransitive.QueryDeclaration(l, r, accessor.Follows));
        AddQueries(_modifies, (l, r) => new Modifies.QueryDeclaration(l, r, accessor.Modifies));
        AddQueries(_uses, (l, r) => new Uses.QueryDeclaration(l, r, accessor.Uses));

        var factory = new WithQueryFactory(programContext);
        var attributes = _attributes.Select(a =>
        {
            var (key, (attribute, value)) = a;
            return factory.Create(_declarations[key], attribute, value);
        }).ToList<IAttributeQuery>();

        var organizer = new QueryOrganizer(_queries.ToList(), attributes, accessor.ProgramContext);
        var root = organizer.Organize(Select);

        return new QueryResult(root.Do());
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

        var type = split.First();

        foreach (var key in rest.Select(s => s.Replace(";", "")))
        {
            _declarations.Add(key, IDeclaration.Parse(type, key));
        }

        return this;
    }

    public IQueryBuilder With(string attribute, string value)
    {
        var regex = AttributeRegex();
        var match = regex.Match(attribute);
        var groups = match.Groups.Values.ToList();
        var key = groups[1].Value;
        var attributeKey = groups[2].Value;

        _attributes.Add(key, (attributeKey, value));

        return this;
    }

    #region Relation methods

    public IQueryBuilder AddFollows(string reference1, string reference2)
    {
        _follows.Add((reference1, reference2));
        return this;
    }

    public IQueryBuilder AddFollowsTransitive(string reference1, string reference2)
    {
        _followsTransitive.Add((reference1, reference2));
        return this;
    }

    public IQueryBuilder AddParent(string parent, string child)
    {
        _parent.Add((parent, child));
        return this;
    }

    public IQueryBuilder AddParentTransitive(string parent, string child)
    {
        _parentTransitive.Add((parent, child));
        return this;
    }

    public IQueryBuilder AddModifies(string reference1, string reference2)
    {
        _modifies.Add((reference1, reference2));
        return this;
    }

    public IQueryBuilder AddUses(string reference1, string reference2)
    {
        _uses.Add((reference1, reference2));
        return this;
    }

    [GeneratedRegex(@"^(.+)\.(.+)$", RegexOptions.Compiled)]
    private static partial Regex AttributeRegex();

    #endregion
}