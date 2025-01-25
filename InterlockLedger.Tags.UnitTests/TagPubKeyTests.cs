// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2025 InterlockLedger Network
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met
//
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
//
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// * Neither the name of the copyright holder nor the names of its
//   contributors may be used to endorse or promote products derived from
//   this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES, LOSS OF USE, DATA, OR PROFITS, OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// ******************************************************************************************************************************

namespace InterlockLedger.Tags;
[TestFixture]
public class TagPubKeyTests
{
    private static readonly byte[] _bytesToSign = [1, 2, 3];

    [TestCase("PubKey!Fl9ud3p6acZqZRfx0GF8PjmBEpwXf_PQYqPHcM6cDUU#EdDSA", Algorithm.EdDSA, typeof(TagPublicEdDSAKey))]
    public void ParsePubKey(string pubKey, Algorithm algorithm, Type type) {
        var pubkey = TagPubKey.Parse(pubKey, null);
        Assert.That(pubkey, Is.Not.Null);
        Assert.That(pubkey, Is.InstanceOf(type));
        Assert.That(pubkey, Is.AssignableTo<TagPubKey>());
        Assert.That(pubkey.Algorithm, Is.EqualTo(algorithm));
    }

    [Test]
    public void CreateEdDSAKeySerializeDeserializeSignAndVerify() {
        var parameters = EdDSAHelper.CreateNewTagEdDSAParameters();
        var pubkey = parameters.PublicKey;
        var pubkeyencodedbytes = pubkey.EncodedBytes();
        TestContext.Out.WriteLine(pubkey.ToString());
        TestContext.Out.WriteLine(pubkeyencodedbytes.AsLiteral());
        using var ms = new MemoryStream(pubkeyencodedbytes);
        var tag = ms.Decode<TagPubKey>();
        Assert.That(tag, Is.Not.Null);
        Assert.That(tag, Is.EqualTo(pubkey));
        Assert.That(tag.EncodedBytes, Is.EqualTo(pubkeyencodedbytes).AsCollection);
        var signatureBytes = EdDSAHelper.HashAndSignStream(new MemoryStream(_bytesToSign), parameters.Value);
        var signature = new TagSignature(Algorithm.EdDSA, signatureBytes);
        Assert.That(pubkey.Verify(new MemoryStream(_bytesToSign), signature), "Signature failed!");
        var keyData = new InterlockSigningKeyData(
                        [KeyPurpose.Protocol],
                        [_permission3],
                        name: "EdDSA Test Key",
                        encrypted: _bytesToSign, // fake
                        pubKey: pubkey);
        var signingKey = new EdDSAInterlockSigningKey(keyData, parameters);
        var streamSignature = signingKey.Sign(new MemoryStream(_bytesToSign));
        Assert.That(pubkey.Verify(new MemoryStream(_bytesToSign), streamSignature), "Stream signature failed!");

        var updatableKeyData = new InterlockUpdatableSigningKeyData(
                        [KeyPurpose.Protocol],
                        name: "EdDSA Updatable Test Key",
                        encrypted: _bytesToSign, // fake
                        pubKey: pubkey,
                        KeyStrength.Normal,
                        DateTimeOffset.UnixEpoch);
        var updatableKey = new EdDSAInterlockUpdatableSigningKey(updatableKeyData, parameters.EncodedBytes(), FakeTimeStamper.Instance);
        using var msout = new MemoryStream();
        updatableKey.SaveToAsync(msout).WaitResult();
        Assert.That(msout.ToArray(), Is.EquivalentTo(updatableKeyData.EncodedBytes()));
    }
    private class FakeTimeStamper : ITimeStamper
    {
        public static FakeTimeStamper Instance = new();

        private FakeTimeStamper() { }   
        public ulong Nonce => 13;
        public TagHash Session => TagHash.Empty;
        public TimeProvider Provider => TimeProvider.System;

        public void SwitchSession(SenderIdentity senderIdentity) {}
        public TimeStampStatus Validate(DateTimeOffset timeStamp, SenderIdentity senderIdentity) => TimeStampStatus.OK;
    }
    private static readonly AppPermissions _permission3 = new(3);

    [TestCase(
        Algorithm.EdDSA,
        new byte[] {
            37,
              34,
                5, 0,
                190, 243, 109, 10, 8, 14, 248, 173, 115, 242, 96, 71, 191, 173, 251, 94,
                    46, 245, 64, 18, 30, 248, 50, 172, 36, 29, 97, 226, 238, 19, 60, 61
        },
        new byte[] {
            190, 243, 109, 10, 8, 14, 248, 173, 115, 242, 96, 71, 191, 173, 251, 94,
                46, 245, 64, 18, 30, 248, 50, 172, 36, 29, 97, 226, 238, 19, 60, 61
        })
    ]
    [TestCase(Algorithm.RSA, new byte[] { 37, 8, 0, 0, 40, 4, 16, 0, 16, 0 }, new byte[] { 40, 4, 16, 0, 16, 0 })]
    public void NewTagPubKeyFromStream(Algorithm algorithm, byte[] bytes, byte[] data) {
        using var ms = new MemoryStream(bytes);
        var tag = ms.Decode<TagPubKey>();
        Assert.That(tag, Is.Not.Null);
        Assert.Multiple(() => {
            Assert.That(tag.TagId, Is.EqualTo(ILTagId.PubKey));
            Assert.That(tag.Algorithm, Is.EqualTo(algorithm));
            Assert.That(tag.Data?.Length ?? 0, Is.EqualTo(data.Length));
        });
    }

    [TestCase(Algorithm.RSA, new byte[] { 0, 0 }, ExpectedResult = new byte[] { 37, 4, 0, 0, 0, 0 })]
    [TestCase(
        Algorithm.EdDSA,
        new byte[] {
            190, 243, 109, 10, 8, 14, 248, 173, 115, 242, 96, 71, 191, 173, 251, 94,
                46, 245, 64, 18, 30, 248, 50, 172, 36, 29, 97, 226, 238, 19, 60, 61
        },
        ExpectedResult = new byte[] {
            37,
              34,
                5, 0,
                190, 243, 109, 10, 8, 14, 248, 173, 115, 242, 96, 71, 191, 173, 251, 94,
                    46, 245, 64, 18, 30, 248, 50, 172, 36, 29, 97, 226, 238, 19, 60, 61
        })]
    public byte[] SerializeTagPubKey(Algorithm algorithm, byte[] data) => new TestTagPubKey(algorithm, data).EncodedBytes();
}

public class TestTagPubKey(Algorithm algorithm, byte[] data) : TagPubKey(algorithm, data)
{
}