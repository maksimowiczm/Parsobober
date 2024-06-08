﻿namespace Parsobober.Tests.IndividualTests;

public class TestsRafal() : BaseTestClass(Code.Dropbox)
{
    [Theory]
    [InlineData("stmt s;Select s such that Parent (s,15)", "14")]
    [InlineData("stmt s;Select s such that Parent(38, s)", "39,40,41,42,43,44")]
    [InlineData("stmt s;Select s such that Parent*(s,121)", "None")]
    [InlineData("stmt s;Select s such that Follows(28, s)", "None")]
    [InlineData("variable v;Select v such that Modifies (\"Move\",v)", "x1,factor,I,x2,tmp")]
    [InlineData("assign a;Select a such that Follows(a, 224)", "223")]
    [InlineData("stmt s;Select s such that Follows*(s, 6)", "1,2,3,4,5")]
    [InlineData("variable v;Select v such that Uses (v, 105)", "x,incre,area,radius,k,j,length,width,incre,c,x,weight,bottm,right,left,height,x2,tmp,y2,y1,decrement,x1,half,base,edge,line,pixel,depth,temporary,triangle,dot,degrees,dx,dy,difference,pink,green")]
    [InlineData("stmt s;Select s such that Uses (s,\"cs5\")", "287,289,291,293,294,296,297,301,302,303,304,305,306,307,309,310,311")]
    [InlineData("variable v;Select v such that Modifies (1,v)", "x1,x2,y1,y2,left,right,top,bottom,incre,decrement")]


    public void TestQuerySingleRafal(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }

    [Theory]
    [InlineData("assign a;Select a such that Modifies(a, \"cs6\") and Uses(a, \"cs6\")", "306")]
    [InlineData("assign a;Select a such that Modifies(a, \"incre\") and Uses(a, \"incre\")", "110,134")]
    [InlineData("assign a;Select a such that Modifies(a, \"difference\") and Uses(a, \"difference\")", "20")]
    [InlineData("while w; assign a;Select a such that Modifies(a, \"circumference\") and Parent(w, a)", "102,104")]
    [InlineData("while w; assign a;Select a such that Modifies(a, \"tmp\") and Parent(w, a)", "17, 48, 60, 90, 96, 137, 149, 188")]
    [InlineData("assign a1,a2;Select s such that Uses(a1, \"height\") and Follows(a1, a2)", "9, 11, 32, 40, 91, 149, 276")]
    [InlineData("procedure p;Select p such that Calls*(p, \"Show\") and Uses(p, \"edge\")", "Rotate,Main,Enlarge,Translate")]
    [InlineData("while w;assign a;Select a such that Modifies(a, \"difference\") and Uses(a, \"difference\")and Parent(w, a)", "20")]
    [InlineData("procedure p;Select p such that Calls*(p, \"Random\") and Uses(p, \"width\") and Modifies(a, \"width\")", "Main")]
    [InlineData("variable v;assign a1,a2;while w;Select a1 such that Uses(a1,v) and Modifies(a1,v) and Parent(w, a1) and Follows(a2, a1)", "8,9,10,20,21,31,61,91,138,183,212,220,229,253,258")]


    public void TestQueryMultipleRafal(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }
}