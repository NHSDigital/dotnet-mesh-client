// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeshOutboxService.cs" company="NHS">
// Copyright (c) NHS. All rights reserved.
// Year: 2024
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using NHS.MESH.Client.Contracts.Clients;
using NHS.MESH.Client.Contracts.Configurations;
using NHS.MESH.Client.Contracts.Services;
using NHS.MESH.Client.Helpers.AuthHelpers;
using NHS.MESH.Client.Helpers.ConetentHelpers;
using NHS.MESH.Client.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace NHS.MESH.Client.Services
{
    /// <summary>The MESH Outbox service.</summary>
    public class MeshOutboxService : IMeshOutboxService
    {
        /// <summary>The MESH Connect Configuration.</summary>
        private readonly IMeshConnectConfiguration meshConnectConfiguration;

        /// <summary>The MESH Connect Client.</summary>
        private readonly IMeshConnectClient meshConnectClient;

        /// <summary>The MESH Operation service.</summary>
        private readonly IMeshOperationService meshOperationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeshOutboxService"/> class.
        /// </summary>
        /// <param name="meshConnectConfiguration">The MESH Connect Configuration.</param>
        /// <param name="meshConnectClient">The MESH Connect Client.</param>
        /// <param name="meshOperationService">The MESH Operation service.</param>
        public MeshOutboxService(IMeshConnectConfiguration meshConnectConfiguration, IMeshConnectClient meshConnectClient, IMeshOperationService meshOperationService)
        {
            this.meshConnectConfiguration = meshConnectConfiguration;
            this.meshConnectClient = meshConnectClient;
            this.meshOperationService = meshOperationService;
        }

        /// <summary>
        /// Send messages asynchronously to the MESH API.
        /// </summary>
        /// <param name="fromMailboxId">The Mailbox Id.</param>
        /// <param name="toMailboxId">The Mailbox Id.</param>
        /// <param name="file">The request content file.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The Arugument Null Exception.</exception>
        /// <exception cref="Exception">The general Exception.</exception>
        public async Task<KeyValuePair<HttpStatusCode, string>> SendCompressedMessageAsync(string fromMailboxId, string toMailboxId, IFormFile file)
        {
            // Validations
            if (string.IsNullOrWhiteSpace(fromMailboxId)) { throw new ArgumentNullException(nameof(fromMailboxId)); }
            if (string.IsNullOrWhiteSpace(toMailboxId)) { throw new ArgumentNullException(nameof(toMailboxId)); }
            if (string.IsNullOrWhiteSpace(this.meshConnectConfiguration.MeshApiBaseUrl)) { throw new ArgumentNullException(nameof(this.meshConnectConfiguration.MeshApiBaseUrl)); }
            if (string.IsNullOrWhiteSpace(this.meshConnectConfiguration.MeshApiOutboxUriPath)) { throw new ArgumentNullException(nameof(this.meshConnectConfiguration.MeshApiOutboxUriPath)); }

            var handshake = await this.meshOperationService.MeshHandsahkeAsync(fromMailboxId);
            if (handshake.Key != HttpStatusCode.OK) { return handshake; }

            // The HTTP Request Message
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post
            };

            // API URL
            var uri = new Uri($"{this.meshConnectConfiguration.MeshApiBaseUrl}/{fromMailboxId}/{this.meshConnectConfiguration.MeshApiOutboxUriPath}");
            httpRequestMessage.RequestUri = uri;

            // Headers
            var authHeader = MeshAuthorizationHelper.GenerateAuthHeaderValue(fromMailboxId);
            httpRequestMessage.Headers.Add("authorization", authHeader);
            httpRequestMessage.Headers.Add("accept", "application/vnd.mesh.v2+json");
            httpRequestMessage.Headers.Add("Mex-ClientVersion", "ApiDocs==0.0.1");
            httpRequestMessage.Headers.Add("Mex-OSName", "Linux");
            httpRequestMessage.Headers.Add("Mex-OSVersion", "#44~18.04.2-Ubuntu");
            httpRequestMessage.Headers.Add("Mex-JavaVersion", "openjdk-11u");
            httpRequestMessage.Headers.Add("Mex-OSArchitecture", "x86_64");
            httpRequestMessage.Headers.Add("mex-from", fromMailboxId);
            httpRequestMessage.Headers.Add("mex-to", toMailboxId);
            httpRequestMessage.Headers.Add("mex-workflowid", "API-DOCS-TEST");
            httpRequestMessage.Headers.Add("mex-filename", "none");
            httpRequestMessage.Headers.Add("mex-localid", "api-docs-bob-greets-alice");
            httpRequestMessage.Headers.Add("Mex-Content-Compressed", "Y");
            httpRequestMessage.Headers.Add("Mex-Content-Encrypted", "N");
            httpRequestMessage.Headers.Add("Mex-Content-Compress", "Y");
            httpRequestMessage.Headers.Add("Mex-Subject", "Test");
            httpRequestMessage.Headers.Add("Mex-Checksum", "Test");
            httpRequestMessage.Headers.Add("Mex-MessageType", "Data");

            // Body
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    using (var fileStream = file.OpenReadStream())
                    {
                        await fileStream.CopyToAsync(gzipStream);
                    }
                }

                memoryStream.Position = 0;

                var content = new StreamContent(memoryStream)
                {
                    Headers =
            {
                ContentType = new MediaTypeHeaderValue("application/octet-stream"),
                ContentEncoding = { "gzip" }
            }
                };

                httpRequestMessage.Content = content;

                var meshResponse = await this.meshConnectClient.SendRequestAsync(httpRequestMessage);

                return meshResponse;
            }
        }


        /// <summary>
        /// Send messages asynchronously to the MESH API.
        /// </summary>
        /// <param name="fromMailboxId">The Mailbox Id.</param>
        /// <param name="toMailboxId">The Mailbox Id.</param>
        /// <param name="file">The request content file.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The Arugument Null Exception.</exception>
        /// <exception cref="Exception">The general Exception.</exception>
        public async Task<KeyValuePair<HttpStatusCode, string>> SendUnCompressedMessageAsync(string fromMailboxId, string toMailboxId, IFormFile file)
        {
            // Validations
            if (string.IsNullOrWhiteSpace(fromMailboxId)) { throw new ArgumentNullException(nameof(fromMailboxId)); }
            if (string.IsNullOrWhiteSpace(toMailboxId)) { throw new ArgumentNullException(nameof(toMailboxId)); }
            if (string.IsNullOrWhiteSpace(this.meshConnectConfiguration.MeshApiBaseUrl)) { throw new ArgumentNullException(nameof(this.meshConnectConfiguration.MeshApiBaseUrl)); }
            if (string.IsNullOrWhiteSpace(this.meshConnectConfiguration.MeshApiOutboxUriPath)) { throw new ArgumentNullException(nameof(this.meshConnectConfiguration.MeshApiOutboxUriPath)); }

            var handshake = await this.meshOperationService.MeshHandsahkeAsync(fromMailboxId);
            if (handshake.Key != HttpStatusCode.OK) { return handshake; }

            // The HTTP Request Message
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post
            };

            // API URL
            var uri = new Uri($"{this.meshConnectConfiguration.MeshApiBaseUrl}/{fromMailboxId}/{this.meshConnectConfiguration.MeshApiOutboxUriPath}");
            httpRequestMessage.RequestUri = uri;

            // Headers
            var authHeader = MeshAuthorizationHelper.GenerateAuthHeaderValue(fromMailboxId);
            httpRequestMessage.Headers.Add("authorization", authHeader);
            httpRequestMessage.Headers.Add("accept", "application/vnd.mesh.v2+json");
            httpRequestMessage.Headers.Add("Mex-ClientVersion", "ApiDocs==0.0.1");
            httpRequestMessage.Headers.Add("Mex-OSName", "Linux");
            httpRequestMessage.Headers.Add("Mex-OSVersion", "#44~18.04.2-Ubuntu");
            httpRequestMessage.Headers.Add("Mex-JavaVersion", "openjdk-11u");
            httpRequestMessage.Headers.Add("Mex-OSArchitecture", "x86_64");
            httpRequestMessage.Headers.Add("mex-from", fromMailboxId);
            httpRequestMessage.Headers.Add("mex-to", toMailboxId);
            httpRequestMessage.Headers.Add("mex-workflowid", "API-DOCS-TEST");
            httpRequestMessage.Headers.Add("mex-filename", "none");
            httpRequestMessage.Headers.Add("mex-localid", "api-docs-bob-greets-alice");
            httpRequestMessage.Headers.Add("Mex-Content-Compressed", "N");
            httpRequestMessage.Headers.Add("Mex-Content-Encrypted", "N");
            httpRequestMessage.Headers.Add("Mex-Content-Compress", "N");
            httpRequestMessage.Headers.Add("Mex-Subject", "Test");
            httpRequestMessage.Headers.Add("Mex-Checksum", "Test");
            httpRequestMessage.Headers.Add("Mex-MessageType", "Data");

            // Body
            using (var fileStream = file.OpenReadStream())
            {
                var content = new StreamContent(fileStream)
                {
                    Headers =
            {
                ContentType = new MediaTypeHeaderValue("application/octet-stream")
            }
                };

                httpRequestMessage.Content = content;

                var meshResponse = await this.meshConnectClient.SendRequestAsync(httpRequestMessage);

                return meshResponse;
            }
        }


        /// <summary>
        /// Send chunked messages asynchronously to the MESH API.
        /// </summary>
        /// <param name="fromMailboxId">The Mailbox Id.</param>
        /// <param name="toMailboxId">The Mailbox Id.</param>
        /// <param name="file">The request content file.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The Arugument Null Exception.</exception>
        /// <exception cref="Exception">The general Exception.</exception>
        public async Task<KeyValuePair<HttpStatusCode, string>> SendChunkedMessageAsync(string fromMailboxId, string toMailboxId, IFormFile file)
        {
            // Validations
            if (string.IsNullOrWhiteSpace(fromMailboxId)) { throw new ArgumentNullException(nameof(fromMailboxId)); }
            if (string.IsNullOrWhiteSpace(toMailboxId)) { throw new ArgumentNullException(nameof(toMailboxId)); }
            if (string.IsNullOrWhiteSpace(this.meshConnectConfiguration.MeshApiBaseUrl)) { throw new ArgumentNullException(nameof(this.meshConnectConfiguration.MeshApiBaseUrl)); }
            if (string.IsNullOrWhiteSpace(this.meshConnectConfiguration.MeshApiOutboxUriPath)) { throw new ArgumentNullException(nameof(this.meshConnectConfiguration.MeshApiOutboxUriPath)); }

            var handshake = await this.meshOperationService.MeshHandsahkeAsync(fromMailboxId);
            if (handshake.Key != HttpStatusCode.OK) { return handshake; }

            var chunkedFiles = await ContentSplitHelper.SplitFileToMemoryStreams(file);
            var sendMessageResponse = new SendMessageResponse();
            var responseKey = HttpStatusCode.InternalServerError;

            for (var i = 0; i < chunkedFiles.Count; i++)
            {
                // The HTTP Request Message
                var httpRequestMessage = new HttpRequestMessage();

                // Request Method
                httpRequestMessage.Method = HttpMethod.Post;

                // Headers
                var authHeader = MeshAuthorizationHelper.GenerateAuthHeaderValue(fromMailboxId);
                httpRequestMessage.Headers.Add("authorization", authHeader);

                // General Headers
                httpRequestMessage.Headers.Add("accept", "application/vnd.mesh.v2+json");
                httpRequestMessage.Headers.Add("Mex-ClientVersion", "ApiDocs==0.0.1");
                httpRequestMessage.Headers.Add("Mex-OSName", "Linux");
                httpRequestMessage.Headers.Add("Mex-OSVersion", "#44~18.04.2-Ubuntu");
                httpRequestMessage.Headers.Add("Mex-JavaVersion", "openjdk-11u");
                httpRequestMessage.Headers.Add("Mex-OSArchitecture", "x86_64");
                // Send Message Headers
                httpRequestMessage.Headers.Add("mex-from", fromMailboxId);
                httpRequestMessage.Headers.Add("mex-to", toMailboxId);
                httpRequestMessage.Headers.Add("mex-workflowid", "API-DOCS-TEST");
                httpRequestMessage.Headers.Add("mex-filename", "none");
                httpRequestMessage.Headers.Add("mex-localid", "api-docs-bob-greets-alice");
                httpRequestMessage.Headers.Add("Mex-Content-Compressed", "Y");
                httpRequestMessage.Headers.Add("Mex-Content-Encrypted", "N");
                httpRequestMessage.Headers.Add("Mex-Content-Compress", "Y");
                httpRequestMessage.Headers.Add("Mex-Subject", "Test");
                httpRequestMessage.Headers.Add("Mex-Checksum", "Test");
                httpRequestMessage.Headers.Add("Mex-MessageType", "Data");
                httpRequestMessage.Headers.Add("mex-chunk-range", (i + 1).ToString() + ":" + chunkedFiles.Count.ToString());

                // Body
                var content = new StreamContent(chunkedFiles[i]);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                content.Headers.ContentEncoding.Add("gzip");

                httpRequestMessage.Content = content;

                if (i == 0)
                {
                    // API URL
                    var uri = new Uri(this.meshConnectConfiguration.MeshApiBaseUrl + "/" + fromMailboxId + "/" + this.meshConnectConfiguration.MeshApiOutboxUriPath);
                    httpRequestMessage.RequestUri = uri;

                    var meshResponse = await this.meshConnectClient.SendRequestAsync(httpRequestMessage);

                    if (meshResponse.Key == HttpStatusCode.Accepted || meshResponse.Key == HttpStatusCode.OK)
                    {
                        responseKey = meshResponse.Key;
                        sendMessageResponse = JsonSerializer.Deserialize<SendMessageResponse>(meshResponse.Value);
                    }
                    else
                    {
                        return new KeyValuePair<HttpStatusCode, string>(meshResponse.Key, meshResponse.Value);
                    }
                }
                else
                {
                    // API URL
                    var uri = new Uri(this.meshConnectConfiguration.MeshApiBaseUrl + "/" + fromMailboxId + "/" + this.meshConnectConfiguration.MeshApiOutboxUriPath + "/" + sendMessageResponse?.MessageId + "/" + (i + 1).ToString());
                    httpRequestMessage.RequestUri = uri;

                    var meshResponse = await this.meshConnectClient.SendRequestAsync(httpRequestMessage);

                    if (meshResponse.Key != HttpStatusCode.OK)
                    {
                        return new KeyValuePair<HttpStatusCode, string>(meshResponse.Key, meshResponse.Value);
                    }
                }
            }

            var response = JsonSerializer.Serialize<SendMessageResponse>(sendMessageResponse);

            return new KeyValuePair<HttpStatusCode, string>(responseKey, response);
        }


        /// <summary>
        /// Get message status by message Id from MESH Inbox asynchronously.
        /// </summary>
        /// <param name="mailboxId">The Mailbox Id.</param>
        /// <param name="messageId">The Message Id.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The Arugument Null Exception.</exception>
        /// <exception cref="Exception">The general Exception.</exception>
        public async Task<KeyValuePair<HttpStatusCode, string>> TrackMessageByIdAsync(string mailboxId, string messageId)
        {
            // Validations
            if (string.IsNullOrWhiteSpace(mailboxId)) { throw new ArgumentNullException(nameof(mailboxId)); }
            if (string.IsNullOrWhiteSpace(this.meshConnectConfiguration.MeshApiBaseUrl)) { throw new ArgumentNullException(nameof(this.meshConnectConfiguration.MeshApiBaseUrl)); }
            if (string.IsNullOrWhiteSpace(this.meshConnectConfiguration.MeshApiOutboxUriPath)) { throw new ArgumentNullException(nameof(this.meshConnectConfiguration.MeshApiOutboxUriPath)); }
            if (string.IsNullOrWhiteSpace(this.meshConnectConfiguration.MeshApiTrackMessageUriPath)) { throw new ArgumentNullException(nameof(this.meshConnectConfiguration.MeshApiTrackMessageUriPath)); }

            var handshake = await this.meshOperationService.MeshHandsahkeAsync(mailboxId);
            if (handshake.Key != HttpStatusCode.OK) { return handshake; }

            // The HTTP Request Message
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get
            };

            // API URL
            var uri = new Uri($"{this.meshConnectConfiguration.MeshApiBaseUrl}/{mailboxId}/{this.meshConnectConfiguration.MeshApiOutboxUriPath}/{this.meshConnectConfiguration.MeshApiTrackMessageUriPath}/{messageId}");
            httpRequestMessage.RequestUri = uri;

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
            var meshResponse = await this.meshConnectClient.SendRequestAsync(httpRequestMessage);

            return new KeyValuePair<HttpStatusCode, string>(meshResponse.Key, meshResponse.Value);
        }
    }
}
