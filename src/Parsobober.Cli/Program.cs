using Microsoft.Extensions.Hosting;
using Parsobober.Pql.Parser;
using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations;
using Parsobober.Simple.Lexer;
using Parsobober.Simple.Parser;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddSimpleLexer()
    .AddSimpleParserBuilder()
    .AddAst()
    .AddRelations()
    .AddPqlParser();

var host = builder.Build();

Console.WriteLine("Parsobober 🦫");

await host.RunAsync();