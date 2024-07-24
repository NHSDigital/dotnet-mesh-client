// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICustomException.cs" company="NHS">
// Copyright (c) NHS. All rights reserved.
// Year: 2024
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NHS.MESH.Client.Contracts.Exceptions
{
    /// <summary>An abstract implementation of a custom exception.</summary>
    public interface ICustomException
    {
        /// <summary>Gets the error code.</summary>
        string ErrorCode { get; }

        /// <summary>Gets the error message.</summary>
        string ErrorMessage { get; }
    }
}
