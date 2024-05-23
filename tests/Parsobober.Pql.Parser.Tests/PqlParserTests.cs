using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Parser.Tests;

public class PqlParserTests
{
    [Fact]
    public void Select_Parent()
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
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
        var queryMock = new Mock<IQueryResult>();
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
        var queryMock = new Mock<IQueryResult>();
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
        var queryMock = new Mock<IQueryResult>();
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
        var queryMock = new Mock<IQueryResult>();
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
        var queryMock = new Mock<IQueryResult>();
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
        var queryMock = new Mock<IQueryResult>();
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

    [Fact]
    public void Select_Parent_Integer()
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
        var parser = new PqlParser(builderMock.Object);

        const string queryString = "stmt s1; Select s1 such that Parent(s1, 10)";
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s1;"));
        builderMock.Verify(b => b.AddSelect("s1"));
        builderMock.Verify(b => b.AddParent("s1", "10"));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Fact]
    public void ParseSingleRelationWithMultipleWith()
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
        var parser = new PqlParser(builderMock.Object);

        const string queryString = """
                                   stmt s1;
                                   Select s1 such that Parent(s1, 10)
                                   with s1.stmt# = 5 and s1.value = 10
                                   """;
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s1;"));
        builderMock.Verify(b => b.AddSelect("s1"));
        builderMock.Verify(b => b.AddParent("s1", "10"));
        builderMock.Verify(b => b.With("s1.stmt#", "5"));
        builderMock.Verify(b => b.With("s1.value", "10"));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Fact]
    public void ParseDoubleRelation()
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
        var parser = new PqlParser(builderMock.Object);

        const string queryString = "stmt s; while w, w1; Select w such that Parent(w, w1) and Parent(w1, s)";
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s;"));
        builderMock.Verify(b => b.AddDeclaration("while w, w1;"));
        builderMock.Verify(b => b.AddSelect("w"));
        builderMock.Verify(b => b.AddParent("w", "w1"));
        builderMock.Verify(b => b.AddParent("w1", "s"));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Fact]
    public void ParseDoubleRelationWith()
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
        var parser = new PqlParser(builderMock.Object);

        const string queryString =
            "stmt s; while w, w1; Select w such that Parent(w, w1) and Parent(w1, s) with w.stmt# = 5";
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s;"));
        builderMock.Verify(b => b.AddDeclaration("while w, w1;"));
        builderMock.Verify(b => b.AddSelect("w"));
        builderMock.Verify(b => b.AddParent("w", "w1"));
        builderMock.Verify(b => b.AddParent("w1", "s"));
        builderMock.Verify(b => b.With("w.stmt#", "5"));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Fact]
    public void ParseDoubleRelationWithMultipleWith()
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
        var parser = new PqlParser(builderMock.Object);

        const string queryString = """
                                   stmt s; while w, w1;
                                   Select w such that Parent(w, w1) and Parent(w1, s)
                                   with w.stmt# = 5 and w1.stmt# = 7
                                   """;
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s;"));
        builderMock.Verify(b => b.AddDeclaration("while w, w1;"));
        builderMock.Verify(b => b.AddSelect("w"));
        builderMock.Verify(b => b.AddParent("w", "w1"));
        builderMock.Verify(b => b.AddParent("w1", "s"));
        builderMock.Verify(b => b.With("w.stmt#", "5"));
        builderMock.Verify(b => b.With("w1.stmt#", "7"));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Fact]
    public void ParseTripleRelation()
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
        var parser = new PqlParser(builderMock.Object);

        const string queryString =
            "stmt s; while w, w1, w2; Select w such that Parent(w, w1) and Parent(w1, w2) and Parent(w2, s)";
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s;"));
        builderMock.Verify(b => b.AddDeclaration("while w, w1, w2;"));
        builderMock.Verify(b => b.AddSelect("w"));
        builderMock.Verify(b => b.AddParent("w", "w1"));
        builderMock.Verify(b => b.AddParent("w1", "w2"));
        builderMock.Verify(b => b.AddParent("w2", "s"));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Fact]
    public void ParseTripleRelationWithMultipleWith()
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
        var parser = new PqlParser(builderMock.Object);

        const string queryString = """
                                   stmt s; while w, w1, w2;
                                   Select w such that Parent(w, w1) and Parent(w1, w2) and Parent(w2, s)
                                   with w.stmt# = 5 and w1.stmt# = 7 and w2.stmt# = 10
                                   """;

        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s;"));
        builderMock.Verify(b => b.AddDeclaration("while w, w1, w2;"));
        builderMock.Verify(b => b.AddSelect("w"));
        builderMock.Verify(b => b.AddParent("w", "w1"));
        builderMock.Verify(b => b.AddParent("w1", "w2"));
        builderMock.Verify(b => b.AddParent("w2", "s"));
        builderMock.Verify(b => b.With("w.stmt#", "5"));
        builderMock.Verify(b => b.With("w1.stmt#", "7"));
        builderMock.Verify(b => b.With("w2.stmt#", "10"));
        builderMock.Verify(b => b.Build(), Times.Once);
    }
}