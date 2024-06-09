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
                    return rightNode._Equals_(x.Value.Children[1]);
                }

                return rightNode.Equals(x.Value.Children[1]);
            })
            .ToList();

        return results.Select(r => r.Value.ToStatement());
    }

    private IEnumerable<IPkbDto> HandleLeft()
    {
        throw new NotImplementedException();
    }

    private IEnumerable<IPkbDto> HandleBoth()
    {
        throw new NotImplementedException();
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