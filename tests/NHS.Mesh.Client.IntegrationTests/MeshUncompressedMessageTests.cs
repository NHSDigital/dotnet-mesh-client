namespace NHS.MESH.Client.IntegrationTests;
using Microsoft.Extensions.DependencyInjection;
using NHS.MESH.Client.Contracts.Services;
using NHS.MESH.Client;
using NHS.MESH.Client.Models;
using NHS.Mesh.Client.TestingCommon;

[TestClass]
[TestCategory("Integration")]
public class MeshUncompressedMessageTests
{

    private readonly IMeshOutboxService _meshOutboxService;
    private readonly IMeshInboxService _meshInboxService;

    private string messageContent;
    private string fileName;
    private string contentType;

    private const string toMailbox = "X26ABC1";
    private const string fromMailbox = "X26ABC2";
    private const string workflowId = "TEST-WORKFLOW";

    public MeshUncompressedMessageTests()
    {
        var services = new ServiceCollection();

        services.AddMeshClient(options =>
        {
            options.MeshApiBaseUrl = "http://localhost:8700/messageexchange";
            options.MeshApiHanshakeUriPath = "";
        });

        var serviceProvider = services.BuildServiceProvider();

        _meshInboxService = serviceProvider.GetService<IMeshInboxService>();
        _meshOutboxService = serviceProvider.GetService<IMeshOutboxService>();
    }
    [TestMethod]
    public async Task Send_Message()
    {

        //arrange
        messageContent = TestingHelpers.LoremIpsum(100, 150, 100, 150, 100);
        fileName = "TestFile.txt";
        contentType = "text/plain";

        FileAttachment fileAttachment = IntegrationTestHelpers.CreateFileAttachment(fileName, messageContent, contentType);

        //Act - Send Uncompressed Message
        var sendMessageResult = await _meshOutboxService.SendUnCompressedMessageAsync(fromMailbox, toMailbox, workflowId, fileAttachment);

        //Assert - UnCompressedMessage Sent Successfully
        Assert.IsTrue(sendMessageResult.IsSuccessful);
        Assert.IsNotNull(sendMessageResult.Response);

        //arrange
        var messageId = sendMessageResult.Response.MessageId;

        //Act - Check Message is in inbox
        var getMessagesResult = await _meshInboxService.GetMessagesAsync(toMailbox);

        //Assert - Check if the message is in the inbox
        Assert.IsTrue(getMessagesResult.IsSuccessful);
        Assert.IsTrue(getMessagesResult.Response.Messages.Count(i => i == messageId) == 1);

        //Act - Check Message Meta Data
        var getMessageHeadResponse = await _meshInboxService.GetHeadMessageByIdAsync(toMailbox, messageId!);

        //Assert - Check Headed Message is valid
        Assert.IsTrue(getMessageHeadResponse.IsSuccessful);
        Assert.AreEqual(workflowId, getMessageHeadResponse.Response.MessageMetaData.WorkflowID);
        Assert.AreEqual(fromMailbox, getMessageHeadResponse.Response.MessageMetaData.FromMailbox);
        Assert.AreEqual(messageId, getMessageHeadResponse.Response.MessageMetaData.MessageId);
        Assert.AreEqual(fileName, getMessageHeadResponse.Response.MessageMetaData.FileName);
        Assert.AreEqual("DATA", getMessageHeadResponse.Response.MessageMetaData.MessageType);

        //Act - Download Message
        var getMessageResponse = await _meshInboxService.GetMessageByIdAsync(toMailbox, messageId!);

        //Assert - check downloded message is correct
        Assert.IsTrue(getMessageResponse.IsSuccessful);
        CollectionAssert.AreEqual(fileAttachment.Content, getMessageResponse.Response.FileAttachment.Content);

        string messageResponseText = System.Text.Encoding.Default.GetString(getMessageResponse.Response.FileAttachment.Content);
        Assert.AreEqual(messageContent, messageResponseText);

        //Act - Acknowledge Message
        var acknowledgeMessageResponse = await _meshInboxService.AcknowledgeMessageByIdAsync(toMailbox, messageId!);

        //Assert
        Assert.IsTrue(acknowledgeMessageResponse.IsSuccessful);
        Assert.AreEqual(messageId, acknowledgeMessageResponse.Response.MessageId);
    }


}
