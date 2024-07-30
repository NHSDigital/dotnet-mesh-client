using System.Text.Json.Serialization;

namespace NHS.MESH.Client.Models;

public class CheckInboxResponse{
    [JsonPropertyName("messages")]
    public IEnumerable<string> Messages {get; set;}
}
