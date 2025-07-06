using System.Text.Json.Serialization;
using partycli.Models;

namespace partycli;

[JsonSerializable(typeof(List<Server>))]
public partial class SerializerContext : JsonSerializerContext
{
}