namespace NHS.MESH.Client.Models;
using System.Text.Json.Serialization;
using NHS.MESH.Client.Models;

public class AcknowledgeMessageResponse
{
    [JsonPropertyName("messageId")]
    public string MessageId { get; set; }

}
