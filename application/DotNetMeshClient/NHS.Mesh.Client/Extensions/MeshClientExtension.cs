using System.Net;
using System.Reflection;
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
    public static IServiceCollection AddMeshClient(this IServiceCollection services, Action<IMeshConnectConfiguration> options){

        IMeshConnectConfiguration meshConnectConfiguration = new MeshConnectConfiguration
        {
            TimeoutInSeconds = 30,
            MaxRetries = 3,
            ProxyAddress = "http://proxy:8080",
            ProxyEnabled = false,
            ProxyUseDefaultCredentials = true,
            MeshApiInboxUriPath = "inbox",
            MeshApiOutboxUriPath = "outbox",



        };

        options(meshConnectConfiguration);
        if(string.IsNullOrWhiteSpace(meshConnectConfiguration.MeshApiBaseUrl)){
          throw new MissingFieldException("MeshApiBaseUrl was not set");
        }
        services.AddSingleton(meshConnectConfiguration);
        services.AddTransient<IMeshConnectClient,MeshConnectClient>();

        if(meshConnectConfiguration.ProxyEnabled)
        {
            services.AddHttpClient("MeshConnectClient").ConfigurePrimaryHttpMessageHandler(
                    () => new HttpClientHandler
                    {
                        UseProxy = meshConnectConfiguration.ProxyEnabled,
                        Proxy = new WebProxy(new Uri(meshConnectConfiguration.ProxyAddress))
                        {
                            UseDefaultCredentials = true,
                        },
                    });
        }
        else
        {
            services.AddHttpClient("MeshConnectClient").ConfigurePrimaryHttpMessageHandler(
                    () => new HttpClientHandler());
        }

        services.AddTransient<IMeshInboxService,MeshInboxService>();
        services.AddTransient<IMeshOutboxService,MeshOutboxService>();
        services.AddTransient<IMeshOperationService,MeshOperationService>();


      return services;
    }

  }

}
