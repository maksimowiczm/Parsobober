using Parsobober.CLI;

// Parse command line arguments
if (args.Length < 1)
{
    Console.WriteLine("Usage: Parsobober.Cli <path-to-code> [--verbose|-v]");
    return 1;
}

var flags = args.Skip(1);
var verbose = flags.Contains("--verbose") || args.Contains("-v");
var code = File.ReadAllText(args[0]);

// Create the application
var builder = new AppBuilder();
if (!verbose)
{
    builder.RemoveLogging();
}

// Run the application
using var app = builder.Build(code);

Console.WriteLine("Parsobober 🦫. Może nie jest szybki, ale za to też nie działa 💀🪦.");
Console.WriteLine("Ready");

while (true)
{
    var declaration = Console.ReadLine();
    if (declaration is "quit" or "exit")
    {
        return 0;
    }

    var query = Console.ReadLine();
    if (query is "quit" or "exit")
    {
        return 0;
    }

    try
    {
        var response = app.Query($"{declaration}{query}");
        Console.WriteLine(response);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
}