namespace Parsobober.Tests.Sprint1;

public class ParentTests() : Sprint1BaseTestClass(Code1)
{
    [Theory]
    // stmt
    [InlineData("stmt s;", "s", "1", "None")]
    [InlineData("stmt s;", "s", "2", "1")]
    [InlineData("stmt s;", "s", "3", "1")]
    [InlineData("stmt s;", "s", "4", "1")]
    [InlineData("stmt s;", "s", "5", "4")]
    [InlineData("stmt s;", "s", "6", "1")]
    // while
    [InlineData("while w;", "w", "1", "None")]
    [InlineData("while w;", "w", "2", "1")]
    [InlineData("while w;", "w", "3", "1")]
    [InlineData("while w;", "w", "4", "1")]
    [InlineData("while w;", "w", "5", "4")]
    [InlineData("while w;", "w", "6", "1")]
    // assign
    [InlineData("assign a;", "a", "1", "None")]
    [InlineData("assign a;", "a", "2", "None")]
    [InlineData("assign a;", "a", "3", "None")]
    [InlineData("assign a;", "a", "4", "None")]
    [InlineData("assign a;", "a", "6", "None")]
    [InlineData("assign a;", "a", "5", "None")]
    public void Parent_Statement_Line(string declaration, string select, string line, string expected)
    {
        var query = $"{declaration}\nSelect {select} such that Parent ({select}, {line})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("stmt s;", "1", "s", "2,3,4,6")]
    [InlineData("stmt s;", "2", "s", "None")]
    [InlineData("stmt s;", "3", "s", "None")]
    [InlineData("stmt s;", "4", "s", "5")]
    [InlineData("stmt s;", "5", "s", "None")]
    [InlineData("stmt s;", "6", "s", "None")]
    // while
    [InlineData("while w;", "1", "w", "2,3,4,6")]
    [InlineData("while w;", "2", "w", "None")]
    [InlineData("while w;", "3", "w", "None")]
    [InlineData("while w;", "4", "w", "5")]
    [InlineData("while w;", "5", "w", "None")]
    [InlineData("while w;", "6", "w", "None")]
    // assign
    [InlineData("assign a;", "1", "a", "None")]
    [InlineData("assign a;", "2", "a", "None")]
    [InlineData("assign a;", "3", "a", "None")]
    [InlineData("assign a;", "4", "a", "None")]
    [InlineData("assign a;", "5", "a", "None")]
    [InlineData("assign a;", "6", "a", "None")]
    public void Parent_Line_Statement(string declaration, string line, string select, string expected)
    {
        var query = $"{declaration}\nSelect {select} such that Parent ({line}, {select})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("stmt s1, s2;", "s1", "s2", "1,4")]
    [InlineData("stmt s; assign a;", "s", "a", "1,4")]
    [InlineData("stmt s; while w;", "s", "w", "1")]
    // while
    [InlineData("while w1, w2;", "w1", "w2", "1")]
    [InlineData("while w; assign a;", "w", "a", "1,4")]
    [InlineData("while w; stmt s;", "w", "s", "1,4")]
    // assign
    [InlineData("assign a1, a2;", "a1", "a2", "None")]
    [InlineData("assign a; stmt s;", "a", "s", "None")]
    [InlineData("assign a; while w;", "a", "w", "None")]
    public void Parent_Statement_Statement_Select_Left(string declaration, string left, string right, string expected)
    {
        var query = $"{declaration}\nSelect {left} such that Parent ({left}, {right})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("stmt s1, s2;", "s2", "s1", "2,3,4,5,6")]
    [InlineData("stmt s; assign a;", "a", "s", "2,3,5,6")]
    [InlineData("stmt s; while w;", "w", "s", "4")]
    // while
    [InlineData("while w1, w2;", "w2", "w1", "4")]
    [InlineData("while w; assign a;", "a", "w", "2,3,5,6")]
    [InlineData("while w; stmt s;", "s", "w", "2,3,4,5,6")]
    // assign
    [InlineData("assign a1, a2;", "a2", "a1", "None")]
    [InlineData("assign a; stmt s;", "s", "a", "None")]
    [InlineData("assign a; while w;", "w", "a", "None")]
    public void Parent_Statement_Statement_Select_Right(string declaration, string left, string right, string expected)
    {
        var query = $"{declaration}\nSelect {right} such that Parent ({left}, {right})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }
}