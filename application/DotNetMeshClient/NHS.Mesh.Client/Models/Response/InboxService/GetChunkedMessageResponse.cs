using NHS.MESH.Client.Models;

namespace NHS.MESH.Client.Models;

public class GetChunkedMessageResponse
{
    public IEnumerable<FileAttachment> FileAttachments { get; set; }
    public MessageMetaData MessageMetaData { get; set; }

}
