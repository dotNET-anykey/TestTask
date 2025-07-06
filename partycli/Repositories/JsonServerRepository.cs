using partycli.Models;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace partycli.Repositories;

public class JsonServerRepository : IServerRepository
{
    private static readonly SemaphoreSlim SaveLock = new(1, 1);
    private static readonly string ServersJsonFilePath = Path.Combine(Environment.CurrentDirectory, "servers.json");

    private readonly ILogger<JsonServerRepository> _logger;

    public JsonServerRepository(ILogger<JsonServerRepository> logger)
    {
        _logger = logger;
    }

    public async Task<Result<List<Server>>> GetServers()
    {
        if (File.Exists(ServersJsonFilePath) is false)
        {
            return Result<List<Server>>.Success([]);
        }

        try
        {
            var json = await File.ReadAllTextAsync(ServersJsonFilePath).ConfigureAwait(false);
            var servers = JsonSerializer.Deserialize(json, SerializerContext.Default.ListServer);

            return Result.Success(servers ?? []);
        }
        catch (Exception exception)
        {
            return Result<List<Server>>.Failure(exception.Message);
        }
    }

    public async Task<Result> SaveServers(List<Server> servers)
    {
        await SaveLock.WaitAsync().ConfigureAwait(false);

        try
        {
            var json = JsonSerializer.Serialize(servers, SerializerContext.Default.ListServer);

            await File.WriteAllTextAsync(ServersJsonFilePath, json).ConfigureAwait(false);

            _logger.LogInformation("Saved new server list: {Servers}", json);

            return Result.Success();
        }
        catch (Exception exception)
        {
            return Result.Failure(exception.Message);
        }
        finally
        {
            SaveLock.Release();
        }
    }
}