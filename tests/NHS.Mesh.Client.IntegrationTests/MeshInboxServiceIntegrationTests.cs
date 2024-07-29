using Microsoft.Extensions.DependencyInjection;
using NHS.MESH.Client.Contracts.Services;
using NHS.MESH.Client;
using System.Net;

namespace NHS.Mesh.Client.IntegrationTests;

[TestClass]
public class MeshInboxServiceIntegrationTests
{

    private readonly IMeshInboxService _meshInboxService;
    public MeshInboxServiceIntegrationTests()
    {
        var services = new ServiceCollection();

        services.AddMeshClient(options => {
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
        Assert.AreEqual(HttpStatusCode.OK,result.Key);
    }
    [TestMethod]
    public async Task CheckNonExistentInboxForMessages_Failure()
    {
        //arrange
        var mailboxId = "X26ABC12";
        //act
        var result = await _meshInboxService.GetMessagesAsync(mailboxId);
        //assert
        Assert.AreEqual(HttpStatusCode.Forbidden,result.Key);
    }



}
