namespace NHS.MESH.Client.Models;

public class GetChunkedMessageResponse
{
    public List<FileAttachment> FileAttachments { get; set; }
    public MessageMetaData MessageMetaData { get; set; }

}
