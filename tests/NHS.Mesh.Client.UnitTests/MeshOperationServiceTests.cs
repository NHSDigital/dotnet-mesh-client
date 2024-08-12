namespace NHS.MESH.Client.UnitTests;

using System.Net;
using System.Text.Json;
using Moq;
using NHS.MESH.Client.Contracts.Clients;
using NHS.MESH.Client.Contracts.Configurations;
using NHS.MESH.Client.Contracts.Services;
using NHS.MESH.Client.Models;
using NHS.MESH.Client.Services;

[TestClass]
public class MeshOperationServiceTests
{
    private readonly Mock<IMeshConnectConfiguration> _meshConnectConfiguration;
    private readonly Mock<IMeshConnectClient> _meshConnectClient;
    private readonly MeshOperationService _meshOperationService;

    public MeshOperationServiceTests()
    {
        _meshConnectConfiguration = new Mock<IMeshConnectConfiguration>(MockBehavior.Strict);
        _meshConnectClient = new Mock<IMeshConnectClient>(MockBehavior.Strict);
        _meshOperationService = new MeshOperationService(_meshConnectConfiguration.Object, _meshConnectClient.Object);
    }

    [TestMethod]
    public async Task MeshHandshakeAsync_MailboxIdNull_ThrowsArgumentNullException()
    {

        //arrange
        Exception testException = null;
        OperatingSystem operatingSystem = Environment.OSVersion;

        // Act
        try
        {
            await _meshOperationService.MeshHandshakeAsync(null);
        }
        //Assert
        catch (ArgumentNullException ex)
        {
            testException = ex;
            Assert.AreEqual(ex.ParamName, "mailboxId");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Incorrect Exception was thrown, Exception Thrown: {ex.Message}");
        }
        Assert.IsNotNull(testException);
    }


    [TestMethod]
    public async Task MeshHandshakeAsync_MeshApiBaseUrlNull_ThrowsArgumentNullException()
    {
        // Arrange
        Exception testException = null;
        var mailboxId = "valid-mailbox-id";
        _meshConnectConfiguration.SetupGet(c => c.MeshApiBaseUrl).Returns((string)null);

        // Act & Assert

        try
        {
            await _meshOperationService.MeshHandshakeAsync(mailboxId);
        }
        //Assert
        catch (ArgumentNullException ex)
        {
            testException = ex;
            Assert.AreEqual(ex.ParamName, "MeshApiBaseUrl");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Incorrect Exception was thrown, Exception Thrown: {ex.Message}");
        }
        Assert.IsNotNull(testException);
    }

    [TestMethod]
    public async Task MeshHandshakeAsync_ValidInputs_ReturnsSuccessfulResponse()
    {
        // Arrange
        var mailboxId = "valid-mailbox-id";
        var responseData = new HandshakeResponse
        {
            MailboxId = mailboxId
        };
        string jsonString = JsonSerializer.Serialize(responseData);

        var meshApiBaseUrl = "https://api.mesh.com";
        var response = UnitTestHelpers.CreateMockHttpResponseMessage<HandshakeResponse>(new HandshakeResponse { MailboxId = mailboxId }, HttpStatusCode.OK);

        _meshConnectConfiguration.SetupGet(c => c.MeshApiBaseUrl).Returns(meshApiBaseUrl);
        _meshConnectConfiguration.SetupGet(c => c.MaxRetries).Returns(3);
        _meshConnectClient.Setup(c => c.SendRequestAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(response);

        // Act
        var result = await _meshOperationService.MeshHandshakeAsync(mailboxId);

        // Assert
        Assert.AreEqual(mailboxId, result.Response.MailboxId);
    }

    [TestMethod]
    public async Task MeshHandshakeAsync_MaxRetriesReached_ReturnsLastResponse()
    {
        // Arrange
        var mailboxId = "valid-mailbox-id";
        var meshApiBaseUrl = "https://api.mesh.com";
        var errorDescription = "Service unavailable";
        var errorString = UnitTestHelpers.CreateMeshErrorResponseJsonString(null, null, errorDescription);

        var httpResponseMock = UnitTestHelpers.CreateMockHttpResponseMessage<string>(errorString, HttpStatusCode.ServiceUnavailable);

        Exception testException = null;


        _meshConnectConfiguration.SetupGet(c => c.MeshApiBaseUrl).Returns(meshApiBaseUrl);
        _meshConnectConfiguration.SetupGet(c => c.MaxRetries).Returns(3);
        _meshConnectClient.Setup(c => c.SendRequestAsync(It.IsAny<HttpRequestMessage>()))
            .ReturnsAsync(httpResponseMock);

        // Act

        var result = await _meshOperationService.MeshHandshakeAsync(mailboxId);


        Assert.IsFalse(result.IsSuccessful);
        Assert.AreEqual(errorDescription, result.Error.ErrorDescription);
    }

    [TestMethod]
    public async Task MeshHandshakeAsync_HttpRequestException_ThrowsException()
    {
        // Arrange
        Exception testException = null;
        var mailboxId = "valid-mailbox-id";
        var meshApiBaseUrl = "https://api.mesh.com";

        _meshConnectConfiguration.SetupGet(c => c.MeshApiBaseUrl).Returns(meshApiBaseUrl);
        _meshConnectConfiguration.SetupGet(c => c.MaxRetries).Returns(3);
        _meshConnectClient.Setup(c => c.SendRequestAsync(It.IsAny<HttpRequestMessage>())).ThrowsAsync(new HttpRequestException("Request failed"));

        //act
        try
        {
            await _meshOperationService.MeshHandshakeAsync(mailboxId);
        }
        //Assert
        catch (HttpRequestException ex)
        {
            testException = ex;
            Assert.AreEqual(ex.Message, "Request failed");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Incorrect Exception was thrown, Exception Thrown: {ex.Message}");
        }
        Assert.IsNotNull(testException);
    }
}
