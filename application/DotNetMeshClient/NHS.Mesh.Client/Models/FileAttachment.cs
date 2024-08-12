namespace NHS.MESH.Client.Models;
public class FileAttachment
{

    public string FileName { get; set; }
    public byte[] Content { get; set; }
    public string ContentType { get; set; }
    public int? ChunkNumber { get; set; }
    public bool IsChunked { get { return ChunkNumber.HasValue; } }
}
