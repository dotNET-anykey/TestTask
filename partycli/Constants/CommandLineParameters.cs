using partycli.Enums;

namespace partycli.Constants;

public static class CommandLineParameters
{
    public const string ServerList = "server_list";
    public const string Config = "config";
    public const string Local = "--local";

    public static IReadOnlyDictionary<string, Country> Countries { get; } = new Dictionary<string, Country>
    {
        { "--albania", Country.Albania },
        { "--argentina", Country.Argentina },
        { "--france", Country.France }
    };

    public static IReadOnlyDictionary<string, Protocol> Protocols { get; } = new Dictionary<string, Protocol>
    {
        { "--TCP", Protocol.Tcp },
        { "--UDP", Protocol.Udp }
    };
}