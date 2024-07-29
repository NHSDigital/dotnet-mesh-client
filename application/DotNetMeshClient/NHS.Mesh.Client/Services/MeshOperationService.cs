// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeshOperationService.cs" company="NHS">
// Copyright (c) NHS. All rights reserved.
// Year: 2024
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using NHS.MESH.Client.Contracts.Clients;
using NHS.MESH.Client.Contracts.Configurations;
using NHS.MESH.Client.Contracts.Services;
using NHS.MESH.Client.Helpers.AuthHelpers;
using NHS.MESH.Client.Models;
using System.Net;
using System.Text.Json;

namespace NHS.MESH.Client.Services
{
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
        public async Task<HandshakeResponse> MeshHandshakeAsync(string mailboxId)
        {
            // Validations
            if (string.IsNullOrWhiteSpace(mailboxId)) { throw new ArgumentNullException(nameof(mailboxId)); }

            if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiBaseUrl)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiBaseUrl)); }

            // The HTTP Request Message
            var httpRequestMessage = new HttpRequestMessage();

            var meshResponse = new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.InternalServerError, "Handshake failed!");

            // API URL
            var uri = new Uri(_meshConnectConfiguration.MeshApiBaseUrl + "/" + mailboxId);
            httpRequestMessage.RequestUri = uri;

            // Request Method
            httpRequestMessage.Method = HttpMethod.Get;

            // Headers
            var authHeader = MeshAuthorizationHelper.GenerateAuthHeaderValue(mailboxId);
            httpRequestMessage.Headers.Add("authorization", authHeader);
            httpRequestMessage.Headers.Add("accept", "application/vnd.mesh.v2+json");
            httpRequestMessage.Headers.Add("User_Agent", "my-client;windows-10;");
            httpRequestMessage.Headers.Add("Mex-ClientVersion", "ApiDocs==0.0.1");
            httpRequestMessage.Headers.Add("Mex-OSName", "Linux");
            httpRequestMessage.Headers.Add("Mex-OSVersion", "#44~18.04.2-Ubuntu");
            httpRequestMessage.Headers.Add("Mex-JavaVersion", "openjdk-11u");
            httpRequestMessage.Headers.Add("Mex-OSArchitecture", "x86_64");

            meshResponse = await _meshConnectClient.SendRequestAsync(httpRequestMessage);

            if(meshResponse.Key == HttpStatusCode.Forbidden)
            {
                var errorContent = getErrorResponse(meshResponse.Value);
                throw new Exception(errorContent.errorDescription);
            }
            else if(meshResponse.Key == HttpStatusCode.BadRequest)
            {
                var errorContent = getErrorResponse(meshResponse.Value);
                throw new Exception(errorContent.errorDescription);

            }
            else if(meshResponse.Key != HttpStatusCode.OK)
            {
                var errorContent = getErrorResponse(meshResponse.Value);
                throw new Exception(errorContent.errorDescription);
            }
            var handshakeResponse = JsonSerializer.Deserialize<HandshakeResponse>(meshResponse.Value);
            return handshakeResponse;
        }
        private APIErrorResponse getErrorResponse(string content){
            return JsonSerializer.Deserialize<APIErrorResponse>(content);
        }
    }

}
