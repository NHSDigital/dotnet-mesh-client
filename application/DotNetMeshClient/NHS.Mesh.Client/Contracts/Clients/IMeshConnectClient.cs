// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMeshConnectClient.cs" company="NHS">
// Copyright (c) NHS. All rights reserved.
// Year: 2024
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Net;

namespace NHS.MESH.Client.Contracts.Clients
{
    /// <summary>An abstract implementation of a Mesh Connect Client, used to send restful API requests.</summary>
    public interface IMeshConnectClient
    {
        /// <summary>
        /// Asynchronously sends a request using the route configuration specified.
        /// </summary>
        /// <param name="httpRequestMessage">The HTTP Request Message.</param>
        /// <returns></returns>
        Task<KeyValuePair<HttpStatusCode, string>> SendRequestAsync(HttpRequestMessage httpRequestMessage);
    }
}
