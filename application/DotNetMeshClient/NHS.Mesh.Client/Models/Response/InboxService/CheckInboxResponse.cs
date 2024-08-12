namespace NHS.MESH.Client.Models;
using System.Text.Json.Serialization;

public class CheckInboxResponse
{
    [JsonPropertyName("messages")]
    public IEnumerable<string> Messages { get; set; }
}
