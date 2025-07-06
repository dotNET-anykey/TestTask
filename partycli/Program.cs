using Microsoft.Extensions.DependencyInjection;
using partycli.Extensions;
using partycli.Services;

namespace partycli;

public class Program
{
    public static async Task Main(string[] args)
    {
        var serviceProvider = BuildServiceProvider();
        var commandLineHandlerService = serviceProvider.GetRequiredService<CommandLineHandlerService>();

        await commandLineHandlerService.HandleCommandLineParameters(args);

        Console.Read();
    }

    private static ServiceProvider BuildServiceProvider()
    {
        return new ServiceCollection()
            .ConfigureServices()
            .ConfigureSerilog()
            .BuildServiceProvider();
    }
}