using Microsoft.Extensions.DependencyInjection;
using Parsobober.Pql.Parser;
using Parsobober.Simple.Parser.Abstractions;

namespace Parsobober.CLI;

public class App : IDisposable
{
    private readonly IServiceScope _scope;

    public App(IServiceScope scope, string code)
    {
        _scope = scope;
        var simpleParserBuilder = scope.ServiceProvider.GetRequiredService<IParserBuilder>();
        simpleParserBuilder.BuildParser(code).Parse();
    }

    public string Query(string queryStr)
    {
        var pqlParser = _scope.ServiceProvider.GetRequiredService<PqlParser>();
        var query = pqlParser.Parse(queryStr);
        var response = query.Execute();
        return response;
    }

    public void Dispose() => _scope.Dispose();
}