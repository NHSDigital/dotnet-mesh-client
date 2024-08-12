namespace NHS.MESH.Client.Models;
using System.ComponentModel.DataAnnotations;

public class MessageMetaData
{
    public string WorkflowID { get; set; }
    public string ToMailbox { get; set; }
    public string FromMailbox { get; set; }
    public string MessageId { get; set; }
    public string? FileName { get; set; }
    public string? MessageType { get; set; }
}
