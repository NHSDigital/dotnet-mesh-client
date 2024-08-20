namespace NHS.MESH.Client.IntegrationTests;
using Microsoft.Extensions.DependencyInjection;
using NHS.MESH.Client.Contracts.Services;
using NHS.MESH.Client;
using NHS.MESH.Client.Models;
using NHS.MESH.Client.Contracts.Configurations;
using NHS.MESH.Client.Helpers;
using NHS.Mesh.Client.TestingCommon;
using System.Security.Cryptography.X509Certificates;

[TestClass]
[TestCategory("Integration")]
public class MeshChunkedMessageTests
{

    private readonly IMeshOutboxService _meshOutboxService;
    private readonly IMeshInboxService _meshInboxService;

    private readonly IMeshConnectConfiguration _config;

    private string messageContent;
    private string fileName;
    private string contentType;

    private const string toMailbox = "X26ABC2";
    private const string fromMailbox = "X26ABC1";
    private const string workflowId = "TEST-WORKFLOW";

    public MeshChunkedMessageTests()
    {
        var services = new ServiceCollection();

        services.AddMeshClient(options =>
        {
            options.MeshApiBaseUrl = "http://localhost:8700/messageexchange";
        })
        .AddMailbox(fromMailbox,
        new Configuration.MailboxConfiguration
        {
            Password = "password",
            SharedKey = "TestKey"
        })
        .AddMailbox(toMailbox,
        new Configuration.MailboxConfiguration
        {
            Password = "password",
            SharedKey = "TestKey"
        })
        .Build();

        var serviceProvider = services.BuildServiceProvider();
        _meshInboxService = serviceProvider.GetService<IMeshInboxService>()!;
        _meshOutboxService = serviceProvider.GetService<IMeshOutboxService>()!;
        _config =  serviceProvider.GetService<IMeshConnectConfiguration>()!;


    }
    [TestMethod]
    public async Task Send_Message()
    {

        //arrange
        messageContent = TestingHelpers.LoremIpsum(200, 250, 100, 150, 200);
        fileName = "TestFile.txt";
        contentType = "text/plain";

        FileAttachment fileAttachment = IntegrationTestHelpers.CreateFileAttachment(fileName, messageContent, contentType);

        //Assert that the test data length is long enough
        Assert.IsTrue(fileAttachment.Content.Length > _config.ChunkSize);
        var numberOfChunks = fileAttachment.Content.Length / _config.ChunkSize +1;

        //Act - Send Uncompressed Message
        var sendMessageResult = await _meshOutboxService.SendChunkedMessageAsync(fromMailbox, toMailbox, workflowId, fileAttachment);

        //Assert - UnCompressedMessage Sent Successfully
        Assert.IsTrue(sendMessageResult.IsSuccessful);
        //Assert.IsNotNull(sendMessageResult.Response);

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
        Assert.AreEqual(numberOfChunks,getMessageHeadResponse.Response.MessageMetaData.TotalChunks);

        //Act - Download Message
        var getMessageResponse = await _meshInboxService.GetChunkedMessageByIdAsync(toMailbox, messageId!);

        //Assert - check downloded message is correct
        Assert.IsTrue(getMessageResponse.IsSuccessful);
        Assert.AreEqual(numberOfChunks,getMessageResponse.Response.FileAttachments.Count,$"Number of chunks did't match, expected count: { numberOfChunks } Actual count: { getMessageResponse.Response.FileAttachments.Count }");

        var assembledFile = await FileHelpers.ReassembleChunkedFile(getMessageResponse.Response.FileAttachments);

        CollectionAssert.AreEqual(fileAttachment.Content, assembledFile.Content);

        string messageResponseText = System.Text.Encoding.Default.GetString(assembledFile.Content);
        Assert.AreEqual(messageContent, messageResponseText);

        //Act - Acknowledge Message
        var acknowledgeMessageResponse = await _meshInboxService.AcknowledgeMessageByIdAsync(toMailbox, messageId!);

        //Assert
        Assert.IsTrue(acknowledgeMessageResponse.IsSuccessful);
        Assert.AreEqual(messageId, acknowledgeMessageResponse.Response.MessageId);

    }
}
