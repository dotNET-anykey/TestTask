using partycli.Models;

namespace partycli.Clients;

public interface IServerApiClient
{
    Task<Result<List<Server>>> GetServers(ServerSearchQuery searchQuery);
}