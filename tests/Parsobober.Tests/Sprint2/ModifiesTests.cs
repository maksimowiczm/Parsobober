namespace Parsobober.Tests.Sprint2;

public class ModifiesTests() : BaseTestClass(Code.ZadanieDomowe1)
{
    [Theory]
    [InlineData("\"Circle\"", "a,b,c,d,k,t")]
    [InlineData("\"Rectangle\"", "a,c,d,t")]
    [InlineData("\"Triangle\"", "a,c,d")]
    [InlineData("\"Hexagon\"", "t")]
    public void Modifies_ProcedureName_Variable(string procedureName, string expected)
    {
        var query = $"variable v;\nSelect v such that Modifies ({procedureName}, v)";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("\"a\"", "Circle,Rectangle,Triangle")]
    [InlineData("\"t\"", "Circle,Hexagon,Rectangle")]
    [InlineData("\"b\"", "Circle")]
    [InlineData("\"c\"", "Circle,Rectangle,Triangle")]
    [InlineData("\"d\"", "Circle,Rectangle,Triangle")]
    public void Modifies_Procedure_VariableName(string variableName, string expected)
    {
        var query = $"procedure p;\nSelect p such that Modifies (p, {variableName})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("\"Circle\"", "\"a\"", "TRUE")]
    [InlineData("\"Circle\"", "\"k\"", "TRUE")]
    [InlineData("\"Rectangle\"", "\"k\"", "FALSE")]
    [InlineData("\"Triangle\"", "\"a\"", "TRUE")]
    [InlineData("\"Circle\"", "\"x\"", "FALSE")]
    public void BooleanModifies_ProcedureName_VariableName(string procedureName, string variableName, string expected)
    {
        var query = $"\nSelect BOOLEAN such that Modifies ({procedureName}, {variableName})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }
}