using Parsobober.Pkb.Relations.Abstractions;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Queries;
using Parsobober.Shared;

namespace Parsobober.Pql.Query;

public class QueryBuilder(IPkbAccessors accessor) : IQueryBuilder
{
    private readonly List<(string parent, string child)> _parentRelations = [];

    public IQuery Build()
    {
        // todo aktualnie działa tylko dla jednego parenta
        var parent = _parentRelations.First().parent;
        var child = _parentRelations.First().child;

        var query = BuildParent(parent, child);

        return new Query(query);
    }

    private IEnumerable<Statement> BuildParent(string parentStr, string childStr)
    {
        // parsowanie argumentów
        var nullableParentLine = parentStr.ParseOrNull<int>();
        var nullableChildLine = childStr.ParseOrNull<int>();

        var nullableParentType = _declarations.GetValueOrDefault(parentStr);
        var nullableChildType = _declarations.GetValueOrDefault(childStr);

        // pattern matching argumentów
        var query = ((nullableParentType, nullableParentLine), (nullableChildType, nullableChildLine)) switch
        {
            // Parent(T, T)
            (({ } parentType, null), ({ } childType, null)) =>
                BuildParentWithSelect((parentStr, parentType), (childStr, childType)),
            // Parent(T, 1)
            (({ } parentType, null), (null, { } childLine)) =>
                Activator.CreateInstance(
                    typeof(Parent.GetParentByLineNumber<>).MakeGenericType(parentType),
                    accessor.Parent,
                    childLine
                ),
            // Parent(1, T)
            ((null, { } parentLine), ({ } childType, null)) =>
                Activator.CreateInstance(
                    typeof(Parent.GetChildrenByLineNumber<>).MakeGenericType(childType),
                    accessor.Parent,
                    parentLine
                ),
            // Parent(1, 2) nie wspierane w tej wersji
            _ => throw new InvalidOperationException("Invalid query")
        } as IEnumerable<Statement>;

        return query!;
    }

    private IEnumerable<Statement> BuildParentWithSelect((string key, Type type) parent, (string key, Type type) child)
    {
        // tu nastąpi samowywrotka przy zapytaniach, w których nie ma wartości z selecta
        // przykład: Select x such that Parent(a, b)

        // sprawdzamy o co pytamy
        var queryType = (parent.key == _select) switch
        {
            // pytam o rodziców Parent(to chce, to mam)
            true => typeof(Parent.GetParentsByChildType<,>).MakeGenericType([parent.type, child.type]),
            // pytam o dzieci Parent(to mam, to chce)
            false => typeof(Parent.GetChildrenByParentType<,>).MakeGenericType([parent.type, child.type])
        };

        var query = Activator.CreateInstance(queryType, accessor.Parent) as IEnumerable<Statement>;
        return query!;
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
        _parentRelations.Add((parent, child));
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