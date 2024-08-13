
using System.Text;
using NHS.MESH.Client.Models;

public static class IntegrationTestHelpers
{
    public static FileAttachment CreateFileAttachment(string fileName, string content, string contentType)
    {
        byte[] data = Encoding.ASCII.GetBytes(content);
        return new FileAttachment
        {
            FileName = fileName,
            Content = data,
            ContentType = contentType
        };
    }
}
