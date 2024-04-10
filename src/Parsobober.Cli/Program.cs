using Microsoft.Extensions.Hosting;
using Parsobober.Pkb.Ast;
using Parsobober.Simple.Lexer;
using Parsobober.Simple.Parser;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddSimpleLexer()
    .AddSimpleParserBuilder()
    .AddAst();

var host = builder.Build();

Console.WriteLine("Parsobober 🦫");

await host.RunAsync();