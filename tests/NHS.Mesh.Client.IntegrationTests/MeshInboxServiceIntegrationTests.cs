namespace NHS.MESH.Client.IntegrationTests;
using Microsoft.Extensions.DependencyInjection;
using NHS.MESH.Client.Contracts.Services;
using NHS.MESH.Client;

[TestClass]
[TestCategory("Integration")]
public class MeshInboxServiceIntegrationTests
{

    private readonly IMeshInboxService _meshInboxService;
    public MeshInboxServiceIntegrationTests()
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
        .AddMailbox("X26ABC12",
        new Configuration.MailboxConfiguration
        {
            Password = "password",
            SharedKey = "TestKey"
        })
        .Build();

        var serviceProvider = services.BuildServiceProvider();

        _meshInboxService = serviceProvider.GetService<IMeshInboxService>()!;
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
    }
}
