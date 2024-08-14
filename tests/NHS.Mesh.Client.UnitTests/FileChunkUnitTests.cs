namespace NHS.MESH.Client.UnitTests;

using System.Text;
using NHS.MESH.Client.Helpers.ContentHelpers;

[TestClass]
public class FileChunkUnitTests
{

    [TestMethod]
    public async Task Split_File_to_Byte_Arrays_ReturnsExpectedChunkLengths()
    {
        //arrange
        var messageContent = "Michael Is testing Stuff";

        int chunkSize = 19;
        byte[] data = Encoding.ASCII.GetBytes(messageContent);

        var numberOfChunks = messageContent.Length / chunkSize +1;
        var lengthOfEncodedData = data.Length;

        var lastChunkSize = lengthOfEncodedData % chunkSize;

        //act
        var chunks = await ContentSplitHelper.SplitFileToByteArrays(data,chunkSize);

        //assert
        for(int i = 0; i<numberOfChunks; i++)
        {
            if(i < numberOfChunks-1)
            {
                Assert.AreEqual(chunkSize,chunks[i].Length);
            }
            else
            {
                Assert.AreEqual(lastChunkSize,chunks[i].Length);
            }
        }
    }




}
