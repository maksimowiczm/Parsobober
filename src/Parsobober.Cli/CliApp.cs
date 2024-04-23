using Microsoft.Extensions.DependencyInjection;
using Parsobober.Pql.Parser;
using Parsobober.Simple.Parser.Abstractions;

namespace Parsobober.CLI;

internal class CliApp(IServiceProvider serviceProvider)
{
    public async Task RunAsync(string codePath, TextReader reader, TextWriter writer)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        if (!File.Exists(codePath))
        {
            throw new FileNotFoundException("Code file not found", codePath);
        }

        var code = await File.ReadAllTextAsync(codePath);

        scope.ServiceProvider
            .GetRequiredService<IParserBuilder>()
            .BuildParser(code)
            .Parse();

        while (true)
        {
            var pqlParser = scope.ServiceProvider.GetRequiredService<PqlParser>();
            var queryStr = (await reader.ReadLineAsync())!;

            var query = pqlParser.Parse(queryStr);
            var response = query.Execute();

            await writer.WriteLineAsync(response);
        }
    }
}