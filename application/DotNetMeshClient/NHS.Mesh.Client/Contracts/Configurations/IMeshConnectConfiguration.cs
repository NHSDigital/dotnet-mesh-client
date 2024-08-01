// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMeshConnectConfiguration.cs" company="NHS">
// Copyright (c) NHS. All rights reserved.
// Year: 2024
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NHS.MESH.Client.Contracts.Configurations
{
    /// <summary>An abstract implementation of the Mesh Connect configuration.</summary>
    public interface IMeshConnectConfiguration
    {
        /// <summary>Gets or sets TimeoutInSeconds.</summary>
        int TimeoutInSeconds { get; set; }

        /// <summary>Gets or sets MaxRetries.</summary>
        int MaxRetries { get; set; }

        /// <summary>Gets or sets MeshApiBaseUrl.</summary>
        string? MeshApiBaseUrl { get; set; }

        /// <summary>Gets or sets MeshApiHanshakeUriPath.</summary>
        string? MeshApiHanshakeUriPath { get; set; }

        /// <summary>Gets or sets MeshApiInboxUriPath.</summary>
        string? MeshApiInboxUriPath { get; set; }

        /// <summary>Gets or sets MeshApiOutboxUriPath.</summary>
        string? MeshApiOutboxUriPath { get; set; }

        /// <summary>Gets or sets MeshApiSendMessageUriPath.</summary>
        string? MeshApiSendMessageUriPath { get; set; }

        /// <summary>Gets or sets MeshApiListMessagesUriPath.</summary>
        string? MeshApiListMessagesUriPath { get; set; }

        /// <summary>Gets or sets MeshApiGetMessageUriPath.</summary>
        string? MeshApiGetMessageUriPath { get; set; }

        /// <summary>Gets or sets MeshApiAcknowledgeUriPath.</summary>
        string? MeshApiAcknowledgeUriPath { get; set; }

        /// <summary>Gets or sets MeshApiTrackMessageUriPath.</summary>
        string? MeshApiTrackMessageUriPath { get; set; }

        /// <summary>Gets or sets a value indicating whether proxy enabled.</summary>
        bool ProxyEnabled { get; set; }

        /// <summary>Gets or sets the proxy address.</summary>
        string ProxyAddress { get; set; }

        /// <summary>Gets or sets a value indicating whether proxy use default credentials.</summary>
        bool ProxyUseDefaultCredentials { get; set; }
        /// <summary>Gets the chunk size in bytes for sending chunked messages 19Mb limit outside of HSCN 100Mb limit within</summary>
        long ChunkSize { get; set; }
    }
}
