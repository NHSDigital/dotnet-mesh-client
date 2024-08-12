// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMeshInboxService.cs" company="NHS">
// Copyright (c) NHS. All rights reserved.
// Year: 2024
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NHS.MESH.Client.Contracts.Services;
using System.Net;
using NHS.MESH.Client.Models;

/// <summary>The MeshInboxService class has functions to get the required details to make Http calls for getting messages from Mesh API.</summary>
public interface IMeshInboxService
{
    /// <summary>
    /// Send messages asynchronously to the MESH API.
    /// </summary>
    /// <param name="mailboxId">The Mailbox Id.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">The Arugument Null Exception.</exception>
    /// <exception cref="Exception">The general Exception.</exception>
    Task<MeshResponse<CheckInboxResponse>> GetMessagesAsync(string mailboxId);

    /// <summary>
    /// Get message by message Id from MESH Inbox asynchronously.
    /// </summary>
    /// <param name="mailboxId">The Mailbox Id.</param>
    /// <param name="messageId">The Message Id.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">The Arugument Null Exception.</exception>
    /// <exception cref="Exception">The general Exception.</exception>
    Task<MeshResponse<GetMessageResponse>> GetMessageByIdAsync(string mailboxId, string messageId);

    Task<MeshResponse<GetChunkedMessageResponse>> GetChunkedMessageByIdAsync(string mailboxId, string messageId);
    /// <summary>
    /// Get message meta data by message Id from MESH Inbox asynchronously.
    /// </summary>
    /// <param name="mailboxId">The Mailbox Id.</param>
    /// <param name="messageId">The Message Id.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">The Arugument Null Exception.</exception>
    /// <exception cref="Exception">The general Exception.</exception>
    Task<MeshResponse<HeadMessageResponse>> GetHeadMessageByIdAsync(string mailboxId, string messageId);

    /// <summary>
    /// Acknowledge sent message by message Id from MESH Inbox asynchronously.
    /// </summary>
    /// <param name="mailboxId">The Mailbox Id.</param>
    /// <param name="messageId">The Message Id.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">The Arugument Null Exception.</exception>
    /// <exception cref="Exception">The general Exception.</exception>
    Task<MeshResponse<AcknowledgeMessageResponse>> AcknowledgeMessageByIdAsync(string mailboxId, string messageId);
}
