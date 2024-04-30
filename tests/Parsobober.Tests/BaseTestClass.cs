namespace Parsobober.Tests;

public abstract class BaseTestClass
{
    protected readonly App App;

    protected BaseTestClass(string code)
    {
        var builder = new AppBuilder().RemoveLogging();
        App = builder.Build(code);
    }
}