using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions;
using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Pattern.Parser.Abstractions;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;
using Pat = Parsobober.Pql.Query.Arguments.Pattern;

namespace Parsobober.Pql.Query.Queries;

public class PatternQuery(
    IDeclaration declaration,
    Pat left,
    Pat right,
    IDtoProgramContextAccessor dtoContext,
    IProgramContextAccessor programContext,
    IPatternParserBuilder parserBuilder
) : IQueryDeclaration
{
    public IArgument Left => LeftPattern;
    public IArgument Right => RightPattern;

    private Pat LeftPattern => left;
    private Pat RightPattern => right;

    public IEnumerable<IPkbDto> Do()
    {
        var result = (LeftPattern.Value, RightPattern.Value) switch
        {
            ("_", "_") => declaration.ExtractFromContext(dtoContext),
            ("_", _) => HandleRight(),
            (_, "_") => HandleLeft(),
            _ => HandleBoth()
        };

        return result;
    }

    private IEnumerable<IPkbDto> HandleRight()
    {
        var rightPattern = RightPattern.Value;
        var rightNode = parserBuilder.BuildParser(rightPattern).Parse();

        var assigns = programContext.StatementsDictionary
            .Where(s => declaration switch
            {
                IStatementDeclaration.Assign => s.Value.Type == EntityType.Assign,
                _ => throw new Exception("xd")
            })
            .ToList();

        throw new NotImplementedException();
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

    public IQueryDeclaration ReplaceArgument(IDeclaration select, IArgument replacement)
    {
        throw new NotImplementedException();
    }
}