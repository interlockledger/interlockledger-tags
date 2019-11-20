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
    public class TagHmacTests
    {
        [TestCase(new byte[] { 47, 4, 4, 0, 0, 0 }, HashAlgorithm.SHA3_512, new byte[] { 0, 0 })]
        [TestCase(new byte[] { 47, 4, 0, 0, 0, 0 }, HashAlgorithm.SHA1, new byte[] { 0, 0 })]
        [TestCase(new byte[] { 47, 2, 3, 0 }, HashAlgorithm.SHA3_256, new byte[] { })]
        public void NewTagHmacFromStream(byte[] bytes, HashAlgorithm algorithm, byte[] data) {
            using var ms = new MemoryStream(bytes);
            var tag = ms.Decode<TagHmac>();
            Assert.AreEqual(ILTagId.Hmac, tag.TagId);
            Assert.AreEqual(algorithm, tag.Algorithm);
            Assert.AreEqual(data.Length, tag.Data?.Length ?? 0);
            Assert.AreEqual(data, tag.Data ?? Array.Empty<byte>());
        }

        [TestCase("AAA#HMAC-SHA3_512", HashAlgorithm.SHA3_512, new byte[] { 0, 0 })]
        [TestCase("AAA#HMAC-SHA1", HashAlgorithm.SHA1, new byte[] { 0, 0 })]
        [TestCase("#HMAC-SHA3_256", HashAlgorithm.SHA3_256, new byte[] { })]
        [TestCase("#HMAC-SHA256", HashAlgorithm.SHA256, new byte[] { })]
        public void NewTagHmacFromString(string textual, HashAlgorithm algorithm, byte[] data) {
            var tag = new TagHmac(textual);
            Assert.AreEqual(ILTagId.Hmac, tag.TagId);
            Assert.AreEqual(algorithm, tag.Algorithm);
            Assert.AreEqual(data.Length, tag.Data?.Length ?? 0);
            Assert.AreEqual(data, tag.Data);
        }

        [TestCase(HashAlgorithm.SHA3_512, new byte[] { 0, 0 }, ExpectedResult = new byte[] { 47, 4, 4, 0, 0, 0 })]
        [TestCase(HashAlgorithm.SHA1, new byte[] { 0, 0 }, ExpectedResult = new byte[] { 47, 4, 0, 0, 0, 0 })]
        [TestCase(HashAlgorithm.SHA3_256, new byte[] { }, ExpectedResult = new byte[] { 47, 2, 3, 0 })]
        public byte[] SerializeTagHmac(HashAlgorithm algorithm, byte[] data) => new TagHmac(algorithm, data).EncodedBytes;

        [TestCase(HashAlgorithm.SHA3_512, new byte[] { 0, 0 }, ExpectedResult = "AAA#HMAC-SHA3_512")]
        [TestCase(HashAlgorithm.SHA1, new byte[] { 0, 0 }, ExpectedResult = "AAA#HMAC-SHA1")]
        [TestCase(HashAlgorithm.SHA3_256, new byte[] { }, ExpectedResult = "#HMAC-SHA3_256")]
        [TestCase(HashAlgorithm.SHA256, new byte[] { }, ExpectedResult = "#HMAC-SHA256")]
        public string StringizeTagHmac(HashAlgorithm algorithm, byte[] data) => new TagHmac(algorithm, data).ToString();
    }
}