// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeshConnectClient.cs" company="NHS">
// Copyright (c) NHS. All rights reserved.
// Year: 2024
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NHS.MESH.Client.Clients;

using Microsoft.Extensions.Logging;
using NHS.MESH.Client.Configuration;
using NHS.MESH.Client.Contracts.Clients;
using NHS.MESH.Client.Contracts.Configurations;
using NHS.MESH.Client.Helpers.AuthHelpers;
using NHS.MESH.Client.Models;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

/// <summary>The MESH connect client for MESH API calls.</summary>
public class MeshConnectClient : IMeshConnectClient
{
    /// <summary>The MESH Connect client name constant.</summary>
    //private const string MeshConnectClientName = "MeshConnectClient";

    /// <summary>The MESH Connect Configuration.</summary>
    private readonly IMeshConnectConfiguration _meshConnectConfiguration;

    private readonly MailboxConfigurationResolver _mailboxConfigurationResolver;

    private readonly ILogger<MeshConnectClient> _logger;

    /// <summary>Initializes a new instance of the <see cref="MeshConnectClient"/> class.</summary>
    /// <param name="httpClientFactory">The HTTP Client.</param>
    /// <param name="meshConnectConfiguration">The MESH Connect Configuration.</param>
    public MeshConnectClient(IMeshConnectConfiguration meshConnectConfiguration, MailboxConfigurationResolver mailboxConfigurationResolver, ILogger<MeshConnectClient> logger)
    {
        _meshConnectConfiguration = meshConnectConfiguration;
        _mailboxConfigurationResolver = mailboxConfigurationResolver;
        _logger = logger;
    }

    /// <summary>
    /// Asynchronously sends a request using the route configuration specified.
    /// </summary>
    /// <param name="httpRequestMessage">The HTTP Request Message.</param>
    /// <returns></returns>
    public async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage httpRequestMessage, string mailboxId)
    {
        try
        {
            _logger.LogInformation($"Sending HttpRequest to mesh: { httpRequestMessage.RequestUri }");
            MailboxConfiguration mailboxConfiguration = _mailboxConfigurationResolver.GetMailboxConfiguration(mailboxId);
            var authHeader = MeshAuthorizationHelper.GenerateAuthHeaderValue(mailboxId,mailboxConfiguration.Password!,mailboxConfiguration.SharedKey!);
            httpRequestMessage.Headers.Add("authorization", authHeader);
            var result = await SendHttpRequest(httpRequestMessage,mailboxConfiguration);
            return result;
        }
        catch(Exception ex)
        {
            _logger.LogCritical(ex,"Exception encountered while calling MESH API");
            throw;
        }
    }


    private async Task<HttpResponseMessage> SendHttpRequest(HttpRequestMessage httpRequestMessage,MailboxConfiguration mailboxConfiguration)
    {

        using var handler = new HttpClientHandler();
        httpRequestMessage = AddHeaders(httpRequestMessage);
        var timeInSeconds = _meshConnectConfiguration.TimeoutInSeconds;

        HttpClient httpClient;

        if(mailboxConfiguration.Cert != null)
        {
                _logger.LogInformation("Adding Certificate to HTTP Call");
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ClientCertificates.Add(mailboxConfiguration.Cert);
                handler.SslProtocols = SslProtocols.Tls12;
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                        // It is possible to inspect the certificate provided by the server.
                    _logger.LogInformation($"Requested URI: {httpRequestMessage.RequestUri}");
                    _logger.LogInformation($"Effective date: {cert?.GetEffectiveDateString()}");
                    _logger.LogInformation($"Exp date: {cert?.GetExpirationDateString()}");
                    _logger.LogInformation($"Issuer: {cert?.Issuer}");
                    _logger.LogInformation($"Subject: {cert?.Subject}");

                    // Based on the custom logic it is possible to decide whether the client considers certificate valid or not
                    _logger.LogInformation($"Errors: {policyErrors}");
                    _logger.LogWarning("Bypassing Server certificate Validation Check");
                    return true;
                };
        }

        httpClient = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(timeInSeconds)
        };
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

        return httpResponseMessage;
    }

    private HttpRequestMessage AddHeaders(HttpRequestMessage httpRequestMessage)
    {
        OperatingSystem operatingSystem = Environment.OSVersion;
        var osArchitecture = Environment.Is64BitOperatingSystem ? "x86_64" : "x86_32";
        httpRequestMessage.Headers.Add("mex-clientversion", "ApiDocs==0.0.1");
        httpRequestMessage.Headers.Add("mex-osarchitecture", osArchitecture);
        httpRequestMessage.Headers.Add("mex-osname", operatingSystem.Platform.ToString());
        httpRequestMessage.Headers.Add("mex-osversion", operatingSystem.VersionString);

        return httpRequestMessage;
    }
}
