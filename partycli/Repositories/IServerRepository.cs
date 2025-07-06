using partycli.Models;

namespace partycli.Repositories;

public interface IServerRepository
{
    Task<Result<List<Server>>> GetServers();
    Task<Result> SaveServers(List<Server> servers);
}