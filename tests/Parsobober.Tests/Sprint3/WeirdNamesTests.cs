namespace Parsobober.Tests.Sprint3;

public class WeirdNamesTests() : BaseTestClass(Code.Dropbox)
{
    [Theory]
    [Trait("SkipInActions", "true")]
    [InlineData("""
                while whilew;
                Select whilew such that Parent(whilew, 1)
                """, "none")]
    [InlineData("""
                if ifs1, ifs2;
                Select ifs2 such that Follows(ifs1, ifs2)
                """, "166, 173")]
    [InlineData("""
                if ifs1, ifs2; while whilew; constant constantc; variable variablev, variablev1;
                Select ifs2 such that Follows(ifs1, ifs2) such that Parent(whilew, ifs2)
                """, "none")]
    [InlineData("""
                if ifs1; while whilew; procedure procedurep;
                Select ifs1 such that Follows(ifs1,whilew) with procedurep.procName="whilew"
                """, "none")]
    [InlineData("""
                if i; while w; procedure p;
                Select i such that Follows(i,w) with p.procName="whilew"
                """, "none")]
    public void NameTests(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }

    [Theory]
    [Trait("SkipInActions", "true")]
    [InlineData("""
                if ifs1; while whilew;variable variablev; procedure procedurep;
                Select ifs1 such that Follows(ifs1, whilew) and Parent(whilew, ifs1) with ifs1.stmt# = whilew.stmt# and procedurep.procName = variablev.varName
                """, "none")]
    public void NameTestsManyAlliases(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }

    [Theory(Skip = "Skipping name testing")]
    [Trait("SkipInActions", "true")]
    [InlineData("""
                assign and; stmt such, Select; while that;
                Select such such that Follows(and, such) and Parent(and, that)
                """, "none")]
    [InlineData("""
                assign and; stmt such, Select; while that;
                Select such such that Follows(and, such) and Parent(and, that) with such.stmt# = Select.stmt#
                """, "none")]
    public void NameTestsCursed(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }
}