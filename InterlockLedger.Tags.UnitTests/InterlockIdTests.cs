/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.IO;
using NUnit.Framework;

namespace InterlockLedger.Tags
{
#pragma warning disable CA2000 // Dispose objects before losing scope

    [TestFixture]
    public class InterlockIdTests
    {
        [Test]
        public void IsEmpty() {
            InterlockId.DefaultType = OwnerId.TypeId;
            Assert.AreEqual(true, new OwnerId("47DEQpj8HBSa-_TImW-5JCeuQeRkm5NMpJWZG3hSuFU").IsEmpty);
            Assert.AreEqual(false, new OwnerId("#SHA3_256").IsEmpty);
        }

        [Test]
        public void ResolveFromStream() {
            Assert.IsInstanceOf<OwnerId>(InterlockId.Resolve(ToStream(new byte[] { 43, 5, 1, 0, 0, 0, 0 })));
            Assert.IsInstanceOf<KeyId>(InterlockId.Resolve(ToStream(new byte[] { 43, 5, 4, 0, 0, 0, 0 })));
        }

        [Test]
        public void ResolveFromTextualRepresentation() {
            Assert.IsInstanceOf<OwnerId>(InterlockId.Resolve("Owner!AAA#SHA1"));
            Assert.IsInstanceOf<KeyId>(InterlockId.Resolve("Key!AAA#SHA1"));
        }

        [TestCase(HashAlgorithm.SHA512, new byte[] { }, ExpectedResult = new byte[] { 43, 3, 4, 2, 0 }, TestName = "SerializeKeyIdFromParts#SHA512")]
        public byte[] SerializeKeyIdFromParts(HashAlgorithm algorithm, byte[] data)
            => new KeyId(new TagHash(algorithm, data)).EncodedBytes;

        [TestCase(HashAlgorithm.SHA256, new byte[] { }, ExpectedResult = new byte[] { 43, 3, 1, 1, 0 }, TestName = "SerializeOwnerIdFromParts#SHA512")]
        public byte[] SerializeOwnerIdFromParts(HashAlgorithm algorithm, byte[] data)
            => new OwnerId(new TagHash(algorithm, data)).EncodedBytes;

        private static MemoryStream ToStream(byte[] bytes) => new MemoryStream(bytes);
    }
}