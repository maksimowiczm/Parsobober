namespace Parsobober.Tests.Sprint3;

public class FollowsTests() : BaseTestClass(Code.Dropbox)
{
    [Theory]
    [InlineData("""
                stmt s1, s2, s3, s4;
                Select s1 such that Follows(s1, s2) and Follows(s2, s3) and Follows(s3, s4)
                """,
        "1, 2, 3, 4, 7, 8, 9, 10, 13, 14, 16, 17, 18, 19, 20, 21, 22, 23, 29, 30, 34, 39, 45, 46, 47, 48, 63, 70, 120, 121, 122, 123, 124, 125, 126, 132, 133, 137, 138, 139, 150, 151, 152, 153, 154, 155, 161, 162, 163, 166, 182, 183, 192, 219, 220, 221, 222, 223, 224, 227, 228, 244, 245, 246, 247, 248, 249, 267, 270, 283, 284, 298, 302, 303")]
    [InlineData("""
                stmt s1, s2; assign a1, a2; while w1, w2;
                Select s1 such that Follows(s1, s2) and Follows*(s1, a1) and Follows*(w1, a2) and Follows(w2, a1)
                """,
        "113, 137, 138, 139, 140, 143, 182, 183, 184, 218, 234, 251")]
    [InlineData("""
                if i; while w;
                Select i such that Follows*(i, w)
                """,
        "14, 23, 34, 55, 140, 192")]
    [InlineData("""
                if i; while w; call c;
                Select i such that Follows*(i, w) and Follows(w, c)
                """,
        "23, 34")]
    [InlineData("""
                if i1, i2, i3;
                Select i2 such that Follows*(i1, i2) and Follows(i2, i3)
                """,
        "170")]
    [InlineData("""
                call c, c1, c2;
                Select c such that Follows*(c, c1) and Follows(c1, c2)
                """,
        "295")]
    [InlineData("""
                call c, c1;
                Select c such that Follows*(c, c1) and Follows(c1, _)
                """,
        "1, 18, 22, 45, 221, 222, 295")]
    [InlineData("""
                call c, c1;
                Select c such that Follows*(_, c1) and Follows*(c1, c)
                """,
        "22, 42, 45, 54, 119, 222, 227, 287, 297")]
    [InlineData("""
                if i, i1;
                Select i such that Follows*(_, i1) and Follows*(i1, i)
                """,
        "34, 55, 166, 170, 173, 230")]
    public void FollowsTest(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }
}