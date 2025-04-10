using System.Net;

using Microsoft.Extensions.DependencyInjection;
using NHS.MESH.Client.Clients;
using NHS.MESH.Client.Configuration;
using NHS.MESH.Client.Contracts.Clients;
using NHS.MESH.Client.Contracts.Configurations;
using NHS.MESH.Client.Contracts.Services;
using NHS.MESH.Client.Services;

public class MeshMailboxBuilder
{
    private IServiceCollection _services;
    private IMeshConnectConfiguration _meshConnectConfiguration;
    private Dictionary<string,MailboxConfiguration> _mailboxes;

    public MeshMailboxBuilder(IServiceCollection services,Action<IMeshConnectConfiguration> options)
    {
        _services = services;

        _meshConnectConfiguration = new MeshConnectConfiguration
        {
            TimeoutInSeconds = 30,
            MaxRetries = 3,
            ProxyAddress = "http://proxy:8080",
            ProxyEnabled = false,
            ProxyUseDefaultCredentials = true,
            MeshApiInboxUriPath = "inbox",
            MeshApiOutboxUriPath = "outbox",
            MeshApiAcknowledgeUriPath = "status/acknowledged",
            ChunkSize = 19 * 1024 * 1024,// below the 20mb limit for external
            BypassServerCertificateValidation = false
        };

        options(_meshConnectConfiguration);

        _mailboxes = new Dictionary<string, MailboxConfiguration>();

    }

    public MeshMailboxBuilder AddMailbox(string mailBoxId, MailboxConfiguration config)
    {
        _mailboxes.Add(mailBoxId,config);

        return this;
    }


    public IServiceCollection Build()
    {

        if(_mailboxes.Count() == 0)
        {
            throw new InvalidOperationException("No Mailboxes were registered");
        }

        if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiBaseUrl))
        {
            throw new MissingFieldException("MeshApiBaseUrl was not set");
        }

        _services.AddSingleton(_meshConnectConfiguration);
        _services.AddTransient<IMeshConnectClient, MeshConnectClient>();

        if (_meshConnectConfiguration.ProxyEnabled)
        {
            _services.AddHttpClient("MeshConnectClient").ConfigurePrimaryHttpMessageHandler(
                    () => new HttpClientHandler
                    {
                        UseProxy = _meshConnectConfiguration.ProxyEnabled,
                        Proxy = new WebProxy(new Uri(_meshConnectConfiguration.ProxyAddress))
                        {
                            UseDefaultCredentials = true,
                        },
                    });
        }
        else
        {
            _services.AddHttpClient("MeshConnectClient").ConfigurePrimaryHttpMessageHandler(
                    () => new HttpClientHandler());
        }

        _services.AddTransient<IMeshInboxService, MeshInboxService>();
        _services.AddTransient<IMeshOutboxService, MeshOutboxService>();
        _services.AddTransient<IMeshOperationService, MeshOperationService>();
        _services.AddSingleton(new MailboxConfigurationResolver(_mailboxes));


        return _services;
    }

}
