// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMeshOutboxService.cs" company="NHS">
// Copyright (c) NHS. All rights reserved.
// Year: 2024
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NHS.MESH.Client.Contracts.Services;
using Microsoft.AspNetCore.Http;
using NHS.MESH.Client.Models;
using System.Net;

/// <summary>The MeshOutboxService class has functions to get the required details to make Http calls for sending messages to Mesh API.</summary>
public interface IMeshOutboxService
{
    /// <summary>
    /// Send messages asynchronously to the MESH API.
    /// </summary>
    /// <param name="fromMailboxId">The Mailbox Id.</param>
    /// <param name="toMailboxId">The Mailbox Id.</param>
    /// <param name="file">The request content file.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">The Arugument Null Exception.</exception>
    /// <exception cref="Exception">The general Exception.</exception>
    Task<MeshResponse<SendMessageResponse>> SendCompressedMessageAsync(string fromMailboxId, string toMailboxId, string workflowId, FileAttachment file, string? localId = null, string? subject = null, bool includeChecksum = false);

    /// <summary>
    /// Send messages asynchronously to the MESH API.
    /// </summary>
    /// <param name="fromMailboxId">The Mailbox Id.</param>
    /// <param name="toMailboxId">The Mailbox Id.</param>
    /// <param name="file">The request content file.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">The Arugument Null Exception.</exception>
    /// <exception cref="Exception">The general Exception.</exception>
    Task<MeshResponse<SendMessageResponse>> SendUnCompressedMessageAsync(string fromMailboxId, string toMailboxId, string workflowId, FileAttachment file, string? localId = null, string? subject = null, bool includeChecksum = false);

    /// <summary>
    /// Send chunked messages asynchronously to the MESH API.
    /// </summary>
    /// <param name="fromMailboxId">The Mailbox Id.</param>
    /// <param name="toMailboxId">The Mailbox Id.</param>
    /// <param name="file">The request content file.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">The Arugument Null Exception.</exception>
    /// <exception cref="Exception">The general Exception.</exception>
    Task<MeshResponse<SendMessageResponse>> SendChunkedMessageAsync(string fromMailboxId, string toMailboxId, string workflowId, FileAttachment file, string? localId = null, string? subject = null, bool includeChecksum = false);

    /// <summary>
    /// Get message status by message Id from MESH Inbox asynchronously.
    /// </summary>
    /// <param name="mailboxId">The Mailbox Id.</param>
    /// <param name="messageId">The Message Id.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">The Arugument Null Exception.</exception>
    /// <exception cref="Exception">The general Exception.</exception>
    Task<MeshResponse<TrackOutboxResponse>> TrackMessageByIdAsync(string mailboxId, string messageId);
}
