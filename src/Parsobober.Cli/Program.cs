using Microsoft.Extensions.Hosting;
using Parsobober.Simple.Lexer;
using Parsobober.Simple.Parser;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddSimpleLexer()
    .AddSimpleParserBuilder();

var host = builder.Build();

Console.WriteLine("Parsobober 🦫");

await host.RunAsync();