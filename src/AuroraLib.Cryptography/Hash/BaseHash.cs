﻿using AuroraLib.Interfaces;
using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace AuroraLib.Cryptography.Hash
{
#if !NET20_OR_GREATER
    /// <summary>
    /// Can be used for any <see cref="HashAlgorithm"/> implementation.
    /// </summary>
    public sealed class BaseHash : IHash
    {
        private readonly byte[] bytes;

        /// <inheritdoc />
        public int ByteSize => hashInstance.HashSize / 8;

        private readonly HashAlgorithm hashInstance;

        public BaseHash(HashAlgorithm algorithm)
        {
            hashInstance = algorithm;
            bytes = new byte[ByteSize];
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Compute(ReadOnlySpan<byte> input)
            => hashInstance.TryComputeHash(input, bytes, out _);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] GetBytes()
        {
            byte[] output = new byte[bytes.Length];
            bytes.AsSpan().CopyTo(output);
            return output;
        }

        /// <inheritdoc />
        public void Write(Span<byte> destination)
            => bytes.AsSpan().CopyTo(destination);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            bytes.AsSpan().Clear();
            hashInstance.Initialize();
        }
    }
#endif
}
