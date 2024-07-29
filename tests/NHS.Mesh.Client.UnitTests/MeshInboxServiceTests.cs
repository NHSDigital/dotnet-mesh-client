namespace NHS.Mesh.Client.UnitTests;

using System.Net;
using Microsoft.VisualBasic;
using Moq;
using NHS.MESH.Client.Contracts.Clients;
using NHS.MESH.Client.Contracts.Configurations;
using NHS.MESH.Client.Contracts.Services;
using NHS.MESH.Client.Helpers.AuthHelpers;
using NHS.MESH.Client.Services;

[TestClass]
public class MeshInboxServiceTests
{
        /// <summary>The MESH Connect Configuration.</summary>
        private readonly Mock<IMeshConnectConfiguration> _meshConnectConfiguration;

        /// <summary>The MESH Connect Client.</summary>
        private readonly Mock<IMeshConnectClient> _meshConnectClient;

        /// <summary>The MESH Operation Service.</summary>
        private readonly Mock<IMeshOperationService> _meshOperationService;

        /// <summary>The MESH Inbox Service.</summary>
        private readonly IMeshInboxService _meshInboxService;

    public MeshInboxServiceTests()
    {
            _meshConnectConfiguration = new Mock<IMeshConnectConfiguration>(MockBehavior.Strict);
            _meshConnectClient = new Mock<IMeshConnectClient>(MockBehavior.Strict);
            _meshOperationService = new Mock<IMeshOperationService>(MockBehavior.Strict);

            _meshInboxService = new MeshInboxService(
                _meshConnectConfiguration.Object,
                _meshConnectClient.Object,
                _meshOperationService.Object
            );

            // Setup default values for configuration mock
            _meshConnectConfiguration.SetupGet(c => c.MeshApiBaseUrl).Returns("https://api.mesh.com");
            _meshConnectConfiguration.SetupGet(c => c.MeshApiInboxUriPath).Returns("inbox");
            _meshConnectConfiguration.SetupGet(c => c.MeshApiAcknowledgeUriPath).Returns("acknowledge");

    }
    [TestMethod]
    public void getAuth()
    {
        var authtoken  = MeshAuthorizationHelper.GenerateAuthHeaderValue("X26ABC1",null,"password","TestKey",null,0);
        Assert.AreEqual("aKey",authtoken);
    }

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
    public async Task GetMessagesAsync_HandshakeFails_ReturnsHandshakeError()
    {
        //arrange
        var mailboxId = "valid-mailbox-id";
        var handshakeResult = new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Unauthorized, "Unauthorized");
        _meshOperationService.Setup(s => s.MeshHandshakeAsync(mailboxId)).ReturnsAsync(handshakeResult);

        //act
        var result = await _meshInboxService.GetMessagesAsync(mailboxId);

        //assert
        Assert.AreEqual(HttpStatusCode.Unauthorized,result.Key);
        Assert.AreEqual("Unauthorized",result.Value);
    }

    [TestMethod]
    public async Task GetMessagesAsync_ValidInputs_ReturnsSuccessfulResponse()
    {
        //arrange
        var mailboxId = "valid-mailbox-id";
        var handshakeResult = new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.OK, "OK");
        var response = new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.OK, "Success");

        _meshOperationService.Setup(s => s.MeshHandshakeAsync(mailboxId)).ReturnsAsync(handshakeResult);
        _meshConnectClient.Setup(c => c.SendRequestAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(response);

        //act
        var result = await _meshInboxService.GetMessagesAsync(mailboxId);

        //assert
        Assert.AreEqual(HttpStatusCode.OK,result.Key);
        Assert.AreEqual("Success",result.Value);
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
        _meshOperationService.Setup(s => s.MeshHandshakeAsync(mailboxId))
            .ReturnsAsync(new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.OK, "OK"));

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
        var handshakeResult = new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.OK, "OK");
        var response = new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.OK, "Success");

        _meshOperationService.Setup(s => s.MeshHandshakeAsync(mailboxId)).ReturnsAsync(handshakeResult);
        _meshConnectClient.Setup(c => c.SendRequestAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(response);

        //act
        var result = await _meshInboxService.GetMessageByIdAsync(mailboxId, messageId);

        //assert
        Assert.AreEqual(HttpStatusCode.OK,result.Key);
        Assert.AreEqual("Success",result.Value);

    }

    [TestMethod]
    public async Task GetHeadMessageByIdAsync_ValidInputs_ReturnsSuccessfulResponse()
    {
        //arrange
        var mailboxId = "valid-mailbox-id";
        var messageId = "valid-message-id";
        var handshakeResult = new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.OK, "OK");
        var headResponse = new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.OK, "Head Success");
        var acknowledgeResponse = new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.OK, "Acknowledge Success");

        _meshOperationService.Setup(s => s.MeshHandshakeAsync(mailboxId)).ReturnsAsync(handshakeResult);
        _meshConnectClient.Setup(c => c.SendRequestAsync(It.Is<HttpRequestMessage>(msg => msg.Method == HttpMethod.Head)))
            .ReturnsAsync(headResponse);
        _meshConnectClient.Setup(c => c.SendRequestAsync(It.Is<HttpRequestMessage>(msg => msg.Method == HttpMethod.Put)))
            .ReturnsAsync(acknowledgeResponse);

        //act
        var result = await _meshInboxService.GetHeadMessageByIdAsync(mailboxId, messageId);

        //assert
        Assert.AreEqual(HttpStatusCode.OK,result.Key);
        Assert.AreEqual("Acknowledge Success",result.Value);
    }


    [TestMethod]
    public async Task AcknowledgeMessageByIdAsync_ValidInputs_ReturnsSuccessfulResponse()
    {
        //arrange
        var mailboxId = "valid-mailbox-id";
        var messageId = "valid-message-id";
        var handshakeResult = new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.OK, "OK");
        var response = new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.OK, "Success");

        _meshOperationService.Setup(s => s.MeshHandshakeAsync(mailboxId)).ReturnsAsync(handshakeResult);
        _meshConnectClient.Setup(c => c.SendRequestAsync(It.Is<HttpRequestMessage>(msg => msg.Method == HttpMethod.Put)))
            .ReturnsAsync(response);

        //act
        var result = await _meshInboxService.AcknowledgeMessageByIdAsync(mailboxId, messageId);

        //assert
        Assert.AreEqual(HttpStatusCode.OK,result.Key);
        Assert.AreEqual("Success",result.Value);
    }
}
