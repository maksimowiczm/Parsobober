using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Parsobober.CLI;
using Parsobober.DesignExtractor;
using Parsobober.Pql.Parser;
using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations;
using Parsobober.Simple.Lexer;
using Parsobober.Simple.Parser;

if (args.Length < 1)
{
    Console.WriteLine("Usage: Parsobober.Cli <path-to-code> [--verbose|-v]");
    return 1;
}

var flags = args.Skip(1);
var verbose = flags.Contains("--verbose") || args.Contains("-v");

var builder = Host.CreateApplicationBuilder(args);

if (!verbose)
{
    builder.Logging.ClearProviders();
}

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