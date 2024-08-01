using NHS.MESH.Client.Models;

namespace NHS.MESH.Client.Helpers;

public static class FileHelpers
{
    public static async Task<FileAttachment> CreateFileAttachment(HttpResponseMessage httpResponseMessage)
    {

        int? chunkNumber = null;
        var chunkRange = httpResponseMessage.Headers.GetHeaderItemValue("mex-chunk-range");
        if(chunkRange != null)
        {
            chunkNumber = ParseChunkRange(chunkRange).currentChunk;
        }

        var fileAttachment = new FileAttachment
        {
            FileName =  httpResponseMessage.Headers.GetHeaderItemValue("mex-filename"),
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

    public static (int currentChunk,int chunkLength) ParseChunkRange(string chunkRange)
    {
        var range =  chunkRange.Split(":");
        int currentChunk = int.Parse(range[0]);
        int chunkLength = int.Parse(range[1]);

        return (currentChunk, chunkLength);
    }


}
