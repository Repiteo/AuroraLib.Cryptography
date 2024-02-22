﻿using AuroraLib.Cryptography.Helper;
using AuroraLib.Interfaces;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AuroraLib.Cryptography.Hash
{
    /// <summary>
    /// 128-bit CityHash implementation.
    /// </summary>
    public sealed class CityHash128 : IHash<UInt128>
    {
        /// <inheritdoc />
        public UInt128 Value { get; private set; }

        /// <inheritdoc />
        public int ByteSize => 16;

        /// <inheritdoc />
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Compute(ReadOnlySpan<byte> input)
        {
            Value = Value == 0 ? CityHash.Hash128(input) : CityHash.Hash128(input, Value);
        }

        /// <inheritdoc />
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] GetBytes()
            => BitConverterX.GetBytes(Value);

        /// <inheritdoc />
        public void Write(Span<byte> destination)
        {
            UInt128 vaule = Value;
            MemoryMarshal.Write(destination, ref vaule);
        }

        /// <inheritdoc />
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
            => Value = 0;

        /// <inheritdoc />
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSeed(UInt128 seed)
            => Value = seed;

        /// <summary>
        /// Generates a 128-bit CityHash hash from the provided input.
        /// </summary>
        /// <param name="input">The input data to hash as a ReadOnlySpan of bytes.</param>
        /// <returns>A 128-bit hash as a UInt128 value.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt128 Generate(ReadOnlySpan<byte> input)
            => CityHash.Hash128(input);

        /// <summary>
        /// Generates a 128-bit CityHash hash from the provided input and a seed value.
        /// </summary>
        /// <param name="input">The input data to hash as a ReadOnlySpan of bytes.</param>
        /// <param name="seed">A seed value for the hash calculation.</param>
        /// <returns>A 128-bit hash as a UInt128 value.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt128 Generate(ReadOnlySpan<byte> input, UInt128 seed)
            => CityHash.Hash128(input, seed);


    }
}
