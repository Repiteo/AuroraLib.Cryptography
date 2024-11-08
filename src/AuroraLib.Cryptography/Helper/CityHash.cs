using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace AuroraLib.Cryptography.Helper
{
    //copy of https://github.com/google/cityhash/blob/master/src/city.cc
    internal static class CityHash
    {
        // Some primes between 2^63 and 2^64 for various uses.
        private const ulong k0 = 0xc3a5c85c97cb3127UL;
        private const ulong k1 = 0xb492b66fbe98f273UL;
        private const ulong k2 = 0x9ae16a3b2f90404fUL;
        // Magic numbers for 32-bit hashing.  Copied from Murmur3.
        private const uint c1 = 0xcc9e2d51;
        private const uint c2 = 0x1b873593;
        private const uint p1 = 0x85ebca6b;
        private const uint p2 = 0xc2b2ae35;


        #region Hash
        internal static uint Hash32(ReadOnlySpan<byte> input)
        {
            uint len = (uint)input.Length;

            if (len <= 4)
                return Hash32Len0to4(input);

            if (len <= 12)
                return Hash32Len5to12(input);

            if (len <= 24)
                return Hash32Len13to24(input);

            // len > 24
            uint h = len, g = c1 * len, f = g;
            uint a0 = RotateRight(ReadUnaligned32(input.Slice(input.Length - 4)) * c1, 17) * c2;
            uint a1 = RotateRight(ReadUnaligned32(input.Slice(input.Length - 8)) * c1, 17) * c2;
            uint a2 = RotateRight(ReadUnaligned32(input.Slice(input.Length - 16)) * c1, 17) * c2;
            uint a3 = RotateRight(ReadUnaligned32(input.Slice(input.Length - 12)) * c1, 17) * c2;
            uint a4 = RotateRight(ReadUnaligned32(input.Slice(input.Length - 20)) * c1, 17) * c2;

            h ^= a0;
            h = RotateRight(h, 19);
            h = h * 5 + 0xe6546b64;
            h ^= a2;
            h = RotateRight(h, 19);
            h = h * 5 + 0xe6546b64;

            g ^= a1;
            g = RotateRight(g, 19);
            g = g * 5 + 0xe6546b64;
            g ^= a3;
            g = RotateRight(g, 19);
            g = g * 5 + 0xe6546b64;

            f += a4;
            f = RotateRight(f, 19);
            f = f * 5 + 0xe6546b64;

            for (int i = 0; i < (len - 1) / 20; i++)
            {
                a0 = RotateRight(ReadUnaligned32(input.Slice(20 * i)) * c1, 17) * c2;
                a1 = ReadUnaligned32(input.Slice(20 * i + 4));
                a2 = RotateRight(ReadUnaligned32(input.Slice(20 * i + 8)) * c1, 17) * c2;
                a3 = RotateRight(ReadUnaligned32(input.Slice(20 * i + 12)) * c1, 17) * c2;
                a4 = ReadUnaligned32(input.Slice(20 * i + 16));

                h ^= a0;
                h = RotateRight(h, 18);
                h = h * 5 + 0xe6546b64;

                f += a1;
                f = RotateRight(f, 19);
                f *= c1;

                g += a2;
                g = RotateRight(g, 18);
                g = g * 5 + 0xe6546b64;

                h ^= a3 + a1;
                h = RotateRight(h, 19);
                h = h * 5 + 0xe6546b64;

                g ^= a4;
                g = BSwap32(g) * 5;

                h += a4 * 5;
                h = BSwap32(h);

                f += a0;

                Permute3(ref f, ref h, ref g);
            }

            g = RotateRight(g, 11) * c1;
            g = RotateRight(g, 17) * c1;

            f = RotateRight(f, 11) * c1;
            f = RotateRight(f, 17) * c1;

            h = RotateRight(h + g, 19);
            h = h * 5 + 0xe6546b64;
            h = RotateRight(h, 17) * c1;

            h = RotateRight(h + f, 19);
            h = h * 5 + 0xe6546b64;
            h = RotateRight(h, 17) * c1;

            return h;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong Hash64(ReadOnlySpan<byte> input, ulong seed)
            => Hash64(input, k2, seed);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong Hash64(ReadOnlySpan<byte> input, ulong seed0, ulong seed1)
            => Hash128to64(Hash64(input) - seed0, seed1);

        internal static ulong Hash64(ReadOnlySpan<byte> input)
        {
            if (input.Length <= 16)
                return HashLen0to16(input);

            if (input.Length <= 32)
                return HashLen17to32(input);

            if (input.Length <= 64)
                return HashLen33to64(input);

            // For strings over 64 bytes we hash the end first, and then as we
            // loop we keep 56 bytes of state: v, w, x, y, and z.

            ulong x = ReadUnaligned64(input, input.Length - 40);
            ulong y = ReadUnaligned64(input, input.Length - 16) + ReadUnaligned64(input, input.Length - 56);
            ulong z = Hash128to64(
                ReadUnaligned64(input, input.Length - 48) + (ulong)input.Length,
                ReadUnaligned64(input, input.Length - 24));

            UInt128 v = WeakHashLen32WithSeeds(input, input.Length - 64, (ulong)input.Length, z);
            UInt128 w = WeakHashLen32WithSeeds(input, input.Length - 32, y + k1, x);

            x = x * k1 + ReadUnaligned64(input);

            // Decrease len to the nearest multiple of 64, and operate on 64-byte chunks.

            int pos = 0;
            int len = input.Length - 1 & ~63;
            do
            {
                x = RotateRight(x + y + v.GetLow() + ReadUnaligned64(input, pos + 8), 37) * k1;
                y = RotateRight(y + v.GetHigh() + ReadUnaligned64(input, pos + 48), 42) * k1;
                x ^= w.GetHigh();
                y += v.GetLow() + ReadUnaligned64(input, pos + 40);
                z = RotateRight(z + w.GetLow(), 33) * k1;
                v = WeakHashLen32WithSeeds(input, pos, v.GetHigh() * k1, x + w.GetLow());
                w = WeakHashLen32WithSeeds(input, pos + 32, z + w.GetHigh(), y + ReadUnaligned64(input, pos + 16));
                Swap(ref z, ref x);

                pos += 64;
                len -= 64;
            } while (len != 0);

            return Hash128to64(Hash128to64(v.GetLow(), w.GetLow()) + ShiftMix(y) * k1 + z, Hash128to64(v.GetHigh(), w.GetHigh()) + x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static UInt128 Hash128(ReadOnlySpan<byte> value)
        {
            if (value.Length >= 16)
                return Hash128(value.Slice(16), new UInt128(ReadUnaligned64(value.Slice(8)) + k0, ReadUnaligned64(value)));
            else
                return Hash128(value, new UInt128(k1, k0));
        }

        internal static UInt128 Hash128(ReadOnlySpan<byte> value, UInt128 seed)
        {
            if (value.Length < 128)
                return CityMurmur(value, seed.GetLow(), seed.GetHigh());

            // We expect len >= 128 to be the common case.  Keep 56 bytes of state:
            // v, w, x, y, and z.
            int len = value.Length;
            ulong x = seed.GetLow();
            ulong y = seed.GetHigh();
            ulong z = (ulong)len * k1;

            UInt128 v = new UInt128(0, RotateRight(y ^ k1, 49) * k1 + ReadUnaligned64(value));
            v = new UInt128(RotateRight(v.GetLow(), 42) * k1 + ReadUnaligned64(value, 8), v.GetLow());

            UInt128 w = new UInt128(RotateRight(x + ReadUnaligned64(value, 88), 53) * k1, RotateRight(y + z, 35) * k1 + x);


            // This is the same inner loop as CityHash64(), manually unrolled.
            int s = 0;
            do
            {
                ulong temp = ReadUnaligned64(value, s + 8);
                x = RotateRight(x + y + v.GetLow() + temp, 37) * k1;
                y = RotateRight(y + v.GetHigh() + ReadUnaligned64(value, s + 48), 42) * k1;
                x ^= w.GetHigh();
                y += v.GetLow() + ReadUnaligned64(value, s + 40);
                z = RotateRight(z + w.GetLow(), 33) * k1;
                v = WeakHashLen32WithSeeds(value, s, v.GetHigh() * k1, x + w.GetLow());
                w = WeakHashLen32WithSeeds(value, s + 32, z + w.GetHigh(), y + ReadUnaligned64(value, s + 16));
                Swap(ref z, ref x);
                s += 64;
                x = RotateRight(x + y + v.GetLow() + ReadUnaligned64(value, s + 8), 37) * k1;
                y = RotateRight(y + v.GetHigh() + ReadUnaligned64(value, s + 48), 42) * k1;
                x ^= w.GetHigh();
                y += v.GetLow() + ReadUnaligned64(value, s + 40);
                z = RotateRight(z + w.GetLow(), 33) * k1;
                v = WeakHashLen32WithSeeds(value, s, v.GetHigh() * k1, x + w.GetLow());
                w = WeakHashLen32WithSeeds(value, s + 32, z + w.GetHigh(), y + ReadUnaligned64(value, s + 16));
                Swap(ref z, ref x);
                s += 64;
                len -= 128;

            } while (len >= 128);

            x += RotateRight(v.GetLow() + z, 49) * k0;
            y = y * k0 + RotateRight(w.GetHigh(), 37);
            z = z * k0 + RotateRight(w.GetLow(), 27);
            w = new UInt128(w.GetHigh(), w.GetLow() * 9);
            v = new UInt128(v.GetHigh(), v.GetLow() * k0);

            // If 0 < len < 128, hash up to 4 chunks of 32 bytes each from the end of s.
            for (int tail = 0; tail < len;)
            {
                tail += 32;

                y = RotateRight(x + y, 42) * k0 + v.GetHigh();
                w = new UInt128(w.GetHigh(), w.GetLow() + ReadUnaligned64(value, s + len - tail + 16));
                x = x * k0 + w.GetLow();
                z += w.GetHigh() + ReadUnaligned64(value, s + len - tail);
                w = new UInt128(w.GetHigh() + v.GetLow(), w.GetLow());
                v = WeakHashLen32WithSeeds(value, s + len - tail, v.GetLow() + z, v.GetHigh());
                v = new UInt128(v.GetHigh(), v.GetLow() * k0);
            }


            // At this point our 56 bytes of state should contain more than
            // enough information for a strong 128-bit hash.  We use two
            // different 56-byte-to-8-byte hashes to get a 16-byte final result.
            x = Hash128to64(x, v.GetLow());
            y = Hash128to64(y + z, w.GetLow());

            return new UInt128(Hash128to64(x + v.GetHigh(), w.GetHigh()) + y, Hash128to64(x + w.GetHigh(), y + v.GetHigh()));
        }

        #endregion

        #region HashLen

        private static uint Hash32Len0to4(ReadOnlySpan<byte> value)
        {
            uint l = (uint)value.Length;
            uint b = 0u;
            uint c = 9u;
            for (int i = 0; i < l; i++)
            {
                b = b * c1 + (uint)(sbyte)value[i];
                c ^= b;
            }

            return MixFinal(Mur(b, Mur(l, c)));
        }

        private static uint Hash32Len5to12(ReadOnlySpan<byte> value)
        {
            uint a = (uint)value.Length, b = a * 5, c = 9, d = b;

            a += ReadUnaligned32(value);
            b += ReadUnaligned32(value.Slice(value.Length - 4));
            c += ReadUnaligned32(value.Slice((value.Length >> 1 & 4)));

            return MixFinal(Mur(c, Mur(b, Mur(a, d))));
        }

        private static uint Hash32Len13to24(ReadOnlySpan<byte> value)
        {
            uint a = ReadUnaligned32(value, (value.Length >> 1) - 4);
            uint b = ReadUnaligned32(value, 4);
            uint c = ReadUnaligned32(value, value.Length - 8);
            uint d = ReadUnaligned32(value, value.Length >> 1);
            uint e = ReadUnaligned32(value);
            uint f = ReadUnaligned32(value, value.Length - 4);
            uint h = (uint)value.Length;

            return MixFinal(Mur(f, Mur(e, Mur(d, Mur(c, Mur(b, Mur(a, h)))))));
        }

        private static ulong HashLen0to16(ReadOnlySpan<byte> value)
        {
            uint len = (uint)value.Length;

            if (len >= 8)
            {

                ulong mul = k2 + (ulong)len * 2;
                ulong a = ReadUnaligned64(value) + k2;
                ulong b = ReadUnaligned64(value, value.Length - 8);
                ulong c = RotateRight(b, 37) * mul + a;
                ulong d = (RotateRight(a, 25) + b) * mul;

                return HashLen16(c, d, mul);
            }

            if (len >= 4)
            {
                ulong mul = k2 + (ulong)len * 2;
                ulong a = ReadUnaligned32(value);
                return HashLen16(len + (a << 3), ReadUnaligned32(value, (int)(len - 4)), mul);
            }

            if (len > 0)
            {
                byte a = value[0];
                int b = value[value.Length >> 1];
                int c = value[value.Length - 1];

                ulong y = a + ((uint)b << 8);
                ulong z = len + ((uint)c << 2);

                return ShiftMix(y * k2 ^ z * k0) * k2;
            }

            return k2;
        }

        private static ulong HashLen17to32(ReadOnlySpan<byte> value)
        {
            ulong mul = k2 + (ulong)value.Length * 2ul;
            ulong a = ReadUnaligned64(value) * k1;
            ulong b = ReadUnaligned64(value, 8);
            ulong c = ReadUnaligned64(value, value.Length - 8) * mul;
            ulong d = ReadUnaligned64(value, value.Length - 16) * k2;

            return HashLen16(RotateRight(a + b, 43) + RotateRight(c, 30) + d, a + RotateRight(b + k2, 18) + c, mul);
        }

        private static ulong HashLen33to64(ReadOnlySpan<byte> value)
        {
            ulong mul = k2 + (ulong)value.Length * 2ul;
            ulong a = ReadUnaligned64(value) * k2;
            ulong b = ReadUnaligned64(value, 8);
            ulong c = ReadUnaligned64(value, value.Length - 24);
            ulong d = ReadUnaligned64(value, value.Length - 32);
            ulong e = ReadUnaligned64(value, 16) * k2;
            ulong f = ReadUnaligned64(value, 24) * 9;
            ulong g = ReadUnaligned64(value, value.Length - 8);
            ulong h = ReadUnaligned64(value, value.Length - 16) * mul;

            ulong u = RotateRight(a + g, 43) + (RotateRight(b, 30) + c) * 9;
            ulong v = (a + g ^ d) + f + 1;
            ulong w = BSwap64((u + v) * mul) + h;
            ulong x = RotateRight(e + f, 42) + c;
            ulong y = (BSwap64((v + w) * mul) + g) * mul;
            ulong z = e + f + c;

            a = BSwap64((x + z) * mul + y) + b;
            b = ShiftMix((z + a) * mul + d + h) * mul;
            return b + x;
        }

        private static UInt128 CityMurmur(ReadOnlySpan<byte> value, ulong seedLow, ulong seedHigh)
        {
            ulong a = seedLow, b = seedHigh, c, d;
            int len = value.Length;
            int l = len - 16;

            if (l <= 0)
            {  // len <= 16
                a = ShiftMix(a * k1) * k1;
                c = b * k1 + HashLen0to16(value);
                d = ShiftMix(a + (len >= 8 ? ReadUnaligned64(value) : c));
            }
            else
            {  // len > 16

                c = Hash128to64(ReadUnaligned64(value, len - 8) + k1, a);
                d = Hash128to64(b + (ulong)len, c + ReadUnaligned64(value, len - 16));
                a += d;

                int p = 0;
                do
                {
                    a ^= ShiftMix(ReadUnaligned64(value, p) * k1) * k1;
                    a *= k1;
                    b ^= a;
                    c ^= ShiftMix(ReadUnaligned64(value, p + 8) * k1) * k1;
                    c *= k1;
                    d ^= c;

                    p += 16;
                    l -= 16;
                } while (l > 0);

            }
            a = Hash128to64(a, c);
            b = Hash128to64(d, b);
            return new UInt128(a ^ b, Hash128to64(b, a));
        }

        private static ulong Hash128to64(ulong Low, ulong High)
        {
            const ulong kMul = 0x9ddfea08eb382d69UL;

            ulong a = (Low ^ High) * kMul;
            a ^= a >> 47;

            ulong b = (High ^ a) * kMul;
            b ^= b >> 47;
            b *= kMul;

            return b;
        }

        // Return a 16-byte hash for 48 bytes.  Quick and dirty.
        // Callers do best to use "random-looking" values for a and b.
        private static UInt128 WeakHashLen32WithSeeds(ulong w, ulong x, ulong y, ulong z, ulong a, ulong b)
        {
            a += w;
            b = RotateRight(b + a + z, 21);

            ulong c = a;
            a += x;
            a += y;

            b += RotateRight(a, 44);

            return new UInt128(b + c, a + z);
        }
        private static UInt128 WeakHashLen32WithSeeds(ReadOnlySpan<byte> value, int offset, ulong a, ulong b)
        {
            return WeakHashLen32WithSeeds(
                ReadUnaligned64(value.Slice(offset)),
                ReadUnaligned64(value.Slice((offset + 8))),
                ReadUnaligned64(value.Slice((offset + 16))),
                ReadUnaligned64(value.Slice((offset + 24))),
                a,
                b);
        }

        #endregion

        #region Helper 

        // A 32-bit to 32-bit integer hash copied from Murmur3.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint MixFinal(uint h)
        {
            h ^= h >> 16;
            h *= p1;
            h ^= h >> 13;
            h *= p2;
            h ^= h >> 16;
            return h;
        }

        // Helper from Murmur3 for combining two 32-bit values.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint Mur(uint a, uint h)
        {
            a *= c1;
            a = RotateRight(a, 17);
            a *= c2;
            h ^= a;
            h = RotateRight(h, 19);
            return h * 5 + 0xe6546b64;
        }

        private static uint RotateRight(uint value, int shift)
#if !(NETSTANDARD || NET20_OR_GREATER)
            => BitOperations.RotateRight(value, shift);
#else
            => (value >> shift) | (value << (32 - shift));
#endif

        private static ulong RotateRight(ulong value, int shift)
#if !(NETSTANDARD || NET20_OR_GREATER)
            => BitOperations.RotateRight(value, shift);
#else
            => (value >> shift) | (value << (64 - shift));
#endif

        private static uint ReadUnaligned32(ReadOnlySpan<byte> value, int index) => ReadUnaligned32(value.Slice(index));

        private static uint ReadUnaligned32(ReadOnlySpan<byte> value)
#if !NETSTANDARD
            => BitConverterX.ReadUnaligned<uint>(value);
#else
            => BitConverter.ToUInt32(value);
#endif

        private static ulong ReadUnaligned64(ReadOnlySpan<byte> value, int index) => ReadUnaligned64(value.Slice(index));

        private static ulong ReadUnaligned64(ReadOnlySpan<byte> value)
#if !NETSTANDARD
            => BitConverterX.ReadUnaligned<ulong>(value);
#else
            => BitConverter.ToUInt64(value);
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong ShiftMix(ulong val)
            => val ^ val >> 47;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Swap<T>(ref T a, ref T b)
            => (a, b) = (b, a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Permute3<T>(ref T a, ref T b, ref T c)
            => (a, b, c) = (c, a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint BSwap32(uint value)
            => BitConverterX.Swap(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong BSwap64(ulong value)
            => BitConverterX.Swap(value);

        // Murmur-inspired hashing.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong HashLen16(ulong u, ulong v, ulong mul)
        {
            ulong a = (u ^ v) * mul;
            a ^= a >> 47;
            ulong b = (v ^ a) * mul;
            b ^= b >> 47;
            b *= mul;
            return b;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong GetLow(this UInt128 value)
#if NET8_0_OR_GREATER

            => (ulong)value;
#else   
            => value.Low;
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong GetHigh(this UInt128 value)
#if NET8_0_OR_GREATER

            => (ulong)(value>> 64);
#else   
            => value.High;
#endif
        #endregion
    }
}
