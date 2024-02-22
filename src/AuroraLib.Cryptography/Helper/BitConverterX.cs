using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AuroraLib.Cryptography.Helper
{
    internal static class BitConverterX
    {
        /// <summary>
        /// Swaps the bits in the given byte value.
        /// </summary>
        /// <param name="value">The byte value to swap the bits for.</param>
        /// <returns>The byte value with swapped bits.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Swap(byte value)
            => (byte)((value * 0x0202020202ul & 0x010884422010ul) % 1023);

        /// <summary>
        /// Converts a instance of <typeparamref name="T"/> to a byte array.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <returns>A byte array representing the value.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] GetBytes<T>(ref T value) where T : unmanaged
        {
            byte[] result = new byte[Unsafe.SizeOf<T>()];
            MemoryMarshal.Write(result, ref value);
            return result;
        }

        /// <inheritdoc cref="GetBytes{T}(ref T)"/>
        [DebuggerStepThrough]
        public static byte[] GetBytes<T>(T value) where T : unmanaged
            => GetBytes(ref value);

        /// <summary>
        /// Swaps the bytes in the 16-bit unsigned integer.
        /// </summary>
        /// <param name="value">The ushort value to swap the bytes for.</param>
        /// <returns>The ushort value with swapped bytes.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Swap(ushort value)
            => (ushort)((value & 0xFF) << 8 | value >> 8 & 0xFF);

        /// <summary>
        /// Swaps the bytes in the 16-bit signed integer.
        /// </summary>
        /// <param name="value">The short value to swap the bytes for.</param>
        /// <returns>The short value with swapped bytes.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short Swap(short value)
            => (short)Swap((ushort)value);

        /// <summary>
        /// Swaps the bytes in the 32-bit unsigned integer.
        /// </summary>
        /// <param name="value">The uint value to swap the bytes for.</param>
        /// <returns>The uint value with swapped bytes.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Swap(uint value)
            => (value & 0x000000ff) << 24 | (value & 0x0000ff00) << 8 | (value & 0x00ff0000) >> 8 | (value & 0xff000000) >> 24;

        /// <summary>
        /// Swaps the bytes in the 32-bit signed integer.
        /// </summary>
        /// <param name="value">The int value to swap the bytes for.</param>
        /// <returns>The int value with swapped bytes.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Swap(int value)
            => (int)Swap((uint)value);

        /// <summary>
        /// Swaps the bytes in the 64-bit unsigned integer.
        /// </summary>
        /// <param name="value">The ulong value to swap the bytes for.</param>
        /// <returns>The ulong value with swapped bytes.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Swap(ulong value)
            => 0x00000000000000FF & value >> 56 | 0x000000000000FF00 & value >> 40 | 0x0000000000FF0000 & value >> 24 | 0x00000000FF000000 & value >> 8 |
            0x000000FF00000000 & value << 8 | 0x0000FF0000000000 & value << 24 | 0x00FF000000000000 & value << 40 | 0xFF00000000000000 & value << 56;

        /// <summary>
        /// Swaps the bytes in the 64-bit signed integer.
        /// </summary>
        /// <param name="value">The long value to swap the bytes for.</param>
        /// <returns>The long value with swapped bytes.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Swap(long value)
            => (long)Swap((ulong)value);

        /// <summary>
        /// Swaps the bits of the specified instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="vaule">The instance to swap the bits for.</param>
        /// <returns>An instance of <typeparamref name="T"/> with swapped bits.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SwapBits<T>(ref T vaule) where T : unmanaged
        {
            Span<byte> src = vaule.AsBytes();
            src.Reverse();
            for (int i = 0; i < src.Length; i++)
            {
                src[i] = Swap(src[i]);
            }
            return vaule;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<byte> AsBytes<T>(this ref T input) where T : unmanaged
        {
            ref byte tRef = ref Unsafe.As<T, byte>(ref input);
            return MemoryMarshal.CreateSpan(ref tRef, Unsafe.SizeOf<T>());
        }
    }
}
