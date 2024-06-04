namespace Parsobober.Tests.Sprint2;

public class UsesTests() : BaseTestClass(Code.ZadanieDomowe1)
{
    [Theory]
    [InlineData("\"Circle\"", "a,b,c,d,k,t")]
    [InlineData("\"Rectangle\"", "a,b,c,d,k,t")]
    [InlineData("\"Triangle\"", "a,b,d,k,t")]
    [InlineData("\"Hexagon\"", "a,t")]
    public void Uses_ProcedureName_Variable(string procedureName, string expected)
    {
        var query = $"variable v;\nSelect v such that Uses ({procedureName}, v)";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("\"a\"", "Circle,Hexagon,Rectangle,Triangle")]
    [InlineData("\"t\"", "Circle,Hexagon,Rectangle,Triangle")]
    [InlineData("\"b\"", "Circle,Rectangle,Triangle")]
    [InlineData("\"c\"", "Circle,Rectangle")]
    [InlineData("\"d\"", "Circle,Rectangle,Triangle")]
    [InlineData("\"k\"", "Circle,Rectangle,Triangle")]
    public void Uses_Procedure_VariableName(string variableName, string expected)
    {
        var query = $"procedure p;\nSelect p such that Uses (p, {variableName})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("\"Circle\"", "\"a\"", "TRUE")]
    [InlineData("\"Circle\"", "\"k\"", "TRUE")]
    [InlineData("\"Rectangle\"", "\"k\"", "TRUE")]
    [InlineData("\"Hexagon\"", "\"k\"", "FALSE")]
    [InlineData("\"Triangle\"", "\"a\"", "TRUE")]
    [InlineData("\"Circle\"", "\"x\"", "FALSE")]
    public void BooleanUses_ProcedureName_VariableName(string procedureName, string variableName, string expected)
    {
        var query = $"\nSelect BOOLEAN such that Uses ({procedureName}, {variableName})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }
}