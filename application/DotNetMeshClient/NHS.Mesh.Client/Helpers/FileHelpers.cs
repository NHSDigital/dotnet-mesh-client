namespace NHS.MESH.Client.Helpers;
using System.Security.Cryptography;
using NHS.MESH.Client.Helpers.ContentHelpers;
using NHS.MESH.Client.Models;

public static class FileHelpers
{
    public static async Task<FileAttachment> CreateFileAttachment(HttpResponseMessage httpResponseMessage)
    {

        int? chunkNumber = null;
        var chunkRange = httpResponseMessage.Headers.GetHeaderItemValue("mex-chunk-range");
        if (chunkRange != null)
        {
            chunkNumber = ParseChunkRange(chunkRange).currentChunk;
        }

        var fileAttachment = new FileAttachment
        {
            FileName = httpResponseMessage.Headers.GetHeaderItemValue("mex-filename"),
            Content = await httpResponseMessage.Content.ReadAsByteArrayAsync(),
            ContentType = httpResponseMessage.Headers.GetHeaderItemValue("content-type"),
            ChunkNumber = chunkNumber


        };
        return fileAttachment;
    }
    public static MessageMetaData CreateMessageMetaData(HttpResponseMessage httpResponseMessage)
    {
        return new MessageMetaData
        {
            WorkflowID = httpResponseMessage.Headers.GetHeaderItemValue("mex-workflowid"),
            ToMailbox = httpResponseMessage.Headers.GetHeaderItemValue("mex-to"),
            FromMailbox = httpResponseMessage.Headers.GetHeaderItemValue("mex-from"),
            MessageId = httpResponseMessage.Headers.GetHeaderItemValue("mex-messageid")
        };
    }

    public static (int currentChunk, int chunkLength) ParseChunkRange(string chunkRange)
    {
        var range = chunkRange.Split(":");
        int currentChunk = int.Parse(range[0]);
        int chunkLength = int.Parse(range[1]);

        return (currentChunk, chunkLength);
    }
    public static string CreateChunkRange(int currentChunk, int chunkLength)
    {
        if (currentChunk > chunkLength)
        {
            throw new ArgumentException("Current chunk cannot be longer than the the chunk length");
        }
        return $"{currentChunk}:{chunkLength}";
    }

    public static string GenerateChecksum(byte[] data)
    {
        using (var md5instance = MD5.Create())
        {
            var hash = md5instance.ComputeHash(data);
            return Convert.ToBase64String(hash);
        }
    }

    public static bool IsFileTooLarge(byte[] data, long MaxLength)
    {
        return data.Length >= MaxLength;
    }

    public static async Task<HttpContent> CompressFileAsync(byte[] data)
    {
        var compressedData = GZIPHelpers.CompressBuffer(data);

        var content = new ByteArrayContent(compressedData);

        return content;
    }

    public static async Task<FileAttachment> ReassembleChunkedFile(List<FileAttachment> fileChunks)
    {

        var orderedChunks = fileChunks.OrderBy(i => i.ChunkNumber);
        var firstChunk = orderedChunks.First();

        using(var memoryStream = new MemoryStream()){
            int expectedChunkNumber = 1;
            foreach(var file in orderedChunks)
            {
                if(file.ChunkNumber != expectedChunkNumber)
                {
                    throw new ArgumentException("List was missing Chunks Cannot reassemble");
                }
                var decompressedData = GZIPHelpers.DeCompressBuffer(file.Content);
                await memoryStream.WriteAsync(decompressedData);
                expectedChunkNumber++;
            }
            return new FileAttachment
            {
                FileName = firstChunk.FileName,
                ContentType = firstChunk.ContentType,
                Content = memoryStream.ToArray()
            };
        }


    }
}
