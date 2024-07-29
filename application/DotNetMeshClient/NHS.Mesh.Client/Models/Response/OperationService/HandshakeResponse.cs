using System.Text.Json.Serialization;

namespace NHS.MESH.Client.Models;

public class HandshakeResponse{
    [JsonPropertyName("mailboxId")]
    public string MailboxId {get; set;}
}
