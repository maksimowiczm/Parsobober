using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pkb.Relations.Utilities;
using Parsobober.Pql.Pattern.Parser.Abstractions;
using Parsobober.Pql.Query.Abstractions;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;
using Pat = Parsobober.Pql.Query.Arguments.Pattern;

namespace Parsobober.Pql.Query.Queries;

public class PatternQuery(
    IArgument argument,
    Pat left,
    Pat right,
    IDtoProgramContextAccessor dtoContext,
    IProgramContextAccessor programContext,
    IPatternParserBuilder parserBuilder
) : IQueryDeclaration
{
    public IArgument Left => argument;
    public IArgument Right => argument;

    private Pat LeftPattern => left;
    private Pat RightPattern => right;

    private IReadOnlyDictionary<int, TreeNode> GetInput()
    {
        if (argument is IDeclaration)
        {
            return programContext.StatementsDictionary;
        }

        if (argument is not Line line)
        {
            throw new WtfException("Pattern query does not have declaration or line as given argument.");
        }

        return programContext.StatementsDictionary
            .Where(x => x.Key == line.Value)
            .ToDictionary(x => x.Key, x => x.Value);
    }

    public IEnumerable<IPkbDto> Do()
    {
        var result = (LeftPattern.Value, RightPattern.Value) switch
        {
            ("_", "_") => HandleAll(),
            ("_", _) => HandleRight(),
            (_, "_") => HandleLeft(),
            _ => HandleBoth()
        };

        return result;
    }

    private IEnumerable<IPkbDto> HandleAll()
    {
        if (argument is IDeclaration declaration)
        {
            return declaration.ExtractFromContext(dtoContext);
        }

        if (argument is Line line)
        {
            return Enumerable.Repeat(programContext.StatementsDictionary[line.Value].ToStatement(), 1);
        }

        throw new WtfException("Pattern query does not have declaration or line as given argument.");
    }

    private IEnumerable<IPkbDto> HandleRight()
    {
        var rightPattern = RightPattern.Value;

        var underscore = rightPattern.Any(c => c == '_');

        var rightNode = parserBuilder.BuildParser(rightPattern.Replace("\"", "").Replace("_", "")).Parse();

        var results = GetInput()
            .Where(x => x.Value.Children.Count > 1)
            .Where(x =>
            {
                if (underscore)
                {
                    return x.Value.Children[1]._Equals_(rightNode);
                }

                return x.Value.Children[1].Equals(rightNode);
            })
            .ToList();

        return results.Select(r => r.Value.ToStatement());
    }

    private IEnumerable<IPkbDto> HandleLeft()
    {
        var leftPattern = LeftPattern.Value;

        var underscore = leftPattern.Any(c => c == '_');

        var leftNode = parserBuilder.BuildParser(leftPattern.Replace("\"", "").Replace("_", "")).Parse();

        var results = GetInput()
            .Where(x => x.Value.Children.Count > 0)
            .Where(x =>
            {
                if (underscore)
                {
                    return x.Value.Children[0]._Equals_(leftNode);
                }

                return x.Value.Children[0].Equals(leftNode);
            })
            .ToList();

        return results.Select(r => r.Value.ToStatement());
    }

    private IEnumerable<IPkbDto> HandleBoth()
    {
        var leftPattern = LeftPattern.Value;
        var leftUnderscore = leftPattern.Any(c => c == '_');
        var leftNode = parserBuilder.BuildParser(leftPattern.Replace("\"", "").Replace("_", "")).Parse();

        var rightPattern = RightPattern.Value;
        var rightUnderscore = rightPattern.Any(c => c == '_');
        var rightNode = parserBuilder.BuildParser(rightPattern.Replace("\"", "").Replace("_", "")).Parse();

        var results = GetInput()
            .Where(x => x.Value.Children.Count > 1)
            .Where(x =>
            {
                if (leftUnderscore && rightUnderscore)
                {
                    return x.Value.Children[0]._Equals_(leftNode) && x.Value.Children[1]._Equals_(rightNode);
                }
                if (leftUnderscore)
                {
                    return x.Value.Children[0]._Equals_(leftNode) && x.Value.Children[1].Equals(rightNode);
                }
                if (rightUnderscore)
                {
                    return x.Value.Children[0].Equals(leftNode) && x.Value.Children[1]._Equals_(rightNode);
                }

                return x.Value.Children[0].Equals(leftNode) && x.Value.Children[1].Equals(rightNode);
            })
            .ToList();

        return results.Select(r => r.Value.ToStatement());
    }

    public IEnumerable<IPkbDto> Do(IDeclaration select)
    {
        throw new NotImplementedException();
    }

    public IQueryDeclaration ReplaceArgument(IDeclaration select, IArgument replacement) =>
        new PatternQuery(replacement, LeftPattern, RightPattern, dtoContext, programContext, parserBuilder);

#if DEBUG
    public override string ToString() => $"Pattern {argument}({LeftPattern}, {RightPattern})";
#endif
}