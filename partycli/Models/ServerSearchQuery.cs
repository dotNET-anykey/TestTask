using partycli.Enums;

namespace partycli.Models;

public class ServerSearchQuery
{
    public Protocol? Protocol { get; set; }

    public Country? Country { get; set; }
}