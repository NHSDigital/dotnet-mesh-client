using NHS.MESH.Client.Models;

namespace NHS.MESH.Client.Models;

public class GetMessageResponse{
    public FileAttachment fileAttachment {get; set;}
    public MessageMetaData messageMetaData {get; set;}

}
