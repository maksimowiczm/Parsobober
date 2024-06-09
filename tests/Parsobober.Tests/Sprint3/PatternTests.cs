namespace Parsobober.Tests.Sprint3;

public class PatternTests() : BaseTestClass(Code.Dropbox)
{
    [Theory]
    [InlineData("assign a;\nSelect a pattern a(_, \"width + incre + left\")", "7")]
    [InlineData("assign a;\nSelect a pattern a(_, \"length + x1\")", "7")]
    [InlineData("assign a;\nSelect a pattern a(_, _\"y2 - y1\"_)", "50,141,168,249")]
    [InlineData("assign a;\nSelect a pattern a(_, \"volume * 11 + volume - x9 + volume\")", "58")]
    [InlineData("assign a;\nSelect a pattern a(_, \"I - (k + j * decrement)\")", "104")]
    [InlineData("assign a;\nSelect a pattern a( \"x\",_)", "106, 116")]
    public void PatternTest(string query, string expected)
    {
        var result = App.Query(query);
        Assert.Equal(expected, result);
    }
}