namespace Parsobober.Tests.Sprint3;

public class PatternTests() : BaseTestClass(Code.Dropbox)
{
    [Theory]
    [InlineData("assign a;\nSelect a pattern a(_, \"width + incre + left\")", "7")]
    [InlineData("assign a;\nSelect a pattern a(_, \"length + x1\")", "43")]
    [InlineData("assign a;\nSelect a pattern a(_, _\"y2 - y1\"_)", "50,141,168,249")]
    [InlineData("assign a;\nSelect a pattern a(_, \"volume * 11 + volume - x9 + volume\")", "58")]
    [InlineData("assign a;\nSelect a pattern a(_, \"I - (k + j * decrement)\")", "104")]
    [InlineData("assign a;\nSelect a pattern a(\"x\",_)", "106, 116")]
    [InlineData("assign a;\nSelect a pattern a(_, _)",
        "2,3,4,7,8,9,10,11,13,17,19,20,21,24,25,27,28,30,31,32,35,36,37,39,40,43,46,48,49,50,52,53,56,57,58,60,61,64,65,67,68,70,71,73,74,75,77,81,82,85,88,90,91,93,94,96,98,99,100,102,104,106,110,111,112,115,116,120,121,122,123,124,125,126,127,128,129,130,131,132,133,134,135,137,138,139,141,142,145,146,147,148,149,150,151,152,153,154,155,156,157,158,161,162,164,165,167,168,169,171,172,174,175,177,178,179,182,183,185,186,188,189,193,194,197,199,200,201,202,203,205,206,207,210,211,212,213,214,215,219,220,223,225,226,228,229,233,236,238,240,242,243,244,245,246,247,248,249,252,253,254,255,257,258,260,261,263,268,269,270,271,272,273,275,276,280,282,283,284,285,288,290,298,299,300,302,304,305,306,308,310,311")]
    [InlineData("while w;Select w pattern w(_, _)",
        "6, 12, 16, 26, 29, 47, 59, 69, 79, 83, 89, 95, 101, 103, 105, 113, 136, 143, 180, 181, 184, 187, 191, 196, 209, 217, 218, 234, 239, 251, 256, 264, 265, 279, 281, 289, 301")]
    [InlineData("assign a; Select a such that Parent(6, a) pattern a(_, \"width + incre + left\")", "7")]
    [InlineData("assign a; Select a such that Parent(6, a) pattern a(_, _)", "7, 8, 9, 10, 11")]
    public void PatternTest(string query, string expected)
    {
        var result = App.Query(query);
        Assert.Equal(expected.Replace(" ", ""), result);
    }
}