using AuroraLib.Cryptography.Hash;
using AuroraLib.Interfaces;
using BenchmarkDotNet.Attributes;
using System.Runtime.InteropServices;

namespace Benchmark.Benchmarks
{
    [MemoryDiagnoser]
    public class Cryptography
    {
        [Params(typeof(Adler32), typeof(Adler64), typeof(CityHash32), typeof(CityHash64), typeof(CityHash128), typeof(Crc32), typeof(Fnv1_32), typeof(Fnv1_64))]
        public Type Algorithm;
        public IHash Instance;

        public byte[] Data;

        [Params(10, 100)]
        public int MB;

        [GlobalSetup]
        public void GlobalSetup()
        {
            Instance = (IHash)Activator.CreateInstance(Algorithm);
            int bytes = 1024 * 1024;//1mb
            Data = new byte[bytes * MB];
            Random rng = new();
            Span<int> intSpan = MemoryMarshal.Cast<byte, int>(Data);
            for (int i = 0; i < intSpan.Length; i++)
            {
                intSpan[i] = rng.Next();
            }
        }

        [Benchmark]
        public void Compute()
        {
            Instance.Compute(Data);
        }

    }
}
