// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeshConnectClient.cs" company="NHS">
// Copyright (c) NHS. All rights reserved.
// Year: 2024
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using NHS.MESH.Client.Contracts.Clients;
using NHS.MESH.Client.Contracts.Configurations;
using NHS.MESH.Client.Models;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;

namespace NHS.MESH.Client.Clients
{
    /// <summary>The MESH connect client for MESH API calls.</summary>
    public class MeshConnectClient : IMeshConnectClient
    {
        /// <summary>The MESH Connect client name constant.</summary>
        private const string MeshConnectClientName = "MeshConnectClient";

        /// <summary>The HTTP client factory.</summary>
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>The MESH Connect Configuration.</summary>
        private readonly IMeshConnectConfiguration _meshConnectConfiguration;

        /// <summary>Initializes a new instance of the <see cref="MeshConnectClient"/> class.</summary>
        /// <param name="httpClientFactory">The HTTP Client.</param>
        /// <param name="meshConnectConfiguration">The MESH Connect Configuration.</param>
        public MeshConnectClient(IHttpClientFactory httpClientFactory, IMeshConnectConfiguration meshConnectConfiguration)
        {
            _httpClientFactory = httpClientFactory;
            _meshConnectConfiguration = meshConnectConfiguration;
        }

        /// <summary>
        /// Asynchronously sends a request using the route configuration specified.
        /// </summary>
        /// <param name="httpRequestMessage">The HTTP Request Message.</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage httpRequestMessage)
        {
            return await SendHttpRequest(httpRequestMessage);
        }


        private async Task<HttpResponseMessage> SendHttpRequest(HttpRequestMessage httpRequestMessage)
        {
            httpRequestMessage = addHeaders(httpRequestMessage);

            var timeInSeconds = _meshConnectConfiguration.TimeoutInSeconds;

            var httpClient = _httpClientFactory.CreateClient(MeshConnectClientName);

            httpClient.Timeout = TimeSpan.FromSeconds(timeInSeconds);


            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            return httpResponseMessage;
        }

        private HttpRequestMessage addHeaders(HttpRequestMessage httpRequestMessage)
        {
            OperatingSystem operatingSystem = Environment.OSVersion;
            var osArchitecture = Environment.Is64BitOperatingSystem ? "x86_64" : "x86_32";
            httpRequestMessage.Headers.Add("mex-clientversion","ApiDocs==0.0.1");
            httpRequestMessage.Headers.Add("mex-osarchitecture",osArchitecture);
            httpRequestMessage.Headers.Add("mex-osname",operatingSystem.Platform.ToString());
            httpRequestMessage.Headers.Add("mex-osversion",operatingSystem.VersionString);

            return httpRequestMessage;
        }
    }
}
