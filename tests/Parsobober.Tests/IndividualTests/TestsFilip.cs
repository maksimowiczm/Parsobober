namespace Parsobober.Tests.IndividualTests;

public class TestsFilip() : BaseTestClass(Code.Dropbox)
{
    [Theory]
    [InlineData("procedure m;\nSelect BOOLEAN such that Calls(\"TT\", m)", "true")]
    [InlineData("if k, l;Select l such that Parent*(l, k)", "14,15,34,66,80,107,159,160,216,266")]
    [InlineData("variable j;Select j such that Modifies(10, j)", "y2")]
    [InlineData("call h; assign i;Select h such that Follows(h, i)", "1,18,45,63,84,114,222,227,259,307")]
    [InlineData("if a;Select a such that Modifies(a, \"j\")", "14,15,66,72,80,97,250")]
    [InlineData("procedure e;Select e such that Uses(e, \"factor\")", "Main,Move,Shear,Shrink")]
    [InlineData("procedure d;Select d such that Calls*(d, \"Shift\")", "Main")]
    [InlineData("procedure c;Select c such that Calls(c,\"Transform\")", "Main")]
    [InlineData("variable j;Select j such that Modifies(20,j)", "difference")]
    [InlineData("while b;Select b such that Uses(b, \"I\")",
        "6,12,16,59,69,79,83,89,101,103,180,181,184,187,251")]
    [InlineData("if a;Select a such that Uses(a, \"j\")", "14,15,66,72,76,80,250")]
    public void TestQuerySingleFilip(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }

    [Theory]
    [InlineData("stmt s, s1; Select s such that Follows(s, s1) and Modifies(s, \"tmp\") and Uses(s1, \"tmp\")",
        "14,16,29,60,90,95,96,137")]
    [InlineData("stmt s; Select s such that Modifies(s, \"width\") and Parent*(s, 8)", "6")]
    [InlineData("stmt s, s1;\nSelect s such that Follows(s, s1) and Uses(s, \"area\") and Modifies(s1, \"height\")",
        "none")]
    [InlineData("stmt s; Select s such that Uses(s, \"tmp\") and Modifies(s, \"area\") and Follows*(1, s)", "6")]
    [InlineData("assign a; Select a such that Modifies(a, \"area\") and Uses(a, \"width\") and Uses(a, \"height\")",
        "11,32")]
    [InlineData("stmt s; if i; Select s such that Parent(s, i) and Uses(i, \"tmp\") and Uses(s, \"width\")",
        "12,14,15,16,34,66,79,105")]
    [InlineData("stmt s, s1; Select s such that Follows(s, s1) and Modifies(s, \"height\") and Uses(s1, \"width\")",
        "16,29,31")]
    [InlineData("while w; Select w such that Uses(w, \"difference\") and Modifies(w, \"tmp\")", "6,12,16,59,79,89,105")]
    [InlineData("stmt s; Select s such that Modifies(s, \"length\") and Uses(s, \"width\") and Uses(s, \"height\")",
        "6,12,14,15,16")]
    [InlineData("variable v; Select v such that Uses(\"Shear\", v) and Modifies(\"Shear\", v)",
        "decrement,factor,incre,x1,x2,y1,y2")]
    [InlineData("assign a; Select a such that Uses(a, \"dx\") and Modifies(a, \"dy\")", "194")]
    public void TestQueryMultipleFilip(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }
}