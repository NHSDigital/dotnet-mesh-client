// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMeshInboxService.cs" company="NHS">
// Copyright (c) NHS. All rights reserved.
// Year: 2024
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Net;

namespace NHS.MESH.Client.Contracts.Services
{
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
        Task<KeyValuePair<HttpStatusCode, string>> GetMessagesAsync(string mailboxId);

        /// <summary>
        /// Get message by message Id from MESH Inbox asynchronously.
        /// </summary>
        /// <param name="mailboxId">The Mailbox Id.</param>
        /// <param name="messageId">The Message Id.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The Arugument Null Exception.</exception>
        /// <exception cref="Exception">The general Exception.</exception>
        Task<KeyValuePair<HttpStatusCode, string>> GetMessageByIdAsync(string mailboxId, string messageId);

        /// <summary>
        /// Get message meta data by message Id from MESH Inbox asynchronously.
        /// </summary>
        /// <param name="mailboxId">The Mailbox Id.</param>
        /// <param name="messageId">The Message Id.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The Arugument Null Exception.</exception>
        /// <exception cref="Exception">The general Exception.</exception>
        Task<KeyValuePair<HttpStatusCode, string>> GetHeadMessageByIdAsync(string mailboxId, string messageId);

        /// <summary>
        /// Acknowledge sent message by message Id from MESH Inbox asynchronously.
        /// </summary>
        /// <param name="mailboxId">The Mailbox Id.</param>
        /// <param name="messageId">The Message Id.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The Arugument Null Exception.</exception>
        /// <exception cref="Exception">The general Exception.</exception>
        Task<KeyValuePair<HttpStatusCode, string>> AcknowledgeMessageByIdAsync(string mailboxId, string messageId);
    }
}
