namespace Parsobober.Tests.Sprint3;

public class TestsUnderscore() : BaseTestClass(Code.Dropbox)
{
    [Theory]
    [InlineData("""
                Select BOOLEAN such that Follows(_, 1)
                """, "false")]
    [InlineData("""
                Select BOOLEAN such that Follows(1, _)
                """, "true")]
    [InlineData("""
                Select BOOLEAN such that Follows(_, _)
                """, "true")]
    [InlineData("""
                Select BOOLEAN such that Follows*(_, 1)
                """, "false")]
    [InlineData("""
                Select BOOLEAN such that Follows*(1, _)
                """, "true")]
    [InlineData("""
                Select BOOLEAN such that Follows*(_, _)
                """, "true")]
    [InlineData("""
                Select BOOLEAN such that Parent(_, 6)
                """, "false")]
    [InlineData("""
                Select BOOLEAN such that Parent(6, _)
                """, "true")]
    [InlineData("""
                Select BOOLEAN such that Parent(_, _)
                """, "true")]
    [InlineData("""
                Select BOOLEAN such that Parent*(_, 6)
                """, "false")]
    [InlineData("""
                Select BOOLEAN such that Parent*(6, _)
                """, "true")]
    [InlineData("""
                Select BOOLEAN such that Parent*(_, _)
                """, "true")]
    [InlineData("""
                Select BOOLEAN such that Calls(_, "Main")
                """, "false")]
    [InlineData("""
                Select BOOLEAN such that Calls("Main", _)
                """, "true")]
    [InlineData("""
                Select BOOLEAN such that Calls(_, _)
                """, "true")]
    [InlineData("""
                Select BOOLEAN such that Calls*(_, "Main")
                """, "false")]
    [InlineData("""
                Select BOOLEAN such that Calls*("Main", _)
                """, "true")]
    [InlineData("""
                Select BOOLEAN such that Calls*(_, _)
                """, "true")]
    [InlineData("""
                Select BOOLEAN such that Next(_, 1)
                """, "false")]
    [InlineData("""
                Select BOOLEAN such that Next(1, _)
                """, "true")]
    [InlineData("""
                Select BOOLEAN such that Next(_, _)
                """, "true")]
    [InlineData("""
                Select BOOLEAN such that Next*(_, 1)
                """, "false")]
    [InlineData("""
                Select BOOLEAN such that Next*(1, _)
                """, "true")]
    [InlineData("""
                Select BOOLEAN such that Next*(_, _)
                """, "true")]
    [InlineData("""
                Select BOOLEAN such that Modifies(1, _)
                """, "true")]
    [InlineData("""
                Select BOOLEAN such that Modifies(2, _)
                """, "true")]
    [InlineData("""
                Select BOOLEAN such that Uses(1, _)
                """, "false")]
    [InlineData("""
                Select BOOLEAN such that Uses(7, _)
                """, "true")]
    public void UnderscoreBooleanTests(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }

    [Theory]
    [InlineData("""
                stmt s;
                Select s such that Next(_, _) and Modifies(s, _) and Parent(_, s) and Parent(s, _) and Follows(_, s) and Follows(s, _)
                """,
        "12, 14, 23, 29, 34, 47, 55, 72, 95, 97, 101, 140, 143, 163, 166, 170, 184, 196, 198, 204, 224, 230, 234, 303")]
    [InlineData("""
                stmt s; while w;
                Select s such that Follows*(w, s) and Follows(s, _)
                """,
        "34, 45, 46, 47, 54, 55, 63, 64, 65, 101, 186, 204, 234")]
    [InlineData("""
                procedure p;
                Select p such that Calls(p,_)
                """,
        "Draw, Enlarge, Main, PP, RR, Rotate, SS, Shrink, TT, Translate, UU")]
    [InlineData("""
                procedure p;
                Select p such that Calls(_,p)
                """,
        "Clear, Draw, Enlarge, Fill, Init, Move, PP, QQ, Random, Rotate, SS, Shear, Shift, Show, Shrink, TT, Transform, Translate, UU, XX")]
    [InlineData("""
                stmt s;
                Select s such that Parent(_, s) and Parent*(s, _) and Uses(s, _) and Follows(s, _)
                """,
        "12, 14, 16, 23, 29, 34, 47, 55, 72, 95, 97, 101, 113, 140, 143, 163, 166, 170, 184, 192, 196, 198, 204, 218, 224, 230, 234, 251, 265, 266, 267, 303")]
    public void UnderscoreTests(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }
}