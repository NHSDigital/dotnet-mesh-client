namespace NHS.MESH.Client.UnitTests;

using System.Net;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using Moq;
using NHS.MESH.Client.Contracts.Clients;
using NHS.MESH.Client.Contracts.Configurations;
using NHS.MESH.Client.Contracts.Services;
using NHS.MESH.Client.Helpers.AuthHelpers;
using NHS.MESH.Client.Models;
using NHS.MESH.Client.Services;
using NuGet.Frameworks;

[TestClass]
public class MeshInboxServiceTests
{
        /// <summary>The MESH Connect Configuration.</summary>
        private readonly Mock<IMeshConnectConfiguration> _meshConnectConfiguration;

        /// <summary>The MESH Connect Client.</summary>
        private readonly Mock<IMeshConnectClient> _meshConnectClient;

        /// <summary>The MESH Inbox Service.</summary>
        private readonly IMeshInboxService _meshInboxService;

    public MeshInboxServiceTests()
    {
            _meshConnectConfiguration = new Mock<IMeshConnectConfiguration>(MockBehavior.Strict);
            _meshConnectClient = new Mock<IMeshConnectClient>(MockBehavior.Strict);

            _meshInboxService = new MeshInboxService(
                _meshConnectConfiguration.Object,
                _meshConnectClient.Object
            );

            // Setup default values for configuration mock
            _meshConnectConfiguration.SetupGet(c => c.MeshApiBaseUrl).Returns("https://api.mesh.com");
            _meshConnectConfiguration.SetupGet(c => c.MeshApiInboxUriPath).Returns("inbox");
            _meshConnectConfiguration.SetupGet(c => c.MeshApiAcknowledgeUriPath).Returns("acknowledge");

    }
    // [TestMethod]
    // public void getAuth()//REMOVE Only to get auth token for local testing
    // {
    //     var authtoken  = MeshAuthorizationHelper.GenerateAuthHeaderValue("X26ABC1",null,"password","TestKey",null,0);
    //     Assert.AreEqual("aKey",authtoken);
    // }

    [TestMethod]
    public async Task GetMessagesAsync_MailboxIdNull_ThrowsArgumentNullException()
    {
        //arrange

        Exception testException = null;
        //act
        try
        {
          await _meshInboxService.GetMessagesAsync(null);
        }
        catch(ArgumentNullException ex)
        {
            //assert
             Assert.AreEqual("mailboxId",ex!.ParamName);
             testException = ex;

        }
        catch(Exception ex)
        {
            Assert.Fail($"Incorrect Exception was thrown, Exception Thrown: {ex.Message}");
        }
        Assert.IsNotNull(testException);
    }
    [TestMethod]
    public async Task GetMessagesAsync_MeshApiBaseUrlNull_ThrowsArgumentNullException()
    {
        //arrange
        Exception testException = null;
        _meshConnectConfiguration.SetupGet(c => c.MeshApiBaseUrl).Returns((string)null);

        //act
        try
        {
            await _meshInboxService.GetMessagesAsync("valid-mailbox-id");
        }
        //assert
        catch(ArgumentNullException ex)
        {
            testException = ex;
            Assert.AreEqual("MeshApiBaseUrl",ex!.ParamName);
        }
        catch(Exception ex)
        {
            Assert.Fail($"Incorrect Exception was thrown, Exception Thrown: {ex.Message}");
        }
        Assert.IsNotNull(testException);
    }

    [TestMethod]
    public async Task GetMessagesAsync_MeshApiInboxUriPathNull_ThrowsArgumentNullException()
    {
        //arrange
        Exception testException = null;
        _meshConnectConfiguration.SetupGet(c => c.MeshApiInboxUriPath).Returns((string)null);
        //act
        try
        {
            await _meshInboxService.GetMessagesAsync("valid-mailbox-id");
        }
        //assert
        catch(ArgumentNullException ex)
        {
            testException = ex;
            Assert.AreEqual("MeshApiInboxUriPath",ex!.ParamName);
        }
        catch(Exception ex)
        {
            Assert.Fail($"Incorrect Exception was thrown, Exception Thrown: {ex.Message}");
        }
        Assert.IsNotNull(testException);
    }

    [TestMethod]
    public async Task GetMessagesAsync_ValidInputs_ReturnsSuccessfulResponse()
    {
        //arrange
        var mailboxId = "valid-mailbox-id";
        var handshakeResult = new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.OK, "OK");

        var checkInboxResponse = new CheckInboxResponse
        {
            Messages = new List<string>{"MessageId"}
        };

        var response = UnitTestHelpers.CreateMockHttpResponseMessage<CheckInboxResponse>(checkInboxResponse,HttpStatusCode.OK);

        _meshConnectClient.Setup(c => c.SendRequestAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(response);

        //act
        var result = await _meshInboxService.GetMessagesAsync(mailboxId);

        //assert
        Assert.IsTrue(result.IsSuccessful);
        Assert.IsTrue(result.Response.Messages.Count(i => i == "MessageId") == 1);
    }

    [TestMethod]
    public async Task GetMessageByIdAsync_MailboxIdNull_ThrowsArgumentNullException()
    {
        //arrange
        Exception testException = null;

        //act
        try
        {
            await _meshInboxService.GetMessageByIdAsync(null, "valid-message-id");
        }
        //assert
        catch(ArgumentNullException exception)
        {
            testException = exception;
            Assert.AreEqual("mailboxId",exception.ParamName);
        }
        catch(Exception ex)
        {
            Assert.Fail($"Incorrect Exception was thrown, Exception Thrown: {ex.Message}");
        }

        Assert.IsNotNull(testException);
    }

    [TestMethod]
    public async Task GetMessageByIdAsync_MessageIdNull_ThrowsArgumentNullException()
    {
        // Arrange
        Exception testException = null;
        var mailboxId = "valid-mailbox-id";

        // Act
        try
        {
            await _meshInboxService.GetMessageByIdAsync(mailboxId, null);
        }
        catch(ArgumentNullException exception)
        {
            testException = exception;
            Assert.AreEqual("messageId",exception.ParamName);
        }
        catch(Exception ex)
        {
            Assert.Fail($"Incorrect Exception was thrown, Exception Thrown: {ex.Message}");
        }
        Assert.IsNotNull(testException);
    }
    [TestMethod]
    public async Task GetMessageByIdAsync_ValidInputs_ReturnsSuccessfulResponse()
    {
        //arrange
        var mailboxId = "valid-mailbox-id";
        var messageId = "valid-message-id";

        //var response = new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.OK, "Success");

        var getMessageReponse = new GetMessageResponse
        {
            fileAttachment = new FileAttachment(),
            messageMetaData = new MessageMetaData
            {
                ToMailbox = mailboxId,
                MessageId = messageId,
            }
        };

        var headers = new Dictionary<string,string>(){
            {"mex-messagetype","DATA"},
            {"mex-to",mailboxId},
            {"mex-messageid",messageId}
        };

        var response = UnitTestHelpers.CreateMockHttpResponseMessage<GetMessageResponse>(getMessageReponse,HttpStatusCode.OK,headers);

        _meshConnectClient.Setup(c => c.SendRequestAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(response);

        //act
        var result = await _meshInboxService.GetMessageByIdAsync(mailboxId, messageId);

        //assert
        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual(messageId,result.Response.messageMetaData.MessageId);

    }

    [TestMethod]
    public async Task GetHeadMessageByIdAsync_ValidInputs_ReturnsSuccessfulResponse()
    {
        //arrange
        var mailboxId = "valid-mailbox-id";
        var messageId = "valid-message-id";

        HeadMessageResponse headMessageResponse = new HeadMessageResponse
        {
            messageMetaData = new MessageMetaData
            {
                ToMailbox = mailboxId,
                MessageId = messageId
            }
        };

        var headers = new Dictionary<string,string>(){
            {"mex-messagetype","DATA"},
            {"mex-to",mailboxId},
            {"mex-messageid",messageId}
        };

        var response = UnitTestHelpers.CreateMockHttpResponseMessage<HeadMessageResponse>(headMessageResponse,HttpStatusCode.OK,headers);

        _meshConnectClient.Setup(c => c.SendRequestAsync(It.Is<HttpRequestMessage>(msg => msg.Method == HttpMethod.Head)))
            .ReturnsAsync(response);

        //act
        var result = await _meshInboxService.GetHeadMessageByIdAsync(mailboxId, messageId);

        //assert
        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual(messageId,result.Response.messageMetaData.MessageId);
    }


    [TestMethod]
    public async Task AcknowledgeMessageByIdAsync_ValidInputs_ReturnsSuccessfulResponse()
    {
        //arrange
        var mailboxId = "valid-mailbox-id";
        var messageId = "valid-message-id";

        var acknowledgeMessageResponse = new AcknowledgeMessageResponse
        {
            MessageId = messageId
        };

        var response = UnitTestHelpers.CreateMockHttpResponseMessage<AcknowledgeMessageResponse>(acknowledgeMessageResponse,HttpStatusCode.OK);

        _meshConnectClient.Setup(c => c.SendRequestAsync(It.Is<HttpRequestMessage>(msg => msg.Method == HttpMethod.Put)))
            .ReturnsAsync(response);

        //act
        var result = await _meshInboxService.AcknowledgeMessageByIdAsync(mailboxId, messageId);

        //assert
        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual(messageId,result.Response.MessageId);
    }
}
