using NHS.MESH.Client.Models;

namespace NHS.MESH.Client.Models;

public class GetChunkedMessageResponse
{
    public IEnumerable<FileAttachment> fileAttachments { get; set; }
    public MessageMetaData messageMetaData { get; set; }

}
