namespace Parsobober.Tests.Sprint2;

public class DoubleRelationTests() : BaseTestClass(Code.ZadanieDomowe1)
{
    [Theory]
    // Mateusz
    [InlineData("stmt s; if i; while w; Select i such that Parent(i, w) and Parent(w, s)", "8")]
    [InlineData("stmt s; if i; while w; Select w such that Parent(i, w) and Parent(w, s)", "10")]
    [InlineData("stmt s; if i; while w; Select s such that Parent(i, w) and Parent(w, s)", "11,12")]
    [InlineData("stmt s; while w; assign a; Select w such that Parent(w, s) and Parent(s, a)", "23")]
    public void DoubleRelationTest(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected);
    }
}