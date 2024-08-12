namespace NHS.MESH.Client.IntegrationTests;
using Microsoft.Extensions.DependencyInjection;
using NHS.MESH.Client.Contracts.Services;
using NHS.MESH.Client;
using System.Net;

[TestClass]
public class MeshInboxServiceIntegrationTests
{

    private readonly IMeshInboxService _meshInboxService;
    public MeshInboxServiceIntegrationTests()
    {
        var services = new ServiceCollection();

        services.AddMeshClient(options =>
        {
            options.MeshApiBaseUrl = "http://localhost:8700/messageexchange";
            options.MeshApiHanshakeUriPath = "";
        });

        var serviceProvider = services.BuildServiceProvider();

        _meshInboxService = serviceProvider.GetService<IMeshInboxService>();
    }
    [TestMethod]
    public async Task CheckExistingInboxForMessages_Success()
    {
        //arrange
        var mailboxId = "X26ABC1";
        //act
        var result = await _meshInboxService.GetMessagesAsync(mailboxId);
        //assert
        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual(1, result.Response.Messages.Count());
    }
    [TestMethod]
    public async Task CheckNonExistentInboxForMessages_Failure()
    {
        //arrange
        var mailboxId = "X26ABC12";
        //act
        var result = await _meshInboxService.GetMessagesAsync(mailboxId);
        //assert
        Assert.IsFalse(result.IsSuccessful);
        Assert.AreEqual("Mailbox id does not match token", result.Error.ErrorDescription);
    }



}
