/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.IO;
using NUnit.Framework;

namespace InterlockLedger.Tags
{
#pragma warning disable CA1062 // Validate arguments of public methods

    [TestFixture]
    public class TagPubKeyTests
    {
        [TestCase(new byte[] { 37, 4, 4, 0, 0, 0 }, Algorithm.EcDSA, new byte[] { 0, 0 })]
        [TestCase(new byte[] { 37, 8, 0, 0, 40, 4, 16, 0, 16, 0 }, Algorithm.RSA, new byte[] { 40, 4, 16, 0, 16, 0 })]
        [TestCase(new byte[] { 37, 2, 3, 0 }, Algorithm.ElGamal, new byte[] { })]
        public void NewTagPubKeyFromStream(byte[] bytes, Algorithm algorithm, byte[] data) {
            using var ms = new MemoryStream(bytes);
            var tag = ms.Decode<TagPubKey>();
            Assert.AreEqual(ILTagId.PubKey, tag.TagId);
            Assert.AreEqual(algorithm, tag.Algorithm);
            Assert.AreEqual(data.Length, tag.Data?.Length ?? 0);
        }

        [TestCase(Algorithm.EcDSA, new byte[] { 0, 0 }, ExpectedResult = new byte[] { 37, 4, 4, 0, 0, 0 })]
        [TestCase(Algorithm.RSA, new byte[] { 0, 0 }, ExpectedResult = new byte[] { 37, 4, 0, 0, 0, 0 })]
        [TestCase(Algorithm.ElGamal, new byte[] { }, ExpectedResult = new byte[] { 37, 2, 3, 0 })]
        public byte[] SerializeTagPubKey(Algorithm algorithm, byte[] data) => new TestTagPubKey(algorithm, data).EncodedBytes;
    }

    public class TestTagPubKey : TagPubKey
    {
        public TestTagPubKey(Algorithm algorithm, byte[] data) : base(algorithm, data) {
        }
    }
}