using System;

namespace AuroraLib.Cryptography.Hash
{
    /// <summary>
    /// Predefined CRC-32 algorithms.
    /// </summary>
    public enum Crc32Algorithm
    {
        Default,
        BZIP2,
        JAMCRC,
        MPEG2,
        POSIX,
        SATA,
        XFER,
        CRC32C,
        CRC32D,
        CRC32Q,
    }

    internal static class Crc32Info
    {
        public static uint Polynomial(this Crc32Algorithm algorithm)
        {
            switch (algorithm)
            {
                case Crc32Algorithm.Default:
                case Crc32Algorithm.BZIP2:
                case Crc32Algorithm.JAMCRC:
                case Crc32Algorithm.MPEG2:
                case Crc32Algorithm.POSIX:
                case Crc32Algorithm.SATA:
                    return 0x04C11DB7;
                case Crc32Algorithm.XFER:
                    return 0x000000AF;
                case Crc32Algorithm.CRC32C:
                    return 0x1EDC6F41;
                case Crc32Algorithm.CRC32D:
                    return 0xA833982B;
                case Crc32Algorithm.CRC32Q:
                    return 0x814141AB;
                default:
                    throw new NotImplementedException();
            }
        }

        public static bool Reverse(this Crc32Algorithm algorithm)
        {
            switch (algorithm)
            {
                case Crc32Algorithm.BZIP2:
                case Crc32Algorithm.MPEG2:
                case Crc32Algorithm.POSIX:
                case Crc32Algorithm.SATA:
                case Crc32Algorithm.XFER:
                case Crc32Algorithm.CRC32Q:
                    return true;
                case Crc32Algorithm.Default:
                case Crc32Algorithm.JAMCRC:
                case Crc32Algorithm.CRC32C:
                case Crc32Algorithm.CRC32D:
                    return false;
                default:
                    throw new NotImplementedException();
            }
        }

        public static uint Initial(this Crc32Algorithm algorithm)
        {
            switch (algorithm)
            {
                case Crc32Algorithm.Default:
                case Crc32Algorithm.BZIP2:
                case Crc32Algorithm.JAMCRC:
                case Crc32Algorithm.MPEG2:
                case Crc32Algorithm.CRC32C:
                case Crc32Algorithm.CRC32D:
                    return 0xFFFFFFFF;
                case Crc32Algorithm.POSIX:
                case Crc32Algorithm.XFER:
                case Crc32Algorithm.CRC32Q:
                    return 0x00000000;
                case Crc32Algorithm.SATA:
                    return 0x52325032;
                default:
                    throw new NotImplementedException();
            }
        }

        public static uint XorOut(this Crc32Algorithm algorithm)
        {
            switch (algorithm)
            {
                case Crc32Algorithm.Default:
                case Crc32Algorithm.BZIP2:
                case Crc32Algorithm.POSIX:
                case Crc32Algorithm.CRC32C:
                case Crc32Algorithm.CRC32D:
                    return 0xFFFFFFFF;
                case Crc32Algorithm.JAMCRC:
                case Crc32Algorithm.MPEG2:
                case Crc32Algorithm.SATA:
                case Crc32Algorithm.XFER:
                case Crc32Algorithm.CRC32Q:
                    return 0x00000000;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
