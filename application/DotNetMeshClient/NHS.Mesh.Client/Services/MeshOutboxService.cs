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
using NHS.MESH.Client.Helpers.ContentHelpers;
using NHS.MESH.Client.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using NHS.MESH.Client.Helpers;

namespace NHS.MESH.Client.Services
{
    /// <summary>The MESH Outbox service.</summary>
    public class MeshOutboxService : IMeshOutboxService
    {
        /// <summary>The MESH Connect Configuration.</summary>
        private readonly IMeshConnectConfiguration _meshConnectConfiguration;

        /// <summary>The MESH Connect Client.</summary>
        private readonly IMeshConnectClient _meshConnectClient;

        /// <summary>The MESH Operation service.</summary>
        private readonly IMeshOperationService _meshOperationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeshOutboxService"/> class.
        /// </summary>
        /// <param name="meshConnectConfiguration">The MESH Connect Configuration.</param>
        /// <param name="meshConnectClient">The MESH Connect Client.</param>
        /// <param name="meshOperationService">The MESH Operation service.</param>
        public MeshOutboxService(IMeshConnectConfiguration meshConnectConfiguration, IMeshConnectClient meshConnectClient, IMeshOperationService meshOperationService)
        {
            _meshConnectConfiguration = meshConnectConfiguration;
            _meshConnectClient = meshConnectClient;
            _meshOperationService = meshOperationService;
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
        public async Task<MeshResponse<SendMessageResponse>> SendCompressedMessageAsync(string fromMailboxId, string toMailboxId,string workflowId, FileAttachment file)
        {
            // Validations
            if (string.IsNullOrWhiteSpace(fromMailboxId)) { throw new ArgumentNullException(nameof(fromMailboxId)); }
            if (string.IsNullOrWhiteSpace(toMailboxId)) { throw new ArgumentNullException(nameof(toMailboxId)); }
            if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiBaseUrl)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiBaseUrl)); }
            if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiOutboxUriPath)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiOutboxUriPath)); }

            // The HTTP Request Message
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post
            };

            // API URL
            var uri = new Uri($"{_meshConnectConfiguration.MeshApiBaseUrl}/{fromMailboxId}/{_meshConnectConfiguration.MeshApiOutboxUriPath}");
            httpRequestMessage.RequestUri = uri;

            // Headers
            var authHeader = MeshAuthorizationHelper.GenerateAuthHeaderValue(fromMailboxId);
            httpRequestMessage.Headers.Add("authorization", authHeader);
            httpRequestMessage.Headers.Add("accept", "application/vnd.mesh.v2+json");
            httpRequestMessage.Headers.Add("mex-from", fromMailboxId);
            httpRequestMessage.Headers.Add("mex-to", toMailboxId);
            httpRequestMessage.Headers.Add("mex-workflowid", workflowId);
            httpRequestMessage.Headers.Add("mex-filename", file.FileName);
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
                    using (var fileStream = new MemoryStream(file.Content))
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

                var meshResponse = await _meshConnectClient.SendRequestAsync(httpRequestMessage);

                return await ResponseHelper.CreateMeshResponse<SendMessageResponse>(meshResponse,async _ => JsonSerializer.Deserialize<SendMessageResponse>(await _.Content.ReadAsStringAsync()));
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
        public async Task<MeshResponse<SendMessageResponse>> SendUnCompressedMessageAsync(string fromMailboxId, string toMailboxId, FileAttachment file)
        {
            // Validations
            if (string.IsNullOrWhiteSpace(fromMailboxId)) { throw new ArgumentNullException(nameof(fromMailboxId)); }
            if (string.IsNullOrWhiteSpace(toMailboxId)) { throw new ArgumentNullException(nameof(toMailboxId)); }
            if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiBaseUrl)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiBaseUrl)); }
            if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiOutboxUriPath)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiOutboxUriPath)); }

            // The HTTP Request Message
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post
            };

            // API URL
            var uri = new Uri($"{_meshConnectConfiguration.MeshApiBaseUrl}/{fromMailboxId}/{_meshConnectConfiguration.MeshApiOutboxUriPath}");
            httpRequestMessage.RequestUri = uri;

            // Headers
            var authHeader = MeshAuthorizationHelper.GenerateAuthHeaderValue(fromMailboxId);
            httpRequestMessage.Headers.Add("authorization", authHeader);
            httpRequestMessage.Headers.Add("accept", "application/vnd.mesh.v2+json");
            httpRequestMessage.Headers.Add("mex-from", fromMailboxId);
            httpRequestMessage.Headers.Add("mex-to", toMailboxId);
            httpRequestMessage.Headers.Add("mex-workflowid", "API-DOCS-TEST");
            httpRequestMessage.Headers.Add("mex-filename", file.FileName);
            httpRequestMessage.Headers.Add("mex-localid", "api-docs-bob-greets-alice");
            httpRequestMessage.Headers.Add("Mex-Content-Compressed", "N");
            httpRequestMessage.Headers.Add("Mex-Content-Encrypted", "N");
            httpRequestMessage.Headers.Add("Mex-Content-Compress", "N");
            httpRequestMessage.Headers.Add("Mex-Subject", "Test");
            httpRequestMessage.Headers.Add("Mex-Checksum", "Test");
            httpRequestMessage.Headers.Add("Mex-MessageType", "Data");

            // Body

                var content = new ByteArrayContent(file.Content)
                {
                    Headers =
                      {
                          ContentType = new MediaTypeHeaderValue("application/octet-stream")
                      }
                };

                httpRequestMessage.Content = content;

                var meshResponse = await _meshConnectClient.SendRequestAsync(httpRequestMessage);



                return await ResponseHelper.CreateMeshResponse<SendMessageResponse>(meshResponse,async _ => JsonSerializer.Deserialize<SendMessageResponse>(await _.Content.ReadAsStringAsync()));

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
        public async Task<MeshResponse<SendMessageResponse>> SendChunkedMessageAsync(string fromMailboxId, string toMailboxId, FileAttachment file)
        {
            // Validations
            if (string.IsNullOrWhiteSpace(fromMailboxId)) { throw new ArgumentNullException(nameof(fromMailboxId)); }
            if (string.IsNullOrWhiteSpace(toMailboxId)) { throw new ArgumentNullException(nameof(toMailboxId)); }
            if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiBaseUrl)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiBaseUrl)); }
            if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiOutboxUriPath)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiOutboxUriPath)); }

            var chunkedFiles = await ContentSplitHelper.SplitFileToMemoryStreams(new MemoryStream(file.Content),_meshConnectConfiguration.ChunkSize);
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
                    var uri = new Uri(_meshConnectConfiguration.MeshApiBaseUrl + "/" + fromMailboxId + "/" + _meshConnectConfiguration.MeshApiOutboxUriPath);
                    httpRequestMessage.RequestUri = uri;

                    var meshResponse = await _meshConnectClient.SendRequestAsync(httpRequestMessage);

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
                    var uri = new Uri(_meshConnectConfiguration.MeshApiBaseUrl + "/" + fromMailboxId + "/" + _meshConnectConfiguration.MeshApiOutboxUriPath + "/" + sendMessageResponse?.MessageId + "/" + (i + 1).ToString());
                    httpRequestMessage.RequestUri = uri;

                    var meshResponse = await _meshConnectClient.SendRequestAsync(httpRequestMessage);

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
        public async Task<MeshResponse<TrackOutboxResponse>> TrackMessageByIdAsync(string mailboxId, string messageId)
        {
            // Validations
            if (string.IsNullOrWhiteSpace(mailboxId)) { throw new ArgumentNullException(nameof(mailboxId)); }
            if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiBaseUrl)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiBaseUrl)); }
            if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiOutboxUriPath)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiOutboxUriPath)); }
            if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiTrackMessageUriPath)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiTrackMessageUriPath)); }

            // The HTTP Request Message
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get
            };

            // API URL
            var uri = new Uri($"{_meshConnectConfiguration.MeshApiBaseUrl}/{mailboxId}/{_meshConnectConfiguration.MeshApiOutboxUriPath}/{_meshConnectConfiguration.MeshApiTrackMessageUriPath}?={messageId}");
            httpRequestMessage.RequestUri = uri;

            // Headers
            var authHeader = MeshAuthorizationHelper.GenerateAuthHeaderValue(mailboxId);
            httpRequestMessage.Headers.Add("authorization", authHeader);
            httpRequestMessage.Headers.Add("accept", "application/vnd.mesh.v2+json");

            // Get Messages
            var meshResponse = await _meshConnectClient.SendRequestAsync(httpRequestMessage);

            return await ResponseHelper.CreateMeshResponse<TrackOutboxResponse>(meshResponse,async _ => JsonSerializer.Deserialize<TrackOutboxResponse>(await _.Content.ReadAsStringAsync()));
        }

        private async Task<HttpResponseMessage> SendMessage(){

        }
    }
}
