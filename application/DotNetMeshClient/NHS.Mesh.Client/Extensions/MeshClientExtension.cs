using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NHS.MESH.Client.Clients;
using NHS.MESH.Client.Configuration;
using NHS.MESH.Client.Contracts.Clients;
using NHS.MESH.Client.Contracts.Configurations;
using NHS.MESH.Client.Contracts.Services;
using NHS.MESH.Client.Services;

namespace NHS.MESH.Client{
  public static class MeshClientServiceExtension{
    public static IHostBuilder AddMeshClient(this IHostBuilder hostBuilder, Action<IMeshConnectConfiguration> options){

      IMeshConnectConfiguration meshConnectConfiguration = new MeshConnectConfiguration
      {
        TimeoutInSeconds = 30,
        MaxRetries = 3,

      };
      // Mesh Connect Client Services.
      hostBuilder.ConfigureServices(_ => {
        _.AddSingleton(meshConnectConfiguration);
        _.AddTransient<IMeshConnectClient,MeshConnectClient>();
        _.AddHttpClient("MeshConnectClient").ConfigurePrimaryHttpMessageHandler(
                () => new HttpClientHandler
                {
                    UseProxy = meshConnectConfiguration.ProxyEnabled,
                    Proxy = new WebProxy(new Uri(meshConnectConfiguration.ProxyAddress))
                    {
                        UseDefaultCredentials = true,
                    },
                });
        _.AddTransient<IMeshInboxService,MeshInboxService>();
        _.AddTransient<IMeshOutboxService,MeshOutboxService>();
        _.AddTransient<IMeshOperationService,MeshOperationService>();
      });

      return hostBuilder;
    }

  }

}
