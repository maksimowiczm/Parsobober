namespace Parsobober.Tests.Sprint2;

public class WithTests() : BaseTestClass(Code.ZadanieDomowe1_1)
{
    [Theory]
    [InlineData("variable x,y; Select x with x.varName = y.varName",
        "Triangle,a,b,c,d,hexagon,k,t")]
    [InlineData("stmt s; constant c;Select s with s.stmt# = c.value",
        "1,2,3,10,20")]
    [InlineData("variable v; procedure p;Select v with v.varName = p.procName",
        "Triangle")]
    [InlineData("call c; procedure p;Select p with c.procName = p.procName",
        "Hexagon, Rectangle, Triangle")]
    [InlineData("call c; variable v;Select v with c.procName = v.varName",
        "Triangle")]
    [InlineData("constant c; variable v;Select c with c.value = v.varName", "None")]
    // [InlineData("constant c; call c2;Select c with c.value = c2.procName", "None")]
    [InlineData("constant c; procedure p;Select c with c.value = p.procName", "None")]
    [InlineData("variable x,y;Select x such that Uses(\"Hexagon\", x) with x.varName = y.varName",
        "a, hexagon, t")]
    public void WithTest(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }
}