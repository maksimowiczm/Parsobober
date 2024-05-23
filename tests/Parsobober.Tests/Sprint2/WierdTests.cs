namespace Parsobober.Tests.Sprint2;

public class WierdTests() : BaseTestClass(Code.ZadanieDomowe1)
{
    [Theory]
    // Useless such that clauses
    [InlineData("""
                assign a; stmt s, s1, s2, s3, s4;
                Select a such that Parent(s, a) and Parent(s1, s2) and Parent(s3, s4) 
                """, "9,11,12,13,14,16,19,21,25,26")]
    public void WierdTest(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected);
    }
}