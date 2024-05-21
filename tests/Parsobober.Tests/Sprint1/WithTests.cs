namespace Parsobober.Tests.Sprint1;

public class WithTests() : BaseTestClass(Code.ShortStatementsOnly)
{
    [Theory]
    [InlineData("stmt s;", "s", "Parent(s, 2)", "s.stmt# = 1", "1")]
    [InlineData("while w;", "w", "Parent(w, 2)", "w.stmt# = 1", "1")]
    [InlineData("while w; assign a;", "a", "Parent(w, 2)", "w.stmt# = 1", "2,3,5,6")]
    [InlineData("stmt s;", "s", "Parent(s, 2)", "s.stmt# = 2", "None")]
    public void Line(string declaration, string select, string relation, string attribute, string expected)
    {
        var query = $"{declaration} Select {select} such that {relation} with {attribute}";
        var result = App.Query(query);
        result.Should().Be(expected);
    }
}