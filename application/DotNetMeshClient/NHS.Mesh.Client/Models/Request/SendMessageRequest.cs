namespace NHS.MESH.Client.Models;

public class SendMessageRequest
{
    public string WorkflowId {get; set;}
    public string? LocalId {get; set;}
    public char ContentCompressed {get; set;}


}
