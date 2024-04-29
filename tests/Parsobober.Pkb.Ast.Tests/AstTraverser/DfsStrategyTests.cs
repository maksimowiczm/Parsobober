using Microsoft.Extensions.Logging;
using Parsobober.Pkb.Ast.Abstractions;
using Parsobober.Pkb.Ast.AstTraverser.Strategies;
using Parsobober.Simple.Lexer;
using Parsobober.Simple.Parser;
using Parsobober.Simple.Parser.Abstractions;
using Xunit.Abstractions;

namespace Parsobober.Pkb.Ast.Tests.AstTraverser;

public class DfsStrategyTests
{
    private readonly IAst _ast;
    private readonly ITestOutputHelper _testOutputHelper;

    private readonly string _testProgram = """
                                           procedure First {
                                             d = z;
                                             while y {
                                                 x = a;
                                                 while a {
                                                     while a {
                                                         c = z;
                                                         a = z;
                                                     }
                                                     x = z;
                                                 }
                                                 b = c;
                                                 a = z;
                                             }
                                             d = y;
                                           }
                                           """;

    public DfsStrategyTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        var parserBuilder = new SimpleParserBuilder(
            new Mock<ILogger<SimpleParserBuilder>>().Object,
            new Ast(),
            new Mock<ILogger<SimpleParser>>().Object,
            Enumerable.Empty<ISimpleExtractor>(),
            new SlyLexerAdapter(new Mock<ILogger<SlyLexerAdapter>>().Object)
        );
        _ast = parserBuilder.BuildParser(_testProgram).Parse();
    }

    [Fact]
    public void StatementTraverse_ShouldReturnNodesInDepthFirstOrder_WhenCalledWithValidRoot()
    {
        // Arrange
        var strategy = new DfsStatementStrategy();

        // Act
        var result = strategy.Traverse(_ast.Root).ToList();

        // Assert
        result.Count.Should().Be(16);

        result[0].node.LineNumber.Should().Be(0);
        result[5].node.LineNumber.Should().Be(3);
        result[10].node.LineNumber.Should().Be(6);
        result[14].node.LineNumber.Should().Be(10);

        result[0].depth.Should().Be(1);
        result[1].depth.Should().Be(2);
        result[3].depth.Should().Be(3);
        result[5].depth.Should().Be(5);
        result[7].depth.Should().Be(6);
        result[11].depth.Should().Be(9);
    }
}