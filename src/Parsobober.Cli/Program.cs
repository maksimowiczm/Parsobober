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

Console.WriteLine("Ready");

while (true)
{
    var query = $"{Console.ReadLine()} {Console.ReadLine()}";
    var response = app.Query(query);
    Console.WriteLine(response);
}