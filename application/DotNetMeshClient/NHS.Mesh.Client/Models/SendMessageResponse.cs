// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SendMessageResponse.cs" company="NHS">
// Copyright (c) NHS. All rights reserved.
// Year: 2024
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace NHS.MESH.Client.Models
{
    /// <summary>The response from Send Message MESH API.</summary>
    public class SendMessageResponse
    {
        [JsonPropertyName("message_id")]
        /// <summary>Gets or sets the MessageId.</summary>
        public string? MessageId { get; set; }
    }
}
