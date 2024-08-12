using System.Text.Json.Serialization;

namespace NHS.MESH.Client.Models;

public class TrackOutboxResponse
{
    [JsonPropertyName("message_id")]
    public string MessageId { get; set; }
    [JsonPropertyName("local_id")]
    public string? LocalId { get; set; }
    [JsonPropertyName("workflow_id")]
    public string? WorkflowId { get; set; }
    [JsonPropertyName("filename")]
    public string FileName { get; set; }
    [JsonPropertyName("expiry_time")]
    public string ExpiryTime { get; set; }
    [JsonPropertyName("upload_timestamp")]
    public string UploadTimestamp { get; set; }
    [JsonPropertyName("recipient")]
    public string Recipient { get; set; }
    [JsonPropertyName("recipient_name")]
    public string RecipientName { get; set; }
    [JsonPropertyName("recipient_ods_code")]
    public string RecipientOdsCode { get; set; }
    [JsonPropertyName("recipient_org_code")]
    public string RecipientOrgCode { get; set; }
    [JsonPropertyName("recipient_org_name")]
    public string RecipientOrgName { get; set; }
    [JsonPropertyName("status_success")]
    public bool StatusSuccess { get; set; }
    [JsonPropertyName("status")]
    public string Status { get; set; }
    [JsonPropertyName("status_event")]
    public string StatusEvent { get; set; }
    [JsonPropertyName("status_timestamp")]
    public DateTime StatusTimestamp { get; set; }
    [JsonPropertyName("status_description")]
    public string StatusDescription { get; set; }
    [JsonPropertyName("status_code")]
    public string StatusCode { get; set; }

}
