// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMeshConnectClient.cs" company="NHS">
// Copyright (c) NHS. All rights reserved.
// Year: 2024
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NHS.MESH.Client.Contracts.Clients;
using System.Net;
using NHS.MESH.Client.Models;

/// <summary>An abstract implementation of a Mesh Connect Client, used to send restful API requests.</summary>
public interface IMeshConnectClient
{
    /// <summary>
    /// Asynchronously sends a request using the route configuration specified.
    /// </summary>
    /// <param name="httpRequestMessage">The HTTP Request Message.</param>
    /// <param name="mailboxId">The Sending Mailboxid</param>
    /// <returns></returns>
    Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage httpRequestMessage, string mailboxId);
}
