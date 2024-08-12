namespace NHS.MESH.Client.UnitTests;

using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using NHS.MESH.Client.Contracts.Clients;
using NHS.MESH.Client.Contracts.Configurations;
using NHS.MESH.Client.Contracts.Services;
using NHS.MESH.Client.Models;
using NHS.MESH.Client.Services;

[TestClass]
public class MeshOutboxServiceTests
{
    private readonly Mock<IMeshConnectConfiguration> _meshConnectConfiguration;
    private readonly Mock<IMeshConnectClient> _meshConnectClient;
    private readonly IMeshOutboxService _meshOutboxService;
    private readonly Mock<ILogger<MeshOutboxService>> _logger;

    public MeshOutboxServiceTests()
    {
        _meshConnectConfiguration = new Mock<IMeshConnectConfiguration>(MockBehavior.Strict);
        _meshConnectClient = new Mock<IMeshConnectClient>(MockBehavior.Strict);

        _logger = new Mock<ILogger<MeshOutboxService>>(MockBehavior.Loose);

        _meshOutboxService = new MeshOutboxService(
            _meshConnectConfiguration.Object,
            _meshConnectClient.Object,
            _logger.Object

        );

        // Setup default values for configuration mock
        _meshConnectConfiguration.SetupGet(c => c.MeshApiBaseUrl).Returns("https://api.mesh.com");
        _meshConnectConfiguration.SetupGet(c => c.MeshApiOutboxUriPath).Returns("outbox");
    }

    [TestMethod]
    public async Task SendCompressedMessageAsync_FromMailboxIdNull_ThrowsArgumentNullException()
    {
        // Arrange
        var fileMock = new Mock<FileAttachment>();
        Exception testException = null;

        // Act & Assert
        try
        {
            await _meshOutboxService.SendCompressedMessageAsync(null, "toMailboxId", "WorkflowId", fileMock.Object);
        }
        catch (ArgumentNullException ex)
        {
            testException = ex;
        }
        catch (Exception ex)
        {
            Assert.Fail();
        }

        Assert.IsNotNull(testException);

    }

    [TestMethod]
    public async Task SendCompressedMessageAsync_ToMailboxIdNull_ThrowsArgumentNullException()
    {
        // Arrange
        var fileMock = new Mock<FileAttachment>();
        Exception testException = null;

        // Act & Assert
        try
        {
            await _meshOutboxService.SendCompressedMessageAsync("fromMailBox", null, "WorkflowId", fileMock.Object);
        }
        catch (ArgumentNullException ex)
        {
            testException = ex;
        }
        catch (Exception ex)
        {
            Assert.Fail();
        }

        Assert.IsNotNull(testException);

    }

    [TestMethod]
    public async Task SendCompressedMessageAsync_MeshApiBaseUrlNull_ThrowsArgumentNullException()
    {
        // Arrange
        _meshConnectConfiguration.SetupGet(c => c.MeshApiBaseUrl).Returns((string)null);
        var fileMock = new Mock<FileAttachment>();
        Exception testException = null;

        // Act & Assert
        try
        {
            await _meshOutboxService.SendCompressedMessageAsync("fromMailBox", "toMailbox", "WorkflowId", fileMock.Object);
        }
        catch (ArgumentNullException ex)
        {
            testException = ex;
        }
        catch (Exception ex)
        {
            Assert.Fail();
        }

        Assert.IsNotNull(testException);
    }

    [TestMethod]
    public async Task SendCompressedMessageAsync_MeshApiOutboxUriPathNull_ThrowsArgumentNullException()
    {
        // Arrange
        _meshConnectConfiguration.SetupGet(c => c.MeshApiOutboxUriPath).Returns((string)null);
        var fileMock = new Mock<FileAttachment>();
        Exception testException = null;

        // Act & Assert
        try
        {
            await _meshOutboxService.SendCompressedMessageAsync("fromMailBox", "toMailbox", "WorkflowId", fileMock.Object);
        }
        catch (ArgumentNullException ex)
        {
            testException = ex;
        }
        catch (Exception ex)
        {
            Assert.Fail();
        }

        Assert.IsNotNull(testException);
    }

    [TestMethod]
    public async Task SendCompressedMessageAsync_ValidInputs_ReturnsSuccessfulResponse()
    {
        // Arrange
        var fromMailboxId = "fromMailboxId";
        var toMailboxId = "toMailboxId";
        var workflowId = "workflowId";
        var messageId = "MessageId";

        var fileAttachment = new FileAttachment
        {
            FileName = "FileName",
            Content = UnitTestHelpers.CreateFakeFileContent(2 * 1024 * 1024),
            ContentType = "application/octet-stream"

        };

        SendMessageResponse sendMessageResponse = new SendMessageResponse
        {
            MessageId = messageId,

        };

        var response = UnitTestHelpers.CreateMockHttpResponseMessage<SendMessageResponse>(sendMessageResponse, HttpStatusCode.OK);

        _meshConnectClient.Setup(c => c.SendRequestAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(response);

        // Act
        var result = await _meshOutboxService.SendCompressedMessageAsync(fromMailboxId, toMailboxId, workflowId, fileAttachment);

        // Assert
        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual(messageId, result.Response.MessageId);
    }


    [TestMethod]
    public async Task SendUnCompressedMessageAsync_FromMailboxIdNull_ThrowsArgumentNullException()
    {
        // Arrange
        var fileAttachment = new FileAttachment();
        Exception testException = null;
        // Act & Assert
        try
        {
            await _meshOutboxService.SendUnCompressedMessageAsync(null, "toMailboxId", "workflowId", fileAttachment);
        }
        catch (ArgumentNullException ex)
        {
            testException = ex;
        }
        catch (Exception ex)
        {
            Assert.Fail();
        }

        Assert.IsNotNull(testException);
    }

    [TestMethod]
    public async Task SendUnCompressedMessageAsync_ToMailboxIdNull_ThrowsArgumentNullException()
    {
        // Arrange
        var fileAttachment = new FileAttachment();
        Exception testException = null;
        // Act & Assert
        try
        {
            await _meshOutboxService.SendUnCompressedMessageAsync("fromMailboxId", null, "workflowId", fileAttachment);
        }
        catch (ArgumentNullException ex)
        {
            testException = ex;
        }
        catch (Exception ex)
        {
            Assert.Fail();
        }

        Assert.IsNotNull(testException);
    }

    [TestMethod]
    public async Task SendUnCompressedMessageAsync_MeshApiBaseUrlNull_ThrowsArgumentNullException()
    {
        // Arrange
        _meshConnectConfiguration.SetupGet(c => c.MeshApiBaseUrl).Returns((string)null);
        var fileAttachment = new FileAttachment();
        Exception testException = null;
        // Act & Assert
        try
        {
            await _meshOutboxService.SendUnCompressedMessageAsync("fromMailboxId", "toMailboxId", "workflowId", fileAttachment);
        }
        catch (ArgumentNullException ex)
        {
            testException = ex;
        }
        catch (Exception ex)
        {
            Assert.Fail();
        }

        Assert.IsNotNull(testException);
    }

    [TestMethod]
    public async Task SendUnCompressedMessageAsync_MeshApiOutboxUriPathNull_ThrowsArgumentNullException()
    {
        // Arrange
        _meshConnectConfiguration.SetupGet(c => c.MeshApiOutboxUriPath).Returns((string)null);
        var fileAttachment = new FileAttachment();
        Exception testException = null;
        // Act & Assert
        try
        {
            await _meshOutboxService.SendUnCompressedMessageAsync("fromMailboxId", "toMailboxId", "workflowId", fileAttachment);
        }
        catch (ArgumentNullException ex)
        {
            testException = ex;
        }
        catch (Exception ex)
        {
            Assert.Fail();
        }

        Assert.IsNotNull(testException);
    }

    [TestMethod]
    public async Task SendUnCompressedMessageAsync_ValidInputs_ReturnsSuccessfulResponse()
    {
        // Arrange
        var fromMailboxId = "fromMailboxId";
        var toMailboxId = "toMailboxId";
        var workflowId = "workflowId";
        var messageId = "MessageId";

        var fileAttachment = new FileAttachment
        {
            FileName = "FileName",
            Content = UnitTestHelpers.CreateFakeFileContent(2 * 1024 * 1024),
            ContentType = "application/octet-stream"

        };

        SendMessageResponse sendMessageResponse = new SendMessageResponse
        {
            MessageId = messageId
        };

        var response = UnitTestHelpers.CreateMockHttpResponseMessage<SendMessageResponse>(sendMessageResponse, HttpStatusCode.OK);

        _meshConnectConfiguration.SetupGet(i => i.ChunkSize).Returns(5 * 1024 * 1024);
        _meshConnectClient.Setup(c => c.SendRequestAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(response);

        // Act
        var result = await _meshOutboxService.SendUnCompressedMessageAsync(fromMailboxId, toMailboxId, workflowId, fileAttachment);

        // Assert
        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual(messageId, result.Response.MessageId);
    }


    [TestMethod]
    public async Task SendChunkedMessageAsync_FromMailboxIdNull_ThrowsArgumentNullException()
    {
        // Arrange
        _meshConnectConfiguration.SetupGet(c => c.MeshApiOutboxUriPath).Returns((string)null);
        var fileAttachment = new FileAttachment();
        Exception testException = null;
        // Act & Assert
        try
        {
            await _meshOutboxService.SendChunkedMessageAsync(null, "toMailboxId", "workSpaceId", fileAttachment);
        }
        catch (ArgumentNullException ex)
        {
            testException = ex;
        }
        catch (Exception ex)
        {
            Assert.Fail();
        }

        Assert.IsNotNull(testException);
    }

    [TestMethod]
    public async Task SendChunkedMessageAsync_ToMailboxIdNull_ThrowsArgumentNullException()
    {
        // Arrange
        _meshConnectConfiguration.SetupGet(c => c.MeshApiOutboxUriPath).Returns((string)null);
        var fileAttachment = new FileAttachment();
        Exception testException = null;
        // Act & Assert
        try
        {
            await _meshOutboxService.SendChunkedMessageAsync("fromMailboxId", null, "workSpaceId", fileAttachment);
        }
        catch (ArgumentNullException ex)
        {
            testException = ex;
        }
        catch (Exception ex)
        {
            Assert.Fail();
        }

        Assert.IsNotNull(testException);
    }

    [TestMethod]
    public async Task SendChunkedMessageAsync_MeshApiBaseUrlNull_ThrowsArgumentNullException()
    {
        // Arrange
        _meshConnectConfiguration.SetupGet(c => c.MeshApiBaseUrl).Returns((string)null);
        var fileAttachment = new FileAttachment();
        Exception testException = null;
        // Act & Assert
        try
        {
            await _meshOutboxService.SendChunkedMessageAsync("fromMailboxId", "toMailboxId", "workSpaceId", fileAttachment);
        }
        catch (ArgumentNullException ex)
        {
            testException = ex;
        }
        catch (Exception ex)
        {
            Assert.Fail();
        }

        Assert.IsNotNull(testException);
    }

    [TestMethod]
    public async Task SendChunkedMessageAsync_MeshApiOutboxUriPathNull_ThrowsArgumentNullException()
    {
        // Arrange
        _meshConnectConfiguration.SetupGet(c => c.MeshApiOutboxUriPath).Returns((string)null);
        var fileAttachment = new FileAttachment();
        Exception testException = null;
        // Act & Assert
        try
        {
            await _meshOutboxService.SendChunkedMessageAsync("fromMailboxId", "toMailboxId", "workSpaceId", fileAttachment);
        }
        catch (ArgumentNullException ex)
        {
            testException = ex;
        }
        catch (Exception ex)
        {
            Assert.Fail();
        }

        Assert.IsNotNull(testException);
    }


    [TestMethod]
    public async Task SendChunkedMessageAsync_ValidInputs_ReturnsSuccessfulResponse()
    {
        // Arrange
        var fromMailboxId = "fromMailboxId";
        var toMailboxId = "toMailboxId";
        var workflowId = "WorkflowId";
        var messageId = "MessageId";

        FileAttachment fileAttachment = new FileAttachment
        {
            FileName = "FileName",
            Content = UnitTestHelpers.CreateFakeFileContent(9 * 1024 * 1024),
            ContentType = "application/octet-stream"
        };

        var response = new SendMessageResponse
        {
            MessageId = messageId
        };
        _meshConnectConfiguration.SetupGet(i => i.ChunkSize).Returns(5 * 1024 * 1024);

        _meshConnectClient.Setup(i => i.SendRequestAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(UnitTestHelpers.CreateMockHttpResponseMessage<SendMessageResponse>(response, HttpStatusCode.OK));


        // Act
        var result = await _meshOutboxService.SendChunkedMessageAsync(fromMailboxId, toMailboxId, workflowId, fileAttachment);

        // Assert
        _meshConnectClient.Verify(i => i.SendRequestAsync(It.IsAny<HttpRequestMessage>()), Times.Exactly(2));
        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual(messageId, result.Response.MessageId);
    }


    [TestMethod]
    public async Task TrackMessageByIdAsync_MailboxIdNull_ThrowsArgumentNullException()
    {
        // Arrange
        Exception testException = null;
        // Act & Assert
        try
        {
            await _meshOutboxService.TrackMessageByIdAsync(null, "messageId");
        }
        catch (ArgumentNullException ex)
        {
            testException = ex;
        }
        catch (Exception ex)
        {
            Assert.Fail();
        }

        Assert.IsNotNull(testException);
    }

    [TestMethod]
    public async Task TrackMessageByIdAsync_MeshApiBaseUrlNull_ThrowsArgumentNullException()
    {
        // Arrange
        _meshConnectConfiguration.SetupGet(c => c.MeshApiBaseUrl).Returns((string)null);

        Exception testException = null;
        // Act & Assert
        try
        {
            await _meshOutboxService.TrackMessageByIdAsync("mailboxId", "messageId");
        }
        catch (ArgumentNullException ex)
        {
            testException = ex;
        }
        catch (Exception ex)
        {
            Assert.Fail();
        }

        Assert.IsNotNull(testException);
    }

    [TestMethod]
    public async Task TrackMessageByIdAsync_MeshApiOutboxUriPathNull_ThrowsArgumentNullException()
    {
        // Arrange
        _meshConnectConfiguration.SetupGet(c => c.MeshApiOutboxUriPath).Returns((string)null);

        Exception testException = null;
        // Act & Assert
        try
        {
            await _meshOutboxService.TrackMessageByIdAsync("mailboxId", "messageId");
        }
        catch (ArgumentNullException ex)
        {
            testException = ex;
        }
        catch (Exception ex)
        {
            Assert.Fail();
        }

        Assert.IsNotNull(testException);
    }

    [TestMethod]
    public async Task TrackMessageByIdAsync_ValidInputs_ReturnsSuccessfulResponse()
    {
        // Arrange
        var mailboxId = "mailboxId";
        var messageId = "messageId";
        var workflowId = "workflowId";
        var fileName = "fileName";

        var trackOutboxResponse = new TrackOutboxResponse
        {
            MessageId = messageId,
            WorkflowId = workflowId,
            FileName = fileName
        };


        _meshConnectConfiguration.SetupGet(c => c.MeshApiBaseUrl).Returns("https://api.mesh.com");
        _meshConnectConfiguration.SetupGet(c => c.MeshApiOutboxUriPath).Returns("outbox");
        _meshConnectConfiguration.SetupGet(c => c.MeshApiInboxUriPath).Returns("inbox");
        _meshConnectConfiguration.SetupGet(c => c.MeshApiTrackMessageUriPath).Returns("track");

        _meshConnectClient.Setup(c => c.SendRequestAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(UnitTestHelpers.CreateMockHttpResponseMessage<TrackOutboxResponse>(trackOutboxResponse, HttpStatusCode.OK));

        // Act
        var result = await _meshOutboxService.TrackMessageByIdAsync(mailboxId, messageId);

        // Assert
        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual(messageId, result.Response.MessageId);
    }
}
