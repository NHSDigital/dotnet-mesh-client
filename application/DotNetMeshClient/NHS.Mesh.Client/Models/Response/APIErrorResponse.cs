using System.Text.Json.Serialization;

namespace NHS.MESH.Client.Models;

public class APIErrorResponse{
    [JsonPropertyName("errorEvent")]
    public string? ErrorEvent {get; set;}
    [JsonPropertyName("errorCode")]
    public string? ErrorCode {get; set;}
    [JsonPropertyName("errorDescription")]
    public string ErrorDescription {get; set;}
}
