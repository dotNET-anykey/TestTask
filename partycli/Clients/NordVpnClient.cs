using System.Text.Json;
using partycli.Enums;
using partycli.Models;

namespace partycli.Clients;

public class NordVpnClient : IServerApiClient
{
    private static readonly Dictionary<Protocol, int> Protocols = new()
    {
        { Protocol.Tcp, 5 },
        { Protocol.Udp, 3 },
        { Protocol.NordLynx, 35 }
    };

    private static readonly Dictionary<Country, int> Countries = new()
    {
        { Country.Albania, 2 },
        { Country.Argentina, 10 },
        { Country.France, 74 }
    };

    private readonly HttpClient _httpClient;

    public NordVpnClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Result<List<Server>>> GetServers(ServerSearchQuery searchQuery)
    {
        try
        {
            var query = BuildQueryUri(searchQuery);
            var response = await _httpClient.GetAsync($"v1/servers{query}").ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var servers = JsonSerializer.Deserialize(responseContent, SerializerContext.Default.ListServer);

            return Result<List<Server>>.Success(servers ?? []);
        }
        catch (Exception e)
        {
            return Result<List<Server>>.Failure(e.Message);
        }
    }

    private static string BuildQueryUri(ServerSearchQuery query)
    {
        List<string> parameters = [];

        if (query.Country.HasValue && Countries.TryGetValue(query.Country.Value, out var countryId))
        {
            parameters.Add($"filters[country_id]={countryId}");
        }

        if (query.Protocol.HasValue && Protocols.TryGetValue(query.Protocol.Value, out var protocolId))
        {
            parameters.Add($"filters[servers_technologies][id]={protocolId}");
        }

        return parameters.Count > 0
            ? $"?{string.Join("&", parameters)}"
            : string.Empty;
    }
}