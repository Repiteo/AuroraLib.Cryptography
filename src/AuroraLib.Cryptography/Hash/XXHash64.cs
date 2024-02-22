﻿using AuroraLib.Cryptography.Helper;
using AuroraLib.Interfaces;
using HashDepot;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AuroraLib.Cryptography.Hash
{
    /// <summary>
    /// 64-bit xxHash implementation.
    /// </summary>
    public sealed class XXHash64 : IHash<ulong>
    {
        /// <inheritdoc />
        public ulong Value { get; private set; }

        /// <inheritdoc />
        public int ByteSize => 8;

        /// <inheritdoc />
        public void Compute(ReadOnlySpan<byte> input)
            => Value = XXHash.Hash64(input, Value);

        /// <inheritdoc />
        public byte[] GetBytes()
            => BitConverterX.GetBytes(Value);

        /// <inheritdoc />
        public void Write(Span<byte> destination)
        {
            ulong vaule = Value;
            MemoryMarshal.Write(destination, ref vaule);
        }

        /// <inheritdoc />
        public void SetSeed(ulong seed = 0u)
            => Value = seed;

        /// <inheritdoc />
        public void Reset() => Value = 0;

        /// <summary>
        /// Generate a 64-bit xxHash value
        /// </summary>
        /// <param name="input">The input data to compute the hash for.</param>
        /// <param name="seed">The seed value to set.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Generate(ReadOnlySpan<byte> input, ulong seed = 0u)
            => XXHash.Hash64(input, seed);
    }
}
