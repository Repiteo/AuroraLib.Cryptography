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

| Method  | Algorithm       | MB  | Mean       | Error     | StdDev     | Allocated |
|-------- |---------------- |---- |-----------:|----------:|-----------:|----------:|
| Compute | Adler32         | 10  |   9.798 ms | 0.1936 ms |  0.4928 ms |       8 B |
| Compute | Adler32         | 100 |  97.185 ms | 1.9237 ms |  4.6091 ms |      91 B |
| Compute | Adler64         | 10  |  25.118 ms | 0.5007 ms |  0.8770 ms |      17 B |
| Compute | Adler64         | 100 | 249.914 ms | 4.9268 ms |  9.3737 ms |     181 B |
| Compute | CityHash128     | 10  |   3.273 ms | 0.0652 ms |  0.1142 ms |       2 B |
| Compute | CityHash128     | 100 |  33.362 ms | 0.6664 ms |  1.8576 ms |      36 B |
| Compute | CityHash32      | 10  |   4.366 ms | 0.0869 ms |  0.1735 ms |       4 B |
| Compute | CityHash32      | 100 |  43.509 ms | 0.8633 ms |  1.4184 ms |      21 B |
| Compute | CityHash64      | 10  |   3.075 ms | 0.0603 ms |  0.1040 ms |       2 B |
| Compute | CityHash64      | 100 |  29.980 ms | 0.5974 ms |  1.0145 ms |      17 B |
| Compute | Crc32           | 10  |  48.440 ms | 0.9656 ms |  1.8834 ms |      54 B |
| Compute | Crc32           | 100 | 473.423 ms | 7.7627 ms | 10.0938 ms |     544 B |
| Compute | Fnv1_32         | 10  |  33.020 ms | 0.6596 ms |  1.2060 ms |      34 B |
| Compute | Fnv1_32         | 100 | 327.978 ms | 6.3849 ms |  7.8413 ms |    6132 B |
| Compute | Fnv1_64         | 10  |  33.356 ms | 0.6622 ms |  1.6120 ms |      34 B |
| Compute | Fnv1_64         | 100 | 327.279 ms | 5.6028 ms |  5.7536 ms |     272 B |