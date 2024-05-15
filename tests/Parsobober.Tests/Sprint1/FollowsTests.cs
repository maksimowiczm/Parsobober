namespace Parsobober.Tests.Sprint1;

public class FollowsTests() : BaseTestClass(Code.ShortStatementsOnly)
{
    [Theory]
    // stmt
    [InlineData("stmt s;", "s", "1", "None")]
    [InlineData("stmt s;", "s", "2", "None")]
    [InlineData("stmt s;", "s", "3", "2")]
    [InlineData("stmt s;", "s", "4", "3")]
    [InlineData("stmt s;", "s", "5", "None")]
    [InlineData("stmt s;", "s", "6", "4")]
    // while
    [InlineData("while w;", "w", "1", "None")]
    [InlineData("while w;", "w", "2", "None")]
    [InlineData("while w;", "w", "3", "None")]
    [InlineData("while w;", "w", "4", "None")]
    [InlineData("while w;", "w", "5", "None")]
    [InlineData("while w;", "w", "6", "4")]
    // assign
    [InlineData("assign a;", "a", "1", "None")]
    [InlineData("assign a;", "a", "2", "None")]
    [InlineData("assign a;", "a", "3", "2")]
    [InlineData("assign a;", "a", "4", "3")]
    [InlineData("assign a;", "a", "5", "None")]
    [InlineData("assign a;", "a", "6", "None")]

    public void Follows_Statement_Line(string declaration, string select, string line, string expected)
    {
        var query = $"{declaration}\nSelect {select} such that Follows ({select}, {line})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("stmt s;", "1", "s", "None")]
    [InlineData("stmt s;", "2", "s", "3")]
    [InlineData("stmt s;", "3", "s", "4")]
    [InlineData("stmt s;", "4", "s", "6")]
    [InlineData("stmt s;", "5", "s", "None")]
    [InlineData("stmt s;", "6", "s", "None")]
    // while
    [InlineData("while w;", "1", "w", "None")]
    [InlineData("while w;", "2", "w", "None")]
    [InlineData("while w;", "3", "w", "4")]
    [InlineData("while w;", "4", "w", "None")]
    [InlineData("while w;", "5", "w", "None")]
    [InlineData("while w;", "6", "w", "None")]
    // assign
    [InlineData("assign a;", "1", "a", "None")]
    [InlineData("assign a;", "2", "a", "3")]
    [InlineData("assign a;", "3", "a", "None")]
    [InlineData("assign a;", "4", "a", "6")]
    [InlineData("assign a;", "5", "a", "None")]
    [InlineData("assign a;", "6", "a", "None")]
    public void Follows_Line_Statement(string declaration, string line, string select, string expected)
    {
        var query = $"{declaration}\nSelect {select} such that Follows ({line}, {select})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("stmt s1, s2;", "s1", "s2", "3,4,6")]
    [InlineData("stmt s; assign a;", "s", "a", "3,6")]
    [InlineData("stmt s; while w;", "s", "w", "4")]
    // while
    [InlineData("while w1, w2;", "w1", "w2", "None")]
    [InlineData("while w; assign a;", "w", "a", "6")]
    [InlineData("while w; stmt s;", "w", "s", "6")]
    // assign
    [InlineData("assign a1, a2;", "a1", "a2", "3")]
    [InlineData("assign a; stmt s;", "a", "s", "3,4")]
    [InlineData("assign a; while w;", "a", "w", "4")]
    public void Follows_Statement_Statement_Select_Left(string declaration, string left, string right, string expected)
    {
        var query = $"{declaration}\nSelect {left} such that Follows ({left}, {right})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("stmt s1, s2;", "s2", "s1", "3,4,6")]
    [InlineData("stmt s; assign a;", "a", "s", "3,6")]
    [InlineData("stmt s; while w;", "w", "s", "4")]
    // while
    [InlineData("while w1, w2;", "w2", "w1", "None")]
    [InlineData("while w; assign a;", "a", "w", "6")]
    [InlineData("while w; stmt s;", "s", "w", "6")]
    // assign
    [InlineData("assign a1, a2;", "a2", "a1", "3")]
    [InlineData("assign a; stmt s;", "s", "a", "3,4")]
    [InlineData("assign a; while w;", "w", "a", "4")]
    public void Follows_Statement_Statement_Select_Right(string declaration, string left, string right, string expected)
    {
        var query = $"{declaration}\nSelect {right} such that Follows ({left}, {right})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }
}