namespace Parsobober.Tests.Sprint3;

public class NextTests() : BaseTestClass(Code.ZadanieDomowe1)
{
    [Theory]
    [InlineData("prog_line pl, pl1;Select pl such that Next(pl, pl1)",
        "1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 18, 19, 20, 21, 23, 24, 25, 26")]
    [InlineData("prog_line pl, pl1;Select pl1 such that Next(pl, pl1)",
        "2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27")]
    public void NextTest(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }
}