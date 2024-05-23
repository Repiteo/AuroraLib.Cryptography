﻿using AuroraLib.Cryptography.Hash;
using AuroraLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace CoreUnitTest
{
    [TestClass]
    public class CryptographyTest
    {
        public const string TestData1 = "Hash Test String, 123";
        public const string TestData2 = "Lorem ipsum dolor sit amet, consectetur adipisici elit, sed eiusmod tempor incidunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquid ex ea commodi consequat. Quis aute iure reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint obcaecat cupiditat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

        public static IEnumerable<object[]> HashTestData()
        {
            yield return new object[] { new Adler32(), "BE06B84F", TestData1 };
            yield return new object[] { new Adler64(), "BE060000B84F0000", TestData1 };
            yield return new object[] { new CityHash32(), "20B22194", TestData1 };
            yield return new object[] { new CityHash32(), "F8ED263B", TestData2 };
            yield return new object[] { new CityHash64(), "AA8DB73547338E34", TestData1 };
            yield return new object[] { new CityHash64(), "177FE70AFC252F2D", TestData2 };
            yield return new object[] { new CityHash128(), "D2CB74C404956ED6FE64B68B1904D2CD", TestData1 };
            yield return new object[] { new CityHash128(), "B94F0F5AB941ED1687FA623AC0BD8C67", TestData2 };
            yield return new object[] { new Fnv1_32(), "5EF94AE2", TestData1 };
            yield return new object[] { new Fnv1_32(true), "FCE30CA9", TestData1 }; // Fnv1a_32
            yield return new object[] { new Fnv1_64(), "DEA365A8B42728EE", TestData1 };
            yield return new object[] { new Fnv1_64(true), "DCE43A4A2526021C", TestData1 }; // Fnv1a_64
            yield return new object[] { new Crc32(Crc32Algorithm.Default), "340C0B54", TestData1 };
            yield return new object[] { new Crc32(Crc32Algorithm.BZIP2), "50237BB7", TestData1 };
            yield return new object[] { new Crc32(Crc32Algorithm.MPEG2), "AFDC8448", TestData1 };
            yield return new object[] { new Crc32(Crc32Algorithm.JAMCRC), "CBF3F4AB", TestData1 };
            yield return new object[] { new Crc32(Crc32Algorithm.POSIX), "2DFC2799", TestData1 };
            yield return new object[] { new Crc32(Crc32Algorithm.XFER), "A533C11A", TestData1 };
            yield return new object[] { new Crc32(Crc32Algorithm.SATA), "C21D8BFE", TestData1 };
            yield return new object[] { new Crc32(Crc32Algorithm.CRC32C), "1B63195F", TestData1 };
            yield return new object[] { new Crc32(Crc32Algorithm.CRC32D), "7AA669CE", TestData1 };
            yield return new object[] { new Crc32(Crc32Algorithm.CRC32Q), "FC9989FC", TestData1 };
        }

        [TestMethod]
        [DynamicData(nameof(HashTestData), DynamicDataSourceType.Method)]
        public void HashMatchTest(IHash algorithm, string expectedHash, string testString)
        {
            algorithm.Reset();

            algorithm.Compute(testString.AsSpan());

            string actualHash = BitConverter.ToString(algorithm.GetBytes()).Replace("-", "");
            Assert.AreEqual(expectedHash, actualHash);
        }
    }
}
