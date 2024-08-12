// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMeshOperationService.cs" company="NHS">
// Copyright (c) NHS. All rights reserved.
// Year: 2024
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System.Net;
using NHS.MESH.Client.Models;

namespace NHS.MESH.Client.Contracts.Services;

/// <summary>The MESH Operations service.</summary>
public interface IMeshOperationService
{
    /// <summary>
    /// Validate a MESH Mailbox asynchronously.
    /// </summary>
    /// <param name="mailboxId">The Mailbox Id.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">The Arugument Null Exception.</exception>
    /// <exception cref="Exception">The general Exception.</exception>
    Task<MeshResponse<HandshakeResponse>> MeshHandshakeAsync(string mailboxId);
}
