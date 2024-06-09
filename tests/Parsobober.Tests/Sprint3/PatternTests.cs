namespace Parsobober.Tests.Sprint3;

public class PatternTests() : BaseTestClass(Code.Dropbox)
{
    [Theory]
    [InlineData("assign a;\nSelect a pattern a(_, \"width + incre + left\")", "7")]
    public void PatternTest(string query, string expected)
    {
        var result = App.Query(query);
        Assert.Equal(expected, result);
    }
}