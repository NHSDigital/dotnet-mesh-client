namespace NHS.MESH.Client.IntegrationTests;
using Microsoft.Extensions.DependencyInjection;
using NHS.MESH.Client.Contracts.Services;
using NHS.MESH.Client;
[TestClass]
[TestCategory("Integration")]
public class MeshOperationServiceIntegrationTests
{

    private readonly IMeshOperationService _meshOperationService;
    public MeshOperationServiceIntegrationTests()
    {
        var services = new ServiceCollection();

        services.AddMeshClient(options =>
        {
            options.MeshApiBaseUrl = "http://localhost:8700/messageexchange";
        })
        .AddMailbox("X26ABC1",
        new Configuration.MailboxConfiguration
        {
            Password = "password",
            SharedKey = "TestKey"
        })
        .AddMailbox("NonExistentMailBox",
        new Configuration.MailboxConfiguration
        {
            Password = "password",
            SharedKey = "TestKey"
        })
        .Build();

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
        Assert.AreEqual(mailboxId, result.Response.MailboxId);
    }

    [TestMethod]
    public async Task HandshakeToNonExistentMailbox_Failure()
    {
        //arrange
        var mailboxId = "NonExistentMailBox";
        //act
        var result = await _meshOperationService.MeshHandshakeAsync(mailboxId);

        //assert

        Assert.IsFalse(result.IsSuccessful);
        Assert.AreEqual("Mailbox id does not match token", result.Error.ErrorDescription);

    }
}

