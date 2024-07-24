// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeshConnectConfiguration.cs" company="NHS">
// Copyright (c) NHS. All rights reserved.
// Year: 2024
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using NHS.MESH.Client.Contracts.Configurations;

namespace NHS.MESH.Client.Configuration
{
    /// <summary>The MESH connect Configurations.</summary>
    public class MeshConnectConfiguration : IMeshConnectConfiguration
    {
        /// <summary>Gets or sets TimeoutInSeconds.</summary>
        public int TimeoutInSeconds { get; set; }

        /// <summary>Gets or sets MaxRetries.</summary>
        public int MaxRetries { get; set; }

        /// <summary>Gets or sets MeshApiBaseUrl.</summary>
        public string? MeshApiBaseUrl { get; set; }

        /// <summary>Gets or sets MeshApiHanshakeUriPath.</summary>
        public string? MeshApiHanshakeUriPath { get; set; }

        /// <summary>Gets or sets MeshApiInboxUriPath.</summary>
        public string? MeshApiInboxUriPath { get; set; }

        /// <summary>Gets or sets MeshApiOutboxUriPath.</summary>
        public string? MeshApiOutboxUriPath { get; set; }

        /// <summary>Gets or sets MeshApiSendMessageUriPath.</summary>
        public string? MeshApiSendMessageUriPath { get; set; }

        /// <summary>Gets or sets MeshApiListMessagesUriPath.</summary>
        public string? MeshApiListMessagesUriPath { get; set; }

        /// <summary>Gets or sets MeshApiGetMessageUriPath.</summary>
        public string? MeshApiGetMessageUriPath { get; set; }

        /// <summary>Gets or sets MeshApiAcknowledgeUriPath.</summary>
        public string? MeshApiAcknowledgeUriPath { get; set; }

        /// <summary>Gets or sets MeshApiTrackMessageUriPath.</summary>
        public string? MeshApiTrackMessageUriPath { get; set; }

        /// <summary>Gets or sets a value indicating whether proxy enabled.</summary>
        public bool ProxyEnabled { get; set; }

        /// <summary>Gets or sets the proxy address.</summary>
        public string ProxyAddress { get; set; }

        /// <summary>Gets or sets a value indicating whether proxy use default credentials.</summary>
        public bool ProxyUseDefaultCredentials { get; set; }
    }
}
