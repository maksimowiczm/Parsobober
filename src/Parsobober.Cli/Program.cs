using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Parsobober.CLI;
using Parsobober.DesignExtractor;
using Parsobober.Pql.Parser;
using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations;
using Parsobober.Simple.Lexer;
using Parsobober.Simple.Parser;

if (args.Length != 1)
{
    Console.WriteLine("Usage: Parsobober.Cli <path-to-code>");
    return 1;
}

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddSimpleLexer()
    .AddDesignExtractor()
    .AddSimpleParserBuilder()
    .AddAst()
    .AddRelations()
    .AddPqlParser()
    .AddSingleton<CliApp>();

var host = builder.Build();

var app = host.Services.GetRequiredService<CliApp>();
await app.RunAsync(args[0], Console.In, Console.Out);
return 0;