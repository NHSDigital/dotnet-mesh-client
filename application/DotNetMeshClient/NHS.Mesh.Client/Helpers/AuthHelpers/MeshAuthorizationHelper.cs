// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeshAuthorizationHelper.cs" company="NHS">
// Copyright (c) NHS. All rights reserved.
// Year: 2024
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NHS.MESH.Client.Helpers.AuthHelpers;
using NHS.MESH.Client.Helpers.ContentHelpers;

/// <summary>Provides helper functions for Mesh Authorization.</summary>
public static class MeshAuthorizationHelper
{
    private const string AuthSchemaName = "NHSMESH";

    public static string GenerateAuthHeaderValue(
        string mailboxId,
        string password,
        string sharedKey,
        string? timestamp = null,
        string? nonce = null,
        int nonceCount = 0)
    {
        if (string.IsNullOrEmpty(mailboxId))
        {
            throw new ArgumentNullException(nameof(mailboxId));
        }

        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentNullException(nameof(password));
        }

        nonce ??= Guid.NewGuid().ToString();

        timestamp ??= DateTime.UtcNow.ToString("yyyyMMddHHmm");

        string hmacMessage = $"{mailboxId}:{nonce}:{nonceCount}:{password}:{timestamp}";

        string hashCode = ContentEncodingHelper.GenerateContentHash(sharedKey, hmacMessage);

        return $"{AuthSchemaName} {mailboxId}:{nonce}:{nonceCount}:{timestamp}:{hashCode}";
    }
}
