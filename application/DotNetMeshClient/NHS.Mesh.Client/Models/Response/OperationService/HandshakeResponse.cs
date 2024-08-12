namespace NHS.MESH.Client.Models;
using System.Text.Json.Serialization;

public class HandshakeResponse
{
    [JsonPropertyName("mailboxId")]
    public string MailboxId { get; set; }
}
