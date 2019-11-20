/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using System.IO;
using NUnit.Framework;

namespace InterlockLedger.Tags
{
#pragma warning disable CA1062 // Validate arguments of public methods

    [TestFixture]
    public class TagHashTests
    {
        [TestCase(new byte[] { 39, 4, 4, 0, 0, 0 }, HashAlgorithm.SHA3_512, new byte[] { 0, 0 })]
        [TestCase(new byte[] { 39, 4, 0, 0, 0, 0 }, HashAlgorithm.SHA1, new byte[] { 0, 0 })]
        [TestCase(new byte[] { 39, 2, 3, 0 }, HashAlgorithm.SHA3_256, new byte[] { })]
        public void NewTagHashFromStream(byte[] bytes, HashAlgorithm algorithm, byte[] data) {
            using var ms = new MemoryStream(bytes);
            var tag = ms.Decode<TagHash>();
            Assert.AreEqual(ILTagId.Hash, tag.TagId);
            Assert.AreEqual(algorithm, tag.Algorithm);
            Assert.AreEqual(data.Length, tag.Data?.Length ?? 0);
            Assert.AreEqual(data, tag.Data ?? Array.Empty<byte>());
        }

        [TestCase("AAA#SHA3_512", HashAlgorithm.SHA3_512, new byte[] { 0, 0 })]
        [TestCase("AAA#SHA1", HashAlgorithm.SHA1, new byte[] { 0, 0 })]
        [TestCase("#SHA3_256", HashAlgorithm.SHA3_256, new byte[] { })]
        [TestCase("#SHA256", HashAlgorithm.SHA256, new byte[] { })]
        public void NewTagHashFromString(string textual, HashAlgorithm algorithm, byte[] data) {
            var tag = new TagHash(textual);
            Assert.AreEqual(ILTagId.Hash, tag.TagId);
            Assert.AreEqual(algorithm, tag.Algorithm);
            Assert.AreEqual(data.Length, tag.Data?.Length ?? 0);
            Assert.AreEqual(data, tag.Data);
        }

        [TestCase(HashAlgorithm.SHA3_512, new byte[] { 0, 0 }, ExpectedResult = new byte[] { 39, 4, 4, 0, 0, 0 })]
        [TestCase(HashAlgorithm.SHA1, new byte[] { 0, 0 }, ExpectedResult = new byte[] { 39, 4, 0, 0, 0, 0 })]
        [TestCase(HashAlgorithm.SHA3_256, new byte[] { }, ExpectedResult = new byte[] { 39, 2, 3, 0 })]
        public byte[] SerializeTagHash(HashAlgorithm algorithm, byte[] data) => new TagHash(algorithm, data).EncodedBytes;

        [TestCase(HashAlgorithm.SHA3_512, new byte[] { 0, 0 }, ExpectedResult = "AAA#SHA3_512")]
        [TestCase(HashAlgorithm.SHA1, new byte[] { 0, 0 }, ExpectedResult = "AAA#SHA1")]
        [TestCase(HashAlgorithm.SHA3_256, new byte[] { }, ExpectedResult = "#SHA3_256")]
        [TestCase(HashAlgorithm.SHA256, new byte[] { }, ExpectedResult = "#SHA256")]
        public string StringizeTagHash(HashAlgorithm algorithm, byte[] data) => new TagHash(algorithm, data).ToString();
    }
}