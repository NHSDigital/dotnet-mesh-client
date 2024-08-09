// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentSplitHelper.cs" company="NHS">
// Copyright (c) NHS. All rights reserved.
// Year: 2024
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using NHS.MESH.Client.Models;
using System.IO.Compression;
using System.Reflection.Metadata;

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


            long offset = 0;
            var memoryStream = new MemoryStream(fileData);
            long bytesLeft = memoryStream.Length;
            while(bytesLeft > 0)
            {
                byte[] buffer = new byte[chunkSize];


                int bytesRead = await memoryStream.ReadAsync(buffer,0,(int)Math.Min(bytesLeft,chunkSize));
                if(bytesRead == 0)
                {
                    return chunks;
                }
                chunks.Add(buffer);
                if(bytesRead < chunkSize)
                {
                    return chunks;
                }
                bytesLeft -= bytesRead;
                offset += bytesRead;
            }

            return chunks;



        }
    }
}
