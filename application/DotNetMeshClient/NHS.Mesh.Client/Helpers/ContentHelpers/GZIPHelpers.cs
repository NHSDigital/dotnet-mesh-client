namespace NHS.MESH.Client.Helpers.ContentHelpers;

using System.IO.Compression;

public class GZIPHelpers
{
    public static byte[] CompressBuffer(byte[]  data)
    {
        using (var compressedStream = new MemoryStream())
        using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
        {
            zipStream.Write(data, 0, data.Length);
            zipStream.Close();
            return compressedStream.ToArray();
        }
    }
    public static byte[] DeCompressBuffer(byte[] byteArray)
    {
        MemoryStream stream = new MemoryStream(byteArray);
        GZipStream gZipStream = new GZipStream(stream, CompressionMode.Decompress);
        using (var resultStream = new MemoryStream())
        {
            gZipStream.CopyTo(resultStream);
            return resultStream.ToArray();
        }
    }

}
