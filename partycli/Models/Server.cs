using System.Text.Json.Serialization;

namespace partycli.Models;

public class Server
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "Unknown";

    [JsonPropertyName("load")]
    public int Load { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = "Unknown";

    public void OutputToConsole(int nameColumnWidth, int loadColumnWidth, int statusColumnWidth)
    {
        var originalColor = Console.ForegroundColor;

        if (Name.StartsWith("France", StringComparison.OrdinalIgnoreCase))
        {
            var rest = Name[6..];
            const string flagPart = "France";
            var full = flagPart + rest;
            var padded = full.PadRight(nameColumnWidth);

            var partLength = flagPart.Length / 3;

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(flagPart[..partLength]);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(flagPart.Substring(partLength, partLength));

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(flagPart[(partLength * 2)..]);

            Console.ForegroundColor = originalColor;
            Console.Write(padded[flagPart.Length..]);
        }
        else
        {
            Console.ForegroundColor = originalColor;
            Console.Write(Name.PadRight(nameColumnWidth));
        }

        Console.ForegroundColor = originalColor;
        Console.Write($" {Load.ToString().PadRight(loadColumnWidth)}");

        if (Status.Equals("online", StringComparison.OrdinalIgnoreCase))
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }
        else if (Status.Equals("offline", StringComparison.OrdinalIgnoreCase))
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }

        Console.WriteLine($" {Status.PadRight(statusColumnWidth)}");

        Console.ForegroundColor = originalColor;
    }
}