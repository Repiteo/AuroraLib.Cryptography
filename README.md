# AuroraLib.Cryptography
Supported mainly older hash algorithms.

[Nuget Package](https://www.nuget.org/packages/AuroraLib.Cryptography)

## Supported hashes

| Hash           | Description                                                                |
|----------------|----------------------------------------------------------------------------|
|Adler32         | 32-bit Adler implementation.                                               |
|Adler64         | 64-bit Adler implementation.                                               |
|CityHash32      | [google](https://github.com/google/cityhash) 32-bit CityHash implementation.                                     |
|CityHash64      | [google](https://github.com/google/cityhash) 64-bit CityHash implementation.                                     |
|CityHash128     | [google](https://github.com/google/cityhash) 128-bit CityHash implementation.                                    |
|Crc32           | Fully customizable Crc32 algorithm                                         |
|Fnv1 32         | 32-bit Fnv1 and Fnv1a implementation.                                       |
|Fnv1 64         | 64-bit Fnv1 and Fnv1a implementation.                                       |
|MurmurHash3 32  | 32-bit MurmurHash3 implementation from [HashDepot](https://github.com/ssg/HashDepot).                          |
|MurmurHash3 128 | 128-bit MurmurHash3 implementation from [HashDepot](https://github.com/ssg/HashDepot).                         |
|XXHash32        | 32-bit XXHash implementation from [HashDepot](https://github.com/ssg/HashDepot).                               |
|XXHash64        | 64-bit XXHash implementation from [HashDepot](https://github.com/ssg/HashDepot).                               |

## How To Use

Generate a hash of a string.
``` csharp
	string testString = "Test String To Hash";
	Crc32 crc32 = new();
	crc32.Compute(testString);
	uint result = crc32.Value;
```

Generate a hash of a span.
``` csharp
	ReadOnlySpan<int> testSpan = new int[] { 75849, -246875, 24856, 0, -24867, 9844534 };
	Crc32 crc32 = new();
	crc32.Compute(testSpan);
	uint result = crc32.Value;
```


## Benchmarks

[Cryptography](https://github.com/Venomalia/AuroraLib.Cryptography/blob/main/Benchmark/Benchmarks/Cryptography.cs)

 Method  | Algorithm       | MB  | Mean       | Error     | StdDev    | Allocated |
|-------- |---------------- |---- |-----------:|----------:|----------:|----------:|
| Compute | Adler32         | 10  |   5.860 ms | 0.0986 ms | 0.0923 ms |       4 B |
| Compute | Adler32         | 100 |  58.041 ms | 0.6680 ms | 0.5922 ms |     132 B |
| Compute | Adler64         | 10  |  22.877 ms | 0.1193 ms | 0.0996 ms |      17 B |
| Compute | Adler64         | 100 | 229.422 ms | 2.5328 ms | 2.2453 ms |    1816 B |
| Compute | CityHash32      | 10  |   6.133 ms | 0.1182 ms | 0.1106 ms |       5 B |
| Compute | CityHash32      | 100 |  61.145 ms | 0.6002 ms | 0.5012 ms |      68 B |
| Compute | CityHash64      | 10  |   4.120 ms | 0.0761 ms | 0.0635 ms |       4 B |
| Compute | CityHash64      | 100 |  42.357 ms | 0.8319 ms | 0.9580 ms |      42 B |
| Compute | CityHash128     | 10  |   4.197 ms | 0.0631 ms | 0.0590 ms |       4 B |
| Compute | CityHash128     | 100 |  42.589 ms | 0.7923 ms | 0.7411 ms |      42 B |
| Compute | Crc32           | 10  |  46.714 ms | 0.3884 ms | 0.3633 ms |      49 B |
| Compute | Crc32           | 100 | 462.799 ms | 3.2204 ms | 2.8548 ms |    1184 B |
| Compute | Fnv1_32         | 10  |  31.317 ms | 0.1278 ms | 0.1133 ms |      18 B |
| Compute | Fnv1_32         | 100 | 314.880 ms | 4.6365 ms | 4.3370 ms |     992 B |
| Compute | Fnv1_64         | 10  |  31.280 ms | 0.1188 ms | 0.0992 ms |      18 B |
| Compute | Fnv1_64         | 100 | 312.195 ms | 0.9702 ms | 0.7575 ms |     992 B |
| Compute | MurmurHash3_32  | 10  |   3.719 ms | 0.0401 ms | 0.0335 ms |       2 B |
| Compute | MurmurHash3_32  | 100 |  36.852 ms | 0.0752 ms | 0.0587 ms |      39 B |
| Compute | MurmurHash3_128 | 10  |   1.873 ms | 0.0287 ms | 0.0255 ms |     402 B |
| Compute | MurmurHash3_128 | 100 |  18.506 ms | 0.0465 ms | 0.0363 ms |    4017 B |
| Compute | XXHash32        | 10  |   2.245 ms | 0.0294 ms | 0.0275 ms |       2 B |
| Compute | XXHash32        | 100 |  22.432 ms | 0.2348 ms | 0.2196 ms |      18 B |
| Compute | XXHash64        | 10  |   2.401 ms | 0.0323 ms | 0.0302 ms |       2 B |
| Compute | XXHash64        | 100 |  23.837 ms | 0.3630 ms | 0.3218 ms |      18 B |