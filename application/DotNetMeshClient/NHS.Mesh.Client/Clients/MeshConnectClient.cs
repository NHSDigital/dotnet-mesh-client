// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeshConnectClient.cs" company="NHS">
// Copyright (c) NHS. All rights reserved.
// Year: 2024
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using NHS.MESH.Client.Contracts.Clients;
using NHS.MESH.Client.Contracts.Configurations;
using System.Net;
using System.Net.Http;

namespace NHS.MESH.Client.Clients
{
    /// <summary>The MESH connect client for MESH API calls.</summary>
    public class MeshConnectClient : IMeshConnectClient
    {
        /// <summary>The MESH Connect client name constant.</summary>
        private const string MeshConnectClientName = "MeshConnectClient";

        /// <summary>The HTTP client factory.</summary>
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>The MESH Connect Configuration.</summary>
        private readonly IMeshConnectConfiguration meshConnectConfiguration;

        /// <summary>Initializes a new instance of the <see cref="MeshConnectClient"/> class.</summary>
        /// <param name="httpClientFactory">The HTTP Client.</param>
        /// <param name="meshConnectConfiguration">The MESH Connect Configuration.</param>
        public MeshConnectClient(IHttpClientFactory httpClientFactory, IMeshConnectConfiguration meshConnectConfiguration)
        {
            this.httpClientFactory = httpClientFactory;
            this.meshConnectConfiguration = meshConnectConfiguration;
        }

        /// <summary>
        /// Asynchronously sends a request using the route configuration specified.
        /// </summary>
        /// <param name="httpRequestMessage">The HTTP Request Message.</param>
        /// <returns></returns>
        public async Task<KeyValuePair<HttpStatusCode, string>> SendRequestAsync(HttpRequestMessage httpRequestMessage)
        {
            try
            {
                var timeInSeconds = this.meshConnectConfiguration.TimeoutInSeconds;

                var httpClient = httpClientFactory.CreateClient(MeshConnectClientName);

                httpClient.Timeout = TimeSpan.FromSeconds(timeInSeconds);

                var httpResponseMessage = httpClient.SendAsync(httpRequestMessage);

                var response = httpResponseMessage.Result;

                var responseContent = await response.Content.ReadAsStringAsync();

                return new KeyValuePair<HttpStatusCode, string>(response.StatusCode, responseContent);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
