// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MyClass.cs" company="NHS">
// Copyright (c) NHS. All rights reserved.
// Year: 2024
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NHS.MESH.Client.Services;
using NHS.MESH.Client.Helpers;
using NHS.MESH.Client.Models;
using NHS.MESH.Client.Contracts.Clients;
using NHS.MESH.Client.Contracts.Configurations;
using NHS.MESH.Client.Contracts.Services;
using NHS.MESH.Client.Helpers.AuthHelpers;
using System.Text.Json;
using System.ComponentModel;
using System.Net;

/// <summary>The MESH Inbox service.</summary>
public class MeshInboxService : IMeshInboxService
{
    /// <summary>The MESH Connect Configuration.</summary>
    private readonly IMeshConnectConfiguration _meshConnectConfiguration;

    /// <summary>The MESH Connect Client.</summary>
    private readonly IMeshConnectClient _meshConnectClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="MeshInboxService"/> class.
    /// </summary>
    /// <param name="meshConnectConfiguration">The MESH Connect Configuration.</param>
    /// <param name="meshConnectClient">The MESH Connect Client.</param>
    /// <param name="meshOperationService">The MESH Operation service.</param>
    public MeshInboxService(IMeshConnectConfiguration meshConnectConfiguration, IMeshConnectClient meshConnectClient)
    {
        _meshConnectConfiguration = meshConnectConfiguration;
        _meshConnectClient = meshConnectClient;
    }

    /// <summary>
    /// Get messages from MESH Inbox asynchronously.
    /// </summary>
    /// <param name="mailboxId">The Mailbox Id.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">The Arugument Null Exception.</exception>
    /// <exception cref="Exception">The general Exception.</exception>
    public async Task<MeshResponse<CheckInboxResponse>> GetMessagesAsync(string mailboxId)
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

        // Get Messages
        var meshResponse = await _meshConnectClient.SendRequestAsync(httpRequestMessage);

        return await ResponseHelper.CreateMeshResponse<CheckInboxResponse>(meshResponse, async _ => JsonSerializer.Deserialize<CheckInboxResponse>(await _.Content.ReadAsStringAsync()));


    }

    /// <summary>
    /// Get message by message Id from MESH Inbox asynchronously.
    /// </summary>
    /// <param name="mailboxId">The Mailbox Id.</param>
    /// <param name="messageId">The Message Id.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">The Arugument Null Exception.</exception>
    /// <exception cref="Exception">The general Exception.</exception>
    public async Task<MeshResponse<GetMessageResponse>> GetMessageByIdAsync(string mailboxId, string messageId)
    {

        // Validations
        if (string.IsNullOrWhiteSpace(mailboxId)) { throw new ArgumentNullException(nameof(mailboxId)); }
        if (string.IsNullOrWhiteSpace(messageId)) { throw new ArgumentNullException(nameof(messageId)); }
        if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiBaseUrl)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiBaseUrl)); }
        if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiInboxUriPath)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiInboxUriPath)); }

        var meshResponse = await GetSingleMessageAsync(mailboxId, messageId);

        var result = await ResponseHelper.CreateMeshResponse<GetMessageResponse>(meshResponse, async _ =>
        {
            return new GetMessageResponse
            {
                FileAttachment = await FileHelpers.CreateFileAttachment(_),
                MessageMetaData = FileHelpers.CreateMessageMetaData(_)

            };
        });

        return result;


    }


    public async Task<MeshResponse<GetChunkedMessageResponse>> GetChunkedMessageByIdAsync(string mailboxId, string messageId)
    {
        if (string.IsNullOrWhiteSpace(mailboxId)) { throw new ArgumentNullException(nameof(mailboxId)); }
        if (string.IsNullOrWhiteSpace(messageId)) { throw new ArgumentNullException(nameof(messageId)); }
        if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiBaseUrl)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiBaseUrl)); }
        if (string.IsNullOrWhiteSpace(_meshConnectConfiguration.MeshApiInboxUriPath)) { throw new ArgumentNullException(nameof(_meshConnectConfiguration.MeshApiInboxUriPath)); }


        var initialMessage = await GetSingleMessageAsync(mailboxId, messageId);

        if (initialMessage.StatusCode == HttpStatusCode.OK)
        {
            throw new InvalidOperationException($"MessageId: {messageId} in MailBox: {mailboxId} is not a chunked message, Use the GetMessageByIdAsync method to get this message");
        }
        else if (initialMessage.StatusCode != HttpStatusCode.PartialContent)
        {
            return await ResponseHelper.CreateMeshResponse<GetChunkedMessageResponse>(initialMessage, _ => null);
        }

        var chunkRange = initialMessage.Headers.GetHeaderItemValue("mex-chunk-range");

        var chunkRangeInts = FileHelpers.ParseChunkRange(chunkRange);
        List<FileAttachment> chunks = new List<FileAttachment>();
        chunks.Add(await FileHelpers.CreateFileAttachment(initialMessage));

        for (int i = 2; i <= chunkRangeInts.chunkLength; i++)
        {
            var meshResponse = await GetMessageChunkAsync(mailboxId, messageId, i);
            if (meshResponse.StatusCode == HttpStatusCode.OK)
            {
                throw new InvalidOperationException($"MessageId: {messageId} in MailBox: {mailboxId} is not a chunked message, Use the GetMessageByIdAsync method to get this message");
            }
            if (meshResponse.StatusCode != HttpStatusCode.PartialContent)
            {
                return await ResponseHelper.CreateMeshResponse<GetChunkedMessageResponse>(initialMessage, _ => null);
            }

            var chunk = await FileHelpers.CreateFileAttachment(meshResponse);
            chunks.Add(chunk);
        }

        return new MeshResponse<GetChunkedMessageResponse>
        {
            IsSuccessful = true,
            Response = new GetChunkedMessageResponse
            {
                FileAttachments = chunks,
                MessageMetaData = FileHelpers.CreateMessageMetaData(initialMessage)
            }

        };
    }

    /// <summary>
    /// Get message meta data by message Id from MESH Inbox asynchronously.
    /// </summary>
    /// <param name="mailboxId">The Mailbox Id.</param>
    /// <param name="messageId">The Message Id.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">The Arugument Null Exception.</exception>
    /// <exception cref="Exception">The general Exception.</exception>
    public async Task<MeshResponse<HeadMessageResponse>> GetHeadMessageByIdAsync(string mailboxId, string messageId)
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

        // Get Messages
        var meshResponse = await _meshConnectClient.SendRequestAsync(httpRequestMessage);

            return await ResponseHelper.CreateMeshResponse<HeadMessageResponse>(meshResponse,async _ => {
                await Task.CompletedTask;
                return new HeadMessageResponse{
                    messageMetaData = new MessageMetaData
                    {
                        WorkflowID = _.Headers.GetHeaderItemValue("mex-workflowid"),
                        ToMailbox = _.Headers.GetHeaderItemValue("mex-to"),
                        FromMailbox = _.Headers.GetHeaderItemValue("mex-from"),
                        MessageId = _.Headers.GetHeaderItemValue("mex-messageid"),
                        FileName = _.Headers.GetHeaderItemValue("mex-filename"),
                        MessageType = _.Headers.GetHeaderItemValue("mex-messagetype")
                    }
                };
            });
        }

    /// <summary>
    /// Acknowledge sent message by message Id from MESH Inbox asynchronously.
    /// </summary>
    /// <param name="mailboxId">The Mailbox Id.</param>
    /// <param name="messageId">The Message Id.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">The Arugument Null Exception.</exception>
    /// <exception cref="Exception">The general Exception.</exception>
    public async Task<MeshResponse<AcknowledgeMessageResponse>> AcknowledgeMessageByIdAsync(string mailboxId, string messageId)
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
            httpRequestMessage.Headers.Add("accept", "*/*");
            httpRequestMessage.Headers.Add("User_Agent", "my-client;windows-10;");

        // Get Messages
        var meshResponse = await _meshConnectClient.SendRequestAsync(httpRequestMessage);

        return await ResponseHelper.CreateMeshResponse<AcknowledgeMessageResponse>(meshResponse, async _ => JsonSerializer.Deserialize<AcknowledgeMessageResponse>(await _.Content.ReadAsStringAsync()));

    }

    #region PrivateMethods
    private Task<HttpResponseMessage> GetSingleMessageAsync(string mailboxId, string messageId)
    {
        var uri = new Uri(_meshConnectConfiguration.MeshApiBaseUrl + "/" + mailboxId + "/" + _meshConnectConfiguration.MeshApiInboxUriPath + "/" + messageId);
        return GetMessageAsync(uri, mailboxId);

    }

    private Task<HttpResponseMessage> GetMessageChunkAsync(string mailboxId, string messageId, int ChunkNumber)
    {
        var uri = new Uri(_meshConnectConfiguration.MeshApiBaseUrl + "/" + mailboxId + "/" + _meshConnectConfiguration.MeshApiInboxUriPath + "/" + messageId + "/" + ChunkNumber.ToString());
        return GetMessageAsync(uri, mailboxId);
    }

    private async Task<HttpResponseMessage> GetMessageAsync(Uri uri, string mailboxId)
    {
        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.RequestUri = uri;

        // Request Method
        httpRequestMessage.Method = HttpMethod.Get;

        // Headers
        var authHeader = MeshAuthorizationHelper.GenerateAuthHeaderValue(mailboxId);
        httpRequestMessage.Headers.Add("authorization", authHeader);
        httpRequestMessage.Headers.Add("accept", "application/vnd.mesh.v2+json");

        // Get Messages
        return await _meshConnectClient.SendRequestAsync(httpRequestMessage);

    }
    #endregion

}
