namespace Parsobober.Tests.Sprint2;

public class DesignEntityTests() : BaseTestClass(Code.ZadanieDomowe1)
{
    [Theory]
    [InlineData("stmtLst sl; stmt s; Select sl such that Parent(8,9)",
        "1, 9, 11, 14, 18, 19, 23, 24, 25, 26, 28")]
    [InlineData("constant c; stmt s; Select c such that Parent(8,9)",
        "1, 2, 3, 10, 20")]
    // prog_line is a synonym for stmt
    [InlineData("prog_line pl; Select pl such that Parent(8,9)",
        "1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28")]
    [InlineData("stmt s; prog_line pl;\nSelect s such that Parent(pl, s)",
        "9, 10, 11, 12, 13, 14, 15, 16, 19, 20, 21, 24, 25, 26")]
    [InlineData("stmt s; prog_line pl;\nSelect s such that Follows(pl, s)",
        "2, 3, 4, 5, 6, 7, 8, 10, 12, 13, 15, 16, 17, 20, 21, 22, 27")]
    [InlineData("stmt s; prog_line pl;\nSelect s such that Parent(s, pl)",
        "8, 10, 18, 23, 24")]
    [InlineData("stmt s; prog_line pl;\nSelect s such that Follows(s, pl)",
        "1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 14, 15, 18, 19, 20, 23")]
    [InlineData("prog_line pl, pl1;\nSelect pl such that Parent(pl, pl1)",
        "8, 10, 18, 23, 24")]
    [InlineData("prog_line pl, pl1;\nSelect pl such that Follows(pl, pl1)",
        "1,2,3,4,5,6,7,8,9,10,11,14,15,18,19,20,23")]
    public void DesignEntityTest(string query, string expected)
    {
        // Act
        var result = App.Query(query);

        // Assert
        result.Should().Be(expected.Replace(" ", ""));
    }
}