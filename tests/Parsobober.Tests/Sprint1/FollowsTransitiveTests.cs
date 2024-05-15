namespace Parsobober.Tests.Sprint1;

public class FollowsTransitiveTestsShort() : Sprint1BaseTestClass(Code1)
{
    [Theory]
    // stmt
    [InlineData("stmt s;", "s", "1", "")]
    [InlineData("stmt s;", "s", "2", "")]
    [InlineData("stmt s;", "s", "3", "")]
    [InlineData("stmt s;", "s", "4", "")]
    [InlineData("stmt s;", "s", "5", "")]
    [InlineData("stmt s;", "s", "6", "")]
    // while
    [InlineData("while w;", "w", "1", "")]
    [InlineData("while w;", "w", "2", "")]
    [InlineData("while w;", "w", "3", "")]
    [InlineData("while w;", "w", "4", "")]
    [InlineData("while w;", "w", "5", "")]
    [InlineData("while w;", "w", "6", "")]
    // assign
    [InlineData("assign a;", "a", "1", "")]
    [InlineData("assign a;", "a", "2", "")]
    [InlineData("assign a;", "a", "3", "")]
    [InlineData("assign a;", "a", "4", "")]
    [InlineData("assign a;", "a", "6", "")]
    [InlineData("assign a;", "a", "5", "")]
    public void FollowsTransitive_Statement_Line(string declaration, string select, string line, string expected)
    {
        var query = $"{declaration}\nSelect {select} such that Follows* ({select}, {line})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("stmt s;", "1", "s", "")]
    [InlineData("stmt s;", "2", "s", "")]
    [InlineData("stmt s;", "3", "s", "")]
    [InlineData("stmt s;", "4", "s", "")]
    [InlineData("stmt s;", "5", "s", "")]
    [InlineData("stmt s;", "6", "s", "")]
    // while
    [InlineData("while w;", "1", "w", "")]
    [InlineData("while w;", "2", "w", "")]
    [InlineData("while w;", "3", "w", "")]
    [InlineData("while w;", "4", "w", "")]
    [InlineData("while w;", "5", "w", "")]
    [InlineData("while w;", "6", "w", "")]
    // assign
    [InlineData("assign a;", "1", "a", "")]
    [InlineData("assign a;", "2", "a", "")]
    [InlineData("assign a;", "3", "a", "")]
    [InlineData("assign a;", "4", "a", "")]
    [InlineData("assign a;", "5", "a", "")]
    [InlineData("assign a;", "6", "a", "")]
    public void FollowsTransitive_Line_Statement(string declaration, string line, string select, string expected)
    {
        var query = $"{declaration}\nSelect {select} such that Follows* ({line}, {select})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("stmt s1, s2;", "s1", "s2", "")]
    [InlineData("stmt s; assign a;", "s", "a", "")]
    [InlineData("stmt s; while w;", "s", "w", "")]
    // while
    [InlineData("while w1, w2;", "w1", "w2", "")]
    [InlineData("while w; assign a;", "w", "a", "")]
    [InlineData("while w; stmt s;", "w", "s", "")]
    // assign
    [InlineData("assign a1, a2;", "a1", "a2", "")]
    [InlineData("assign a; stmt s;", "a", "s", "")]
    [InlineData("assign a; while w;", "a", "w", "")]
    public void FollowsTransitive_Statement_Statement_Select_Left(string declaration, string left, string right,
        string expected)
    {
        var query = $"{declaration}\nSelect {left} such that Follows* ({left}, {right})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("stmt s1, s2;", "s2", "s1", "")]
    [InlineData("stmt s; assign a;", "a", "s", "")]
    [InlineData("stmt s; while w;", "w", "s", "")]
    // while
    [InlineData("while w1, w2;", "w2", "w1", "")]
    [InlineData("while w; assign a;", "a", "w", "")]
    [InlineData("while w; stmt s;", "s", "w", "")]
    // assign
    [InlineData("assign a1, a2;", "a2", "a1", "")]
    [InlineData("assign a; stmt s;", "s", "a", "")]
    [InlineData("assign a; while w;", "w", "a", "")]
    public void FollowsTransitive_Statement_Statement_Select_Right(string declaration, string left, string right,
        string expected)
    {
        var query = $"{declaration}\nSelect {right} such that Follows* ({left}, {right})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }
}

public class FollowsTransitiveTestsLong() : Sprint1BaseTestClass(Code1)
{
    [Theory]
    // stmt
    [InlineData("stmt s;", "s", "7", "")]
    [InlineData("stmt s;", "s", "11", "")]
    // while
    [InlineData("while w;", "w", "11", "")]
    public void FollowsTransitive_Statement_Line(string declaration, string select, string line, string expected)
    {
        var query = $"{declaration}\nSelect {select} such that Follows* ({select}, {line})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("stmt s;", "2", "s", "")]
    [InlineData("stmt s;", "17", "s", "")]
    // while
    [InlineData("while w;", "2", "w", "")]
    [InlineData("while w;", "15", "w", "")]
    public void FollowsTransitive_Line_Statement(string declaration, string line, string select, string expected)
    {
        var query = $"{declaration}\nSelect {select} such that Follows* ({line}, {select})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("stmt s1, s2;", "s1", "s2", "")]
    // while
    [InlineData("stmt s; while w;", "w", "s", "")]
    [InlineData("while w1, w2;", "w1", "w2", "")]
    // assign
    [InlineData("stmt s; assign a;", "a", "s", "")]
    public void FollowsTransitive_Statement_Statement_Select_Left(string declaration, string left, string right,
        string expected)
    {
        var query = $"{declaration}\nSelect {left} such that Follows* ({left}, {right})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("stmt s1, s2;", "s2", "s1", "")]
    [InlineData("stmt s; assign a;", "a", "s", "None")]
    public void FollowsTransitive_Statement_Statement_Select_Right(string declaration, string left, string right,
        string expected)
    {
        var query = $"{declaration}\nSelect {right} such that Follows* ({left}, {right})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }
}