// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentSplitHelper.cs" company="NHS">
// Copyright (c) NHS. All rights reserved.
// Year: 2024
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using NHS.MESH.Client.Models;
using System.IO.Compression;

namespace NHS.MESH.Client.Helpers.ContentHelpers
{
    /// <summary>Provides helper functions for content split.</summary>
    public static class ContentSplitHelper
    {
        /// <summary>
        /// Used for spliting larger files to samller chunks.
        /// </summary>
        /// <param name="inputFilePath">The input Path.</param>
        /// <param name="chunkSize">The file size.</param>
        /// <returns></returns>
        public static async Task<List<MemoryStream>> SplitFileToMemoryStreams(Stream stream, long chunkSize = 19 * 1024 * 1024)
        {
            List<MemoryStream> compressedChunks = new List<MemoryStream>();

            using (var fileStream = stream.BeginRead())
            {
                byte[] buffer = new byte[chunkSize];
                int bytesRead;

                while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    var chunkMemoryStream = new MemoryStream();
                    using (var gzipStream = new GZipStream(chunkMemoryStream, CompressionMode.Compress, true))
                    {
                        gzipStream.Write(buffer, 0, bytesRead);
                    }

                    // Reset the position of the MemoryStream for reading later
                    chunkMemoryStream.Position = 0;
                    compressedChunks.Add(chunkMemoryStream);
                }
            }

            return compressedChunks;
        }
    }
}
