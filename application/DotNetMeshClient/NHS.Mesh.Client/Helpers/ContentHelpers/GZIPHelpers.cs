namespace NHS.MESH.Client.Helpers.ContentHelpers;

using System.IO.Compression;

public class GZIPHelpers
{
    public static byte[] CompressBuffer(byte[]  byteArray)
    {
        MemoryStream strm = new MemoryStream();
        GZipStream GZipStrem = new GZipStream(strm, CompressionMode.Compress, true);
        GZipStrem.Write(byteArray, 0, byteArray.Length);
        GZipStrem.Flush();
        strm.Flush();
        byte[]  ByteArrayToreturn= strm.GetBuffer();
        GZipStrem.Close();
        strm.Close();
        return ByteArrayToreturn;
    }
    public static byte[] DeCompressBuffer(byte[] byteArray)
    {
        MemoryStream strm = new MemoryStream(byteArray);
        GZipStream GZipStrem = new GZipStream(strm, CompressionMode.Decompress,true);
        List<byte> ByteListUncompressedData = new List<byte>();

        int bytesRead = GZipStrem.ReadByte();
        while (bytesRead != -1)
        {
            ByteListUncompressedData.Add((byte)bytesRead);
            bytesRead = GZipStrem.ReadByte();
        }
        GZipStrem.Flush();
        strm.Flush();
        GZipStrem.Close();
        strm.Close();
        return ByteListUncompressedData.ToArray();
    }

}
