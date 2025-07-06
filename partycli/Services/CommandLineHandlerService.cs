using partycli.Clients;
using partycli.Extensions;
using partycli.Models;
using partycli.Repositories;
using System.Diagnostics;
using static partycli.Constants.CommandLineParameters;

namespace partycli.Services;

public class CommandLineHandlerService
{
    private static readonly string ProcessName = GetFileName();

    private readonly IServerApiClient _apiClient;
    private readonly IServerRepository _repository;

    public CommandLineHandlerService(IServerApiClient apiClient, IServerRepository repository)
    {
        _apiClient = apiClient;
        _repository = repository;
    }

    public async Task HandleCommandLineParameters(string[] args)
    {
        if (args.Length is 0)
        {
            OutputInstructions();

            return;
        }

        var firstParameter = args[0];

        if (firstParameter.EqualsToParameter(ServerList))
        {
            await HandleServerListParameter([.. args.Skip(1)]);
        }
        // Left empty due to unclear purpose of Config parameter, as logs and servers are no longer stored in .settings.
        // Might be used to explicitly populate the local server storage with data, but no validation of the provided data can be done.
        // Setting the value of log seems weird.
        else if (firstParameter.EqualsToParameter(Config))
        {
        }
        else
        {
            OutputInstructions();
        }
    }

    private async Task HandleServerListParameter(IList<string> args)
    {
        if (args.Any(static x => x.EqualsToParameter(Local)))
        {
            await OutputLocalServers().ConfigureAwait(false);
        }
        else
        {
            var query = new ServerSearchQuery();

            foreach (var parameter in args)
            {
                var normalized = parameter.ToLowerInvariant();

                if (query.Protocol.HasValue is false && Protocols.TryGetValue(normalized, out var protocol))
                {
                    query.Protocol = protocol;
                }

                if (query.Country.HasValue is false && Countries.TryGetValue(normalized, out var country))
                {
                    query.Country = country;
                }
            }

            await GetAndOutputServers(query).ConfigureAwait(false);
        }
    }

    private async Task OutputLocalServers()
    {
        var getLocalServersResult = await _repository.GetServers().ConfigureAwait(false);

        if (getLocalServersResult.IsSuccess is false)
        {
            Console.WriteLine($"Failed to retrieve servers from a json file: {getLocalServersResult.ErrorMessage}");

            return;
        }

        var servers = getLocalServersResult.Value;

        OutputServersToConsole(servers, isLocal: true);
    }

    private async Task GetAndOutputServers(ServerSearchQuery query)
    {
        var getServersResult = await _apiClient.GetServers(query).ConfigureAwait(false);

        if (getServersResult.IsSuccess is false)
        {
            Console.WriteLine($"Could not fetch the servers due to: {getServersResult.ErrorMessage}");

            return;
        }

        var servers = getServersResult.Value;

        OutputServersToConsole(servers, isLocal: false);

        var saveServersResult = await _repository.SaveServers(servers).ConfigureAwait(false);

        if (saveServersResult.IsSuccess is false)
        {
            Console.WriteLine($"Failed save servers to a json file: {saveServersResult.ErrorMessage}");
        }
    }

    private static void OutputServersToConsole(List<Server> servers, bool isLocal)
    {
        if (servers.Count is 0)
        {
            var output = isLocal
                ? "Error: There is no server data in local storage."
                : "Warning: Retrieved empty server list from API.";

            Console.WriteLine(output);

            return;
        }

        const int nameColumnWidth = 25;
        const int loadColumnWidth = 10;
        const int statusColumnWidth = 10;
        const int totalWidth = nameColumnWidth + loadColumnWidth + statusColumnWidth + 2;

        var separator = new string('-', totalWidth);

        Console.WriteLine($"{"Server Name", -nameColumnWidth} {"Load (%)", -loadColumnWidth} {"Status", -statusColumnWidth}");
        Console.WriteLine(separator);

        foreach (var server in servers)
        {
            server.OutputToConsole(nameColumnWidth, loadColumnWidth, statusColumnWidth);
        }

        Console.WriteLine(separator);
        Console.WriteLine($"Total servers: {servers.Count}");
    }

    private static void OutputInstructions()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine($"    {ProcessName} {ServerList}                      Get and save all servers");
        Console.WriteLine($"    {ProcessName} {ServerList} --france             Get and save France servers");
        Console.WriteLine($"    {ProcessName} {ServerList} --TCP                Get and save TCP-supporting servers");
        Console.WriteLine($"    {ProcessName} {ServerList} {Local}              View saved local server list");
    }

    private static string GetFileName()
    {
        var pathToExecutable = Process.GetCurrentProcess().MainModule?.FileName;

        return string.IsNullOrEmpty(pathToExecutable)
            ? "partycli.exe"
            : Path.GetFileName(pathToExecutable);
    }
}