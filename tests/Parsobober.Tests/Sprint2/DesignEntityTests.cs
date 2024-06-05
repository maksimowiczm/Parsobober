namespace Parsobober.Tests.Sprint2;

public class DesignEntityTests() : BaseTestClass(Code.ZadanieDomowe1)
{
    [Theory]
    [InlineData("stmtLst sl; stmt s; Select sl such that Parent(8,9)",
        "1, 9, 11, 14, 18, 19, 23, 24, 25, 26, 28")]
    [InlineData("constant c; stmt s; Select c such that Parent(8,9)",
        "1, 2, 3, 10, 20")]
    [InlineData("prog_line pl; Select pl such that Parent(8,9)",
        "1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28")]
    public void DesignEntityTest(string query, string expected)
    {
        // Act
        var result = App.Query(query);

        // Assert
        result.Should().Be(expected.Replace(" ", ""));
    }
}