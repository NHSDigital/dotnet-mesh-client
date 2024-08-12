// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SendMessageResponse.cs" company="NHS">
// Copyright (c) NHS. All rights reserved.
// Year: 2024
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NHS.MESH.Client.Models;
using System.Text.Json.Serialization;

/// <summary>The response from Send Message MESH API.</summary>
public class SendMessageResponse
{
    [JsonPropertyName("message_id")]
    /// <summary>Gets or sets the MessageId.</summary>
    public string? MessageId { get; set; }
}
