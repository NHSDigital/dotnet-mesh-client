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

namespace NHS.MESH.Client.Services;

/// <summary>The MESH Outbox service.</summary>
public class MeshOutboxService : IMeshOutboxService
{
    /// <summary>The MESH Connect Configuration.</summary>
    private readonly IMeshConnectConfiguration _meshConnectConfiguration;

    /// <summary>The MESH Connect Client.</summary>
    private readonly IMeshConnectClient _meshConnectClient;


    /// <summary>
    /// Initializes a new instance of the <see cref="MeshOutboxService"/> class.
    /// </summary>
    /// <param name="meshConnectConfiguration">The MESH Connect Configuration.</param>
    /// <param name="meshConnectClient">The MESH Connect Client.</param>
    /// <param name="meshOperationService">The MESH Operation service.</param>
    public MeshOutboxService(IMeshConnectConfiguration meshConnectConfiguration, IMeshConnectClient meshConnectClient)
    {
        _meshConnectConfiguration = meshConnectConfiguration;
        _meshConnectClient = meshConnectClient;
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
    public async Task<MeshResponse<SendMessageResponse>> SendCompressedMessageAsync(string fromMailboxId, string toMailboxId, string workflowId, FileAttachment file, string? localId = null, string? subject = null, bool includeChecksum = false)
    {
        // Validations
        if (string.IsNullOrWhiteSpace(fromMailboxId)) { throw new ArgumentNullException(nameof(fromMailboxId)); }
        if (string.IsNullOrWhiteSpace(toMailboxId)) { throw new ArgumentNullException(nameof(toMailboxId)); }
        if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiBaseUrl)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiBaseUrl)); }
        if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiOutboxUriPath)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiOutboxUriPath)); }
        if (file.Content == null || file.Content.Length == 0) { throw new ArgumentNullException(nameof(file.Content)); }

        var uri = new Uri($"{_meshConnectConfiguration.MeshApiBaseUrl}/{fromMailboxId}/{_meshConnectConfiguration.MeshApiOutboxUriPath}");

        // Body
        var content = await FileHelpers.CompressFileAsync(file.Content);
        var meshResponse = await SendSingleMessage(uri, fromMailboxId, toMailboxId, workflowId, content, file.FileName, localId, subject, includeChecksum);
        return await ResponseHelper.CreateMeshResponse<SendMessageResponse>(meshResponse, async _ => JsonSerializer.Deserialize<SendMessageResponse>(await _.Content.ReadAsStringAsync()));

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
    public async Task<MeshResponse<SendMessageResponse>> SendUnCompressedMessageAsync(string fromMailboxId, string toMailboxId, string workflowId, FileAttachment file, string? localId = null, string? subject = null, bool includeChecksum = false)
    {
        // Validations
        if (string.IsNullOrWhiteSpace(fromMailboxId)) { throw new ArgumentNullException(nameof(fromMailboxId)); }
        if (string.IsNullOrWhiteSpace(toMailboxId)) { throw new ArgumentNullException(nameof(toMailboxId)); }
        if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiBaseUrl)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiBaseUrl)); }
        if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiOutboxUriPath)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiOutboxUriPath)); }
        if (FileHelpers.IsFileTooLarge(file.Content, _meshConnectConfiguration.ChunkSize)) { throw new InvalidDataException("File Size exceeds set MESH limit"); }


        // API URL
        var uri = new Uri($"{_meshConnectConfiguration.MeshApiBaseUrl}/{fromMailboxId}/{_meshConnectConfiguration.MeshApiOutboxUriPath}");

        // Body
        var content = new ByteArrayContent(file.Content)
        {
            Headers =
                {
                    ContentType = new MediaTypeHeaderValue("application/octet-stream")
                }
        };



        var meshResponse = await SendSingleMessage(uri, fromMailboxId, toMailboxId, workflowId, content, file.FileName, localId, subject, includeChecksum);

        return await ResponseHelper.CreateMeshResponse<SendMessageResponse>(meshResponse, async _ => JsonSerializer.Deserialize<SendMessageResponse>(await _.Content.ReadAsStringAsync()));

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
    public async Task<MeshResponse<SendMessageResponse>> SendChunkedMessageAsync(string fromMailboxId, string toMailboxId, string workflowId, FileAttachment file, string? localId = null, string? subject = null, bool includeChecksum = false)
    {
        // Validations
        if (string.IsNullOrWhiteSpace(fromMailboxId)) { throw new ArgumentNullException(nameof(fromMailboxId)); }
        if (string.IsNullOrWhiteSpace(toMailboxId)) { throw new ArgumentNullException(nameof(toMailboxId)); }
        if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiBaseUrl)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiBaseUrl)); }
        if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiOutboxUriPath)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiOutboxUriPath)); }
        ArgumentNullException.ThrowIfNull(file);

        var chunkedFiles = await ContentSplitHelper.SplitFileToMemoryStreams(file.Content, _meshConnectConfiguration.ChunkSize);


        Uri initalMessageURI = new Uri($"{_meshConnectConfiguration.MeshApiBaseUrl}/{fromMailboxId}/{_meshConnectConfiguration.MeshApiOutboxUriPath}");
        var initialChunk = await FileHelpers.CompressFileAsync(chunkedFiles[0]);
        var httpResponseMessage = await SendMessageChunk(initalMessageURI, fromMailboxId, toMailboxId, workflowId, initialChunk, file.FileName, 1, chunkedFiles.Count, localId, subject, includeChecksum);

        var meshResponse = await ResponseHelper.CreateMeshResponse<SendMessageResponse>(httpResponseMessage, async _ => JsonSerializer.Deserialize<SendMessageResponse>(await _.Content.ReadAsStringAsync()));

        if (!meshResponse.IsSuccessful)
        {
            return meshResponse;
        }

        var messageId = meshResponse.Response.MessageId;

        for (var i = 1; i < chunkedFiles.Count; i++)
        {
            Uri chunkMessageURI = new Uri($"{_meshConnectConfiguration.MeshApiBaseUrl}/{fromMailboxId}/{_meshConnectConfiguration.MeshApiOutboxUriPath}/{messageId}/{i + 1}");
            var chunk = await FileHelpers.CompressFileAsync(chunkedFiles[i]);
            var chunkMeshResponse = await SendMessageChunk(chunkMessageURI, fromMailboxId, toMailboxId, workflowId, chunk, file.FileName, i + 1, chunkedFiles.Count, localId, subject, includeChecksum);
            meshResponse = await ResponseHelper.CreateMeshResponse<SendMessageResponse>(chunkMeshResponse, async _ => JsonSerializer.Deserialize<SendMessageResponse>(await _.Content.ReadAsStringAsync()));

            if (!meshResponse.IsSuccessful)
            {
                return meshResponse;
            }

        }

        return meshResponse;

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

        return await ResponseHelper.CreateMeshResponse<TrackOutboxResponse>(meshResponse, async _ => JsonSerializer.Deserialize<TrackOutboxResponse>(await _.Content.ReadAsStringAsync()));
    }



    private async Task<HttpResponseMessage> SendSingleMessage(Uri uri, string fromMailboxId, string toMailboxId, string workflowId, HttpContent content, string fileName, string? localId = null, string? subject = null, bool includeChecksum = false)
    {
        var httpRequestMessage = await BuildMessage(uri, fromMailboxId, toMailboxId, workflowId, content, fileName, localId, subject, includeChecksum);
        var meshResponse = await _meshConnectClient.SendRequestAsync(httpRequestMessage);

        return meshResponse;

    }

    private async Task<HttpResponseMessage> SendMessageChunk(Uri uri, string fromMailboxId, string toMailboxId, string workflowId, HttpContent content, string fileName, int chunkNumber, int chunkLength, string? localId = null, string? subject = null, bool includeChecksum = false)
    {
        var httpRequestMessage = await BuildMessage(uri, fromMailboxId, toMailboxId, workflowId, content, fileName, localId, subject, includeChecksum);
        httpRequestMessage.Headers.Add("mex-chunk-range", FileHelpers.CreateChunkRange(chunkNumber, chunkLength));
        var meshResponse = await _meshConnectClient.SendRequestAsync(httpRequestMessage);
        return meshResponse;

    }

    private static async Task<HttpRequestMessage> BuildMessage(Uri uri, string fromMailboxId, string toMailboxId, string workflowId, HttpContent content, string fileName, string? localId = null, string? subject = null, bool includeChecksum = false)
    {
        var httpRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = uri
        };

        var authHeader = MeshAuthorizationHelper.GenerateAuthHeaderValue(fromMailboxId);
        httpRequestMessage.Headers.Add("authorization", authHeader);
        httpRequestMessage.Headers.Add("accept", "application/vnd.mesh.v2+json");
        httpRequestMessage.Headers.Add("mex-from", fromMailboxId);
        httpRequestMessage.Headers.Add("mex-to", toMailboxId);
        httpRequestMessage.Headers.Add("mex-workflowid", workflowId);
        httpRequestMessage.Headers.Add("mex-filename", fileName);
        httpRequestMessage.Headers.Add("mex-localid", localId);
        httpRequestMessage.Headers.Add("Mex-Subject", subject);
        if (includeChecksum)
        {
            httpRequestMessage.Headers.Add("Mex-Checksum", FileHelpers.GenerateChecksum(await content.ReadAsByteArrayAsync()));
        }

        httpRequestMessage.Content = content;

        return httpRequestMessage;
    }
}
