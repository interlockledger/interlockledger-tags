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
    public class TagSignatureTests
    {
        [TestCase(new byte[] { 38, 4, 4, 0, 0, 0 }, Algorithm.EcDSA, new byte[] { 0, 0 })]
        [TestCase(new byte[] { 38, 4, 0, 0, 0, 0 }, Algorithm.RSA, new byte[] { 0, 0 })]
        [TestCase(new byte[] { 38, 2, 3, 0 }, Algorithm.ElGamal, new byte[] { })]
        public void NewTagSignatureFromStream(byte[] bytes, Algorithm algorithm, byte[] data) {
            using var ms = new MemoryStream(bytes);
            var tag = ms.Decode<TagSignature>();
            Assert.AreEqual(ILTagId.Signature, tag.TagId);
            Assert.AreEqual(algorithm, tag.Algorithm);
            Assert.AreEqual(data.Length, tag.Data?.Length ?? 0);
        }

        [TestCase(Algorithm.EcDSA, new byte[] { 0, 0 }, ExpectedResult = new byte[] { 38, 4, 4, 0, 0, 0 })]
        [TestCase(Algorithm.RSA, new byte[] { 0, 0 }, ExpectedResult = new byte[] { 38, 4, 0, 0, 0, 0 })]
        [TestCase(Algorithm.ElGamal, new byte[] { }, ExpectedResult = new byte[] { 38, 2, 3, 0 })]
        public byte[] SerializeTagSignature(Algorithm algorithm, byte[] data) => new TagSignature(algorithm, data).EncodedBytes;
    }
}