namespace Parsobober.Tests.Sprint2;

public class TripleRelationTests() : BaseTestClass(Code.ZadanieDomowe1)
{
    [Theory]
    [InlineData("""
                stmt s; if i; while w; assign a;
                Select a such that Parent(w, i) and Parent(i, s) and Uses(a, t)
                """, "2,3,5,7,11,13,14,22,25,26,27,28")]
    [InlineData("""
                stmt s; while w; assign a;
                Select a such that Parent(w, s) and Parent(s, a) and Uses(a, "t")
                """, "25,26")]
    [InlineData("""
                stmt s; while w; assign a;
                Select w such that Parent(w, s) and Parent(s, a) and Uses(a, "t")
                """, "23")]
    // this test is illegal in sprint 2 (only 3 such that allowed), but it works on this implementation
    [InlineData("""
                stmt s; if i; while w; assign a;
                Select a such that Parent(w, i) and Parent(i, s) and Uses(a, "t") and Modifies(a, "b")
                """, "5,7")]
    public void TripleRelationTest(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected);
    }
}