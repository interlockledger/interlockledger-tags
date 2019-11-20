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
    public class TagEncryptedTests
    {
        [TestCase(new byte[] { 42, 6, 0, 0, 16, 2, 0, 0 }, CipherAlgorithm.AES256, new byte[] { 0, 0 }, TestName = "NewTagEncryptedFromStream [0, 0]")]
        [TestCase(new byte[] { 42, 4, 0, 0, 16, 0 }, CipherAlgorithm.AES256, new byte[] { }, TestName = "NewTagEncryptedFromStream []")]
        public void NewTagEncryptedFromStream(byte[] bytes, CipherAlgorithm algorithm, byte[] data) {
            using var ms = new MemoryStream(bytes);
            var tag = ms.Decode<TagEncrypted>();
            Assert.AreEqual(ILTagId.Encrypted, tag.TagId);
            Assert.AreEqual(algorithm, tag.Algorithm);
            Assert.AreEqual(data.Length, tag.CipherData?.Length ?? 0);
        }

        [TestCase(CipherAlgorithm.AES256, new byte[] { 0, 0 }, ExpectedResult = new byte[] { 42, 6, 0, 0, 16, 2, 0, 0 }, TestName = "SerializeTagEncrypted [0, 0]")]
        [TestCase(CipherAlgorithm.AES256, new byte[] { }, ExpectedResult = new byte[] { 42, 4, 0, 0, 16, 0 }, TestName = "SerializeTagEncrypted []")]
        public byte[] SerializeTagEncrypted(CipherAlgorithm algorithm, byte[] data) => new TagEncrypted(algorithm, data).EncodedBytes;
    }
}