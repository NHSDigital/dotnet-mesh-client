using Microsoft.Extensions.DependencyInjection;
using NHS.MESH.Client.Contracts.Services;
using NHS.MESH.Client;
using System.Net;

namespace NHS.MESH.Client.IntegrationTests;

[TestClass]
public class MeshOperationServiceIntegrationTests
{

    private readonly IMeshOperationService _meshOperationService;
    public MeshOperationServiceIntegrationTests()
    {
        var services = new ServiceCollection();

        services.AddMeshClient(options => {
            options.MeshApiBaseUrl = "http://localhost:8700/messageexchange";
            options.MeshApiHanshakeUriPath = "";
            options.ProxyEnabled = false;
            options.ProxyAddress = "http://proxy:8080";
            options.ProxyUseDefaultCredentials = true;
        });

        var serviceProvider = services.BuildServiceProvider();

        _meshOperationService = serviceProvider.GetService<IMeshOperationService>();
    }

    [TestMethod]
    public async Task HandshakeToExistingMailbox_Success()
    {
        //arrange
        var mailboxId = "X26ABC1";

        //act
        var result = await _meshOperationService.MeshHandshakeAsync(mailboxId);

        //assert
        Assert.AreEqual(mailboxId,result.Response.MailboxId);
    }

    [TestMethod]
    public async Task HandshakeToNonExistentMailbox_Failure()
    {
        //arrange
        var mailboxId = "NonExistentMailBox";
        Exception testException = null;
        try
        {
        //act
            var result = await _meshOperationService.MeshHandshakeAsync(mailboxId);

        }
        catch(Exception ex){
            testException = ex;

        }
        //assert
        Assert.IsNotNull(testException);
        //add check that exception is correct;
    }
}

