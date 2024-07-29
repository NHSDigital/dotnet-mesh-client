// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MyClass.cs" company="NHS">
// Copyright (c) NHS. All rights reserved.
// Year: 2024
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using NHS.MESH.Client.Contracts.Clients;
using NHS.MESH.Client.Contracts.Configurations;
using NHS.MESH.Client.Contracts.Services;
using NHS.MESH.Client.Helpers.AuthHelpers;
using System.Net;

namespace NHS.MESH.Client.Services
{
    /// <summary>The MESH Inbox service.</summary>
    public class MeshInboxService : IMeshInboxService
    {
        /// <summary>The MESH Connect Configuration.</summary>
        private readonly IMeshConnectConfiguration _meshConnectConfiguration;

        /// <summary>The MESH Connect Client.</summary>
        private readonly IMeshConnectClient _meshConnectClient;

        /// <summary>The MESH Operation service.</summary>
        private readonly IMeshOperationService _meshOperationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeshInboxService"/> class.
        /// </summary>
        /// <param name="meshConnectConfiguration">The MESH Connect Configuration.</param>
        /// <param name="meshConnectClient">The MESH Connect Client.</param>
        /// <param name="meshOperationService">The MESH Operation service.</param>
        public MeshInboxService(IMeshConnectConfiguration meshConnectConfiguration, IMeshConnectClient meshConnectClient, IMeshOperationService meshOperationService)
        {
            _meshConnectConfiguration = meshConnectConfiguration;
            _meshConnectClient = meshConnectClient;
            _meshOperationService = meshOperationService;
        }

        /// <summary>
        /// Get messages from MESH Inbox asynchronously.
        /// </summary>
        /// <param name="mailboxId">The Mailbox Id.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The Arugument Null Exception.</exception>
        /// <exception cref="Exception">The general Exception.</exception>
        public async Task<KeyValuePair<HttpStatusCode, string>> GetMessagesAsync(string mailboxId)
        {
            try
            {
                // Validations
                if (string.IsNullOrWhiteSpace(mailboxId)) { throw new ArgumentNullException(nameof(mailboxId)); }

                if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiBaseUrl)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiBaseUrl)); }

                if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiInboxUriPath)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiInboxUriPath)); }

                // The HTTP Request Message
                var httpRequestMessage = new HttpRequestMessage();

                // API URL
                var uri = new Uri(_meshConnectConfiguration.MeshApiBaseUrl + "/" + mailboxId + "/" + _meshConnectConfiguration.MeshApiInboxUriPath);
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
                httpRequestMessage.Headers.Add("Mex-OSArchitecture", "x86_64");

                // Get Messages
                var meshResponse = await _meshConnectClient.SendRequestAsync(httpRequestMessage);

                return new KeyValuePair<HttpStatusCode, string>(meshResponse.Key, meshResponse.Value);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// Get message by message Id from MESH Inbox asynchronously.
        /// </summary>
        /// <param name="mailboxId">The Mailbox Id.</param>
        /// <param name="messageId">The Message Id.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The Arugument Null Exception.</exception>
        /// <exception cref="Exception">The general Exception.</exception>
        public async Task<KeyValuePair<HttpStatusCode, string>> GetMessageByIdAsync(string mailboxId, string messageId)
        {
            try
            {
                // Validations
                if (string.IsNullOrWhiteSpace(mailboxId)) { throw new ArgumentNullException(nameof(mailboxId)); }
                if (string.IsNullOrWhiteSpace(messageId)) { throw new ArgumentNullException(nameof(messageId)); }
                if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiBaseUrl)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiBaseUrl)); }
                if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiInboxUriPath)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiInboxUriPath)); }

                // The HTTP Request Message
                var httpRequestMessage = new HttpRequestMessage();

                // API URL
                var uri = new Uri(_meshConnectConfiguration.MeshApiBaseUrl + "/" + mailboxId + "/" + _meshConnectConfiguration.MeshApiInboxUriPath + "/" + messageId);
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


                // Get Messages
                var meshResponse = await _meshConnectClient.SendRequestAsync(httpRequestMessage);

                return new KeyValuePair<HttpStatusCode, string>(meshResponse.Key, meshResponse.Value);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// Get message meta data by message Id from MESH Inbox asynchronously.
        /// </summary>
        /// <param name="mailboxId">The Mailbox Id.</param>
        /// <param name="messageId">The Message Id.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The Arugument Null Exception.</exception>
        /// <exception cref="Exception">The general Exception.</exception>
        public async Task<KeyValuePair<HttpStatusCode, string>> GetHeadMessageByIdAsync(string mailboxId, string messageId)
        {
            try
            {
                // Validations
                if (string.IsNullOrWhiteSpace(mailboxId)) { throw new ArgumentNullException(nameof(mailboxId)); }

                if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiBaseUrl)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiBaseUrl)); }

                if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiInboxUriPath)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiInboxUriPath)); }


                // The HTTP Request Message
                var httpRequestMessage = new HttpRequestMessage();

                // API URL
                var uri = new Uri(_meshConnectConfiguration.MeshApiBaseUrl + "/" + mailboxId + "/" + _meshConnectConfiguration.MeshApiInboxUriPath + "/" + messageId);
                httpRequestMessage.RequestUri = uri;

                // Request Method
                httpRequestMessage.Method = HttpMethod.Head;

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

                // Get Messages
                var meshResponse = await _meshConnectClient.SendRequestAsync(httpRequestMessage);

                // Acknowledge Message
                var acknowledgeMessage = await AcknowledgeMessageByIdAsync(mailboxId, messageId);

                return new KeyValuePair<HttpStatusCode, string>(acknowledgeMessage.Key, acknowledgeMessage.Value);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// Acknowledge sent message by message Id from MESH Inbox asynchronously.
        /// </summary>
        /// <param name="mailboxId">The Mailbox Id.</param>
        /// <param name="messageId">The Message Id.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The Arugument Null Exception.</exception>
        /// <exception cref="Exception">The general Exception.</exception>
        public async Task<KeyValuePair<HttpStatusCode, string>> AcknowledgeMessageByIdAsync(string mailboxId, string messageId)
        {
            try
            {
                // Validations
                if (string.IsNullOrWhiteSpace(mailboxId)) { throw new ArgumentNullException(nameof(mailboxId)); }

                if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiBaseUrl)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiBaseUrl)); }

                if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiInboxUriPath)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiInboxUriPath)); }


                // The HTTP Request Message
                var httpRequestMessage = new HttpRequestMessage();

                // API URL
                var uri = new Uri(_meshConnectConfiguration.MeshApiBaseUrl + "/" + mailboxId + "/" + _meshConnectConfiguration.MeshApiInboxUriPath + "/" + messageId + "/" + _meshConnectConfiguration.MeshApiAcknowledgeUriPath);
                httpRequestMessage.RequestUri = uri;

                // Request Method
                httpRequestMessage.Method = HttpMethod.Put;

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

                // Get Messages
                var meshResponse = await _meshConnectClient.SendRequestAsync(httpRequestMessage);

                return new KeyValuePair<HttpStatusCode, string>(meshResponse.Key, meshResponse.Value);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
