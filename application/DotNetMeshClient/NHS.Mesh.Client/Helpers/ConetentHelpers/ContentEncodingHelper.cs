// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentEncodingHelper.cs" company="NHS">
// Copyright (c) NHS. All rights reserved.
// Year: 2024
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;

namespace NHS.MESH.Client.Helpers.ConetentHelpers
{
    /// <summary>Provides helper functions for content encoding.</summary>
    public static class ContentEncodingHelper
    {
        /// <summary>
        /// Used to generate a SHA256 hash with the given parameters.
        /// </summary>
        /// <param name="key">The key value.</param>
        /// <param name="message">The message.</param>
        /// <returns>The content Hash.</returns>
        public static string GenerateContentHash(string key, string message)
        {
            using (var hmacSha256 = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hashBytes = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(message));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

    }
}
