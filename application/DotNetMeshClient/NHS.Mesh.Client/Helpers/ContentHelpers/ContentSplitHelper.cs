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
        public static async Task<List<byte[]>> SplitFileToMemoryStreams(byte[] fileData, int chunkSize = 19 * 1024 * 1024)
        {
            List<byte[]> chunks = new List<byte[]>();


            int offset = 0;
            var memoryStream = new MemoryStream(fileData);

            while(true)
            {
                byte[] buffer = new byte[chunkSize];
                int bytesRead = await memoryStream.ReadAsync(buffer,offset,chunkSize);
                if(bytesRead == 0)
                {
                    return chunks;
                }
                chunks.Add(buffer);
                if(bytesRead < chunkSize)
                {
                    return chunks;
                }
                offset += bytesRead;
            }



        }
    }
}
