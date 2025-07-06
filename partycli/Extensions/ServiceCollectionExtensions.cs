using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using partycli.Clients;
using partycli.Repositories;
using partycli.Services;
using Serilog;
using Serilog.Events;

namespace partycli.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddHttpClient<IServerApiClient, NordVpnClient>(static httpClient =>
        {
            httpClient.BaseAddress = new Uri("https://api.nordvpn.com");
        });
        services.AddTransient<IServerRepository, JsonServerRepository>();
        services.AddTransient<CommandLineHandlerService>();

        return services;
    }

    public static IServiceCollection ConfigureSerilog(this IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
            .WriteTo.File(Path.Combine(Environment.CurrentDirectory, "log"), LogEventLevel.Information)
            .CreateLogger();

        services.AddLogging(static loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog();
        });

        return services;
    }
}