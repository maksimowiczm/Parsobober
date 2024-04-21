using FluentAssertions;
using Moq;
using Parsobober.Pql.Query;
using Xunit;

namespace Parsobober.Pql.Parser.Tests;

public class PqlParserTests
{
    [Fact]
    public void Select_Parent()
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQuery>();
        var parser = new PqlParser(builderMock.Object);

        const string queryString = "stmt s1, s2; Select s1 such that Parent (s1, s2)";
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s1, s2;"));
        builderMock.Verify(b => b.AddSelect("s1"));
        builderMock.Verify(b => b.AddParent("s1", "s2"));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Fact]
    public void Select_ParentTransitive()
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQuery>();
        var parser = new PqlParser(builderMock.Object);

        const string queryString = "stmt s1, s2; Select s1 such that Parent* (s1, s2)";
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s1, s2;"));
        builderMock.Verify(b => b.AddSelect("s1"));
        builderMock.Verify(b => b.AddParentTransitive("s1", "s2"));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Fact]
    public void Select_Modifies()
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQuery>();
        var parser = new PqlParser(builderMock.Object);

        const string queryString = "stmt s1, s2; Select s1 such that Modifies (s1, s2)";
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s1, s2;"));
        builderMock.Verify(b => b.AddSelect("s1"));
        builderMock.Verify(b => b.AddModifies("s1", "s2"));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Fact]
    public void Select_Follows()
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQuery>();
        var parser = new PqlParser(builderMock.Object);

        const string queryString = "stmt s1, s2; Select s1 such that Follows (s1, s2)";
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s1, s2;"));
        builderMock.Verify(b => b.AddSelect("s1"));
        builderMock.Verify(b => b.AddFollows("s1", "s2"));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Fact]
    public void Select_FollowsTransitive()
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQuery>();
        var parser = new PqlParser(builderMock.Object);

        const string queryString = "stmt s1, s2; Select s1 such that Follows* (s1, s2)";
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s1, s2;"));
        builderMock.Verify(b => b.AddSelect("s1"));
        builderMock.Verify(b => b.AddFollowsTransitive("s1", "s2"));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Fact]
    public void Select_With()
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQuery>();
        var parser = new PqlParser(builderMock.Object);

        const string queryString = "stmt s1, s2; Select s1 with s1.stmt# = 1";
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s1, s2;"));
        builderMock.Verify(b => b.AddSelect("s1"));
        builderMock.Verify(b => b.With("s1.stmt#", "1"));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Fact]
    public void Select_Uses()
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQuery>();
        var parser = new PqlParser(builderMock.Object);
        
        const string queryString = "stmt s1, s2; Select s1 such that Uses (s1, s2)";
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);
        
        // Act
        var query = parser.Parse(queryString);
        
        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s1, s2;"));
        builderMock.Verify(b => b.AddSelect("s1"));
        builderMock.Verify(b => b.AddUses("s1", "s2"));
        builderMock.Verify(b => b.Build(), Times.Once);
    }
}