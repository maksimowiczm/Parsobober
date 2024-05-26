namespace Parsobober.Tests.Sprint2;

public class TripleRelationTests() : BaseTestClass(Code.ZadanieDomowe1)
{
    [Theory]
    // Mateusz
    [InlineData("""
                stmt s; if i; while w; assign a;
                Select a such that Parent(w, i) and Parent(i, s) and Uses(a, t)
                """, "25,26")]
    [InlineData("""
                stmt s; while w; assign a;
                Select a such that Parent(w, s) and Parent(s, a) and Uses(a, t)
                """, "25,26")]
    [InlineData("""
                stmt s; while w; assign a;
                Select w such that Parent(w, s) and Parent(s, a) and Uses(a, t)
                """, "23")]
    public void TripleRelationTest(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected);
    }
}