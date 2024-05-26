namespace Parsobober.Tests.Sprint2;

public class WierdTests() : BaseTestClass(Code.ZadanieDomowe1)
{
    [Theory]
    [InlineData("""
                assign a; stmt s, s1, s2;
                Select a such that Parent(s, a) and Parent(s1, s2)
                """, "9,11,12,13,14,16,19,21,25,26"
    )]
    [InlineData("""
                assign a; stmt s, s1, s2, s3;
                Select a such that Parent(s, a) and Parent(s1, s2) and Parent(s2, s3) 
                """, "9,11,12,13,14,16,19,21,25,26")]
    [InlineData("""
                assign a; stmt s, s1, s2, s3, s4;
                Select a such that Parent(s, a) and Parent(s1, s2) and Parent(s3, s4) 
                """, "9,11,12,13,14,16,19,21,25,26")]
    [InlineData("""
                assign a; stmt s;
                Select a such that Parent(s, a) and Parent(s, a) and Parent(s, a)
                """, "9,11,12,13,14,16,19,21,25,26")]
    [InlineData("""
                assign a; stmt s;
                Select a such that Parent(s, a) and Parent(s, a) and Parent(s, a) with a.stmt# = 9
                """, "9")]
    [InlineData("""
                stmt s; assign a; while w;
                Select a such that Parent(w, s) with w.stmt# = 1 and s.stmt# = 2
                """, "None")]
    [InlineData("""
                stmt s; assign a; while w;
                Select a such that Parent(w, s) with w.stmt# = 10 and s.stmt# = 11
                """, "1,2,3,5,7,9,11,12,13,14,16,19,21,22,25,26,27,28")]
    public void WierdTest(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected);
    }
}