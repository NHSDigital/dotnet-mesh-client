namespace NHS.MESH.Client;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NHS.MESH.Client.Clients;
using NHS.MESH.Client.Configuration;
using NHS.MESH.Client.Contracts.Clients;
using NHS.MESH.Client.Contracts.Configurations;
using NHS.MESH.Client.Contracts.Services;
using NHS.MESH.Client.Services;

public static class MeshClientServiceExtension
{
    public static MeshMailboxBuilder AddMeshClient(this IServiceCollection services, Action<IMeshConnectConfiguration> options)
    {
        return new MeshMailboxBuilder(services,options);
    }

}
