/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.IO;
using System.Security.Cryptography;
using NUnit.Framework;

namespace InterlockLedger.Tags
{
    [TestFixture]
    public class TagRSAParametersTests
    {
        [TestCase(new byte[] { 41, 24, 16, 1, 6, 16, 1, 5, 16, 1, 7, 16, 1, 8, 16, 1, 2, 16, 1, 3, 16, 1, 4, 16, 1, 1 })]
        public void NewTagRSAParametersFromStream(byte[] bytes) {
            using var ms = new MemoryStream(bytes);
            var tag = ms.Decode<TagRSAParameters>();
            Assert.AreEqual(ILTagId.RSAParameters, tag.TagId);
            Assert.AreEqual(new byte[] { 1 }, tag.Value.D);
            Assert.AreEqual(new byte[] { 2 }, tag.Value.DP);
            Assert.AreEqual(new byte[] { 3 }, tag.Value.DQ);
            Assert.AreEqual(new byte[] { 4 }, tag.Value.InverseQ);
            Assert.AreEqual(new byte[] { 5 }, tag.Value.Exponent);
            Assert.AreEqual(new byte[] { 6 }, tag.Value.Modulus);
            Assert.AreEqual(new byte[] { 7 }, tag.Value.P);
            Assert.AreEqual(new byte[] { 8 }, tag.Value.Q);
        }

        [TestCase(ExpectedResult = new byte[] { 41, 24, 16, 1, 6, 16, 1, 5, 16, 1, 7, 16, 1, 8, 16, 1, 2, 16, 1, 3, 16, 1, 4, 16, 1, 1 })]
        public byte[] SerializeTagRSAParameters() =>
            new TagRSAParameters(new RSAParameters {
                D = new byte[] { 1 },
                DP = new byte[] { 2 },
                DQ = new byte[] { 3 },
                InverseQ = new byte[] { 4 },
                Exponent = new byte[] { 5 },
                Modulus = new byte[] { 6 },
                P = new byte[] { 7 },
                Q = new byte[] { 8 },
            }).EncodedBytes;
    }
}