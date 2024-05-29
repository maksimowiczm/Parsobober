using System.Text.RegularExpressions;
using Parsobober.Pkb.Relations.Abstractions;
using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pql.Query.Abstractions;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Queries.With;
using Parsobober.Pql.Query.QueryResult;
using Parsobober.Pql.Query.Tree;
using static Parsobober.Pql.Query.Tree.Abstraction.IQueryContainer;

namespace Parsobober.Pql.Query;

internal partial class QueryBuilder(
    IPkbAccessors accessor,
    IProgramContextAccessor programContext,
    IQueryContainerBuilder queryContainerBuilder
) : IQueryBuilder
{
    private string _select = string.Empty;

    private IDeclaration? Select => _declarations.GetValueOrDefault(_select);

    // trzymanie deklaracji jako konkretne typy IDeclaration
    private readonly Dictionary<string, IDeclaration> _declarations = new();

    private record AttributeDeclaration(string Attribute, string Value);

    private readonly Dictionary<string, AttributeDeclaration> _attributes = new();

    private record QueryDeclaration(string Left, string Right);

    private readonly List<QueryDeclaration> _parent = [];

    private readonly List<QueryDeclaration> _parentTransitive = [];

    private readonly List<QueryDeclaration> _follows = [];

    private readonly List<QueryDeclaration> _followsTransitive = [];

    private readonly List<QueryDeclaration> _modifies = [];

    private readonly List<QueryDeclaration> _uses = [];

    private IQueryResultFactory _queryResultFactory = new QueryListResult.Factory();

    private void AddQueries<T>(List<QueryDeclaration> relations, Func<IArgument, IArgument, T> queryCreator)
        where T : IQueryDeclaration
    {
        foreach (var (l, r) in relations.Distinct())
        {
            var left = IArgument.Parse(_declarations, l);
            var right = IArgument.Parse(_declarations, r);
            queryContainerBuilder.AddQuery(queryCreator(left, right));
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
        var attributes = _attributes
            .Distinct()
            .Select(a =>
            {
                var (key, (attribute, value)) = a;
                return factory.Create(_declarations[key], attribute, value);
            });
        foreach (var attribute in attributes)
        {
            queryContainerBuilder.AddAttribute(attribute);
        }

        var organizer = new QueryOrganizer(queryContainerBuilder.Build(), accessor.ProgramContext);

        var root = Select switch
        {
            not null => organizer.Organize(Select),
            _ => organizer.OrganizeBoolean()
        };

        return _queryResultFactory.Create(root.Do());
    }

    public IQueryBuilder AddSelect(string synonym)
    {
        _queryResultFactory = new QueryListResult.Factory();
        _select = synonym;
        return this;
    }

    public IQueryBuilder SetBoolean()
    {
        _queryResultFactory = new QueryBooleanResult.Factory();
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

        _attributes.Add(key, new AttributeDeclaration(attributeKey, value));

        return this;
    }

    #region Relation methods

    public IQueryBuilder AddFollows(string reference1, string reference2)
    {
        _follows.Add(new QueryDeclaration(reference1, reference2));
        return this;
    }

    public IQueryBuilder AddFollowsTransitive(string reference1, string reference2)
    {
        _followsTransitive.Add(new QueryDeclaration(reference1, reference2));
        return this;
    }

    public IQueryBuilder AddParent(string parent, string child)
    {
        _parent.Add(new QueryDeclaration(parent, child));
        return this;
    }

    public IQueryBuilder AddParentTransitive(string parent, string child)
    {
        _parentTransitive.Add(new QueryDeclaration(parent, child));
        return this;
    }

    public IQueryBuilder AddModifies(string reference1, string reference2)
    {
        _modifies.Add(new QueryDeclaration(reference1, reference2));
        return this;
    }

    public IQueryBuilder AddUses(string reference1, string reference2)
    {
        _uses.Add(new QueryDeclaration(reference1, reference2));
        return this;
    }

    [GeneratedRegex(@"^(.+)\.(.+)$", RegexOptions.Compiled)]
    private static partial Regex AttributeRegex();

    #endregion
}