// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeshOperationService.cs" company="NHS">
// Copyright (c) NHS. All rights reserved.
// Year: 2024
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace NHS.MESH.Client.Services;
using NHS.MESH.Client.Contracts.Clients;
using NHS.MESH.Client.Contracts.Configurations;
using NHS.MESH.Client.Contracts.Services;
using NHS.MESH.Client.Helpers;
using NHS.MESH.Client.Helpers.AuthHelpers;
using NHS.MESH.Client.Models;
using System.Net;
using System.Text.Json;

/// <summary>The MESH Operations service.</summary>
public class MeshOperationService : IMeshOperationService
{
    /// <summary>The MESH Connect Configuration.</summary>
    private readonly IMeshConnectConfiguration _meshConnectConfiguration;

    /// <summary>The MESH Connect Client.</summary>
    private readonly IMeshConnectClient _meshConnectClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="MeshOperationService"/> class.
    /// </summary>
    /// <param name="meshConnectConfiguration">The MESH Connect Configuration.</param>
    /// <param name="meshConnectClient">The MESH Connect Client.</param>
    public MeshOperationService(IMeshConnectConfiguration meshConnectConfiguration, IMeshConnectClient meshConnectClient)
    {
        _meshConnectConfiguration = meshConnectConfiguration;
        _meshConnectClient = meshConnectClient;
    }

    /// <summary>
    /// Validate a MESH Mailbox asynchronously.
    /// </summary>
    /// <param name="mailboxId">The Mailbox Id.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">The Arugument Null Exception.</exception>
    /// <exception cref="Exception">The general Exception.</exception>
    public async Task<MeshResponse<HandshakeResponse>> MeshHandshakeAsync(string mailboxId)
    {
        // Validations
        if (string.IsNullOrWhiteSpace(mailboxId)) { throw new ArgumentNullException(nameof(mailboxId)); }

        if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiBaseUrl)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiBaseUrl)); }

        // The HTTP Request Message
        var httpRequestMessage = new HttpRequestMessage();



        // API URL
        var uri = new Uri(_meshConnectConfiguration.MeshApiBaseUrl + "/" + mailboxId);
        httpRequestMessage.RequestUri = uri;

        // Request Method
        httpRequestMessage.Method = HttpMethod.Get;

        // Headers
        //httpRequestMessage.Headers.Add("accept", "application/vnd.mesh.v2+json");
        httpRequestMessage.Headers.Add("accept", "*/*");
        httpRequestMessage.Headers.Add("User_Agent", "my-client;windows-10;");

        var meshResponse = await _meshConnectClient.SendRequestAsync(httpRequestMessage,mailboxId);
        var stringContent = await meshResponse.Content.ReadAsStringAsync();

        var response = await ResponseHelper.CreateMeshResponse<HandshakeResponse>(meshResponse, async _ => JsonSerializer.Deserialize<HandshakeResponse>(await _.Content.ReadAsStringAsync()));

        return response;
    }

}
