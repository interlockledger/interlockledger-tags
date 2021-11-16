// ******************************************************************************************************************************
//
// Copyright (c) 2018-2021 InterlockLedger Network
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

using NUnit.Framework;

namespace InterlockLedger.Tags;
[TestFixture]
public class TagPubKeyTests
{
    [TestCase(KeyStrength.Normal)]
    [TestCase(KeyStrength.Strong)]
    [TestCase(KeyStrength.ExtraStrong)]
    [TestCase(KeyStrength.MegaStrong)]
    [TestCase(KeyStrength.SuperStrong)]
    [TestCase(KeyStrength.HyperStrong)]
    [TestCase(KeyStrength.UltraStrong)]
    public void CreateECKeySerializeDeserializeSignAndVerify(KeyStrength keyStrength) {
        var parameters = ECDsaHelper.CreateNewECDsaParameters(keyStrength);
        var key = new TagPubECKey(parameters);
        var bytes = key.EncodedBytes;
        TestContext.WriteLine(bytes.AsLiteral());
        using var ms = new MemoryStream(bytes);
        var tag = ms.Decode<TagPubKey>();
        Assert.NotNull(tag);
        Assert.AreEqual(key, tag);
        CollectionAssert.AreEqual(bytes, tag.EncodedBytes);
        var signatureBytes = ECDsaHelper.HashAndSign(bytes, parameters.Parameters, parameters.HashAlgorithm.ToName());
        var signature = new TagSignature(Algorithm.EcDSA, signatureBytes);
        Assert.IsTrue(key.Verify(bytes, signature), "Signature failed!");
    }

    [TestCase(
        new byte[] {
                37, 242,
                    4, 0,
                    58, 238,
                        5, 1, 0,
                        10, 0,
                        10, 1,
                        16, 24,
                            30, 85, 30, 243, 193, 131, 193, 248, 41, 244, 83, 206,
                            154, 79, 111, 148, 222, 162, 141, 172, 165, 74, 178, 177,
                        16, 24,
                            21, 52, 66, 9, 109, 160, 78, 48, 249, 74, 228, 82,
                        200, 57, 244, 102, 239, 23, 51, 162, 217, 165, 252, 229, 0,
                        17, 176,
                            123, 13, 10, 32, 32, 34, 79, 105, 100, 34, 58, 32, 123, 13, 10, 32, 32, 32, 32, 34, 86, 97, 108, 117,
                            101, 34, 58, 32, 110, 117, 108, 108, 44, 13, 10, 32, 32, 32, 32, 34, 70, 114, 105, 101, 110, 100, 108,
                            121, 78, 97, 109, 101, 34, 58, 32, 34, 98, 114, 97, 105, 110, 112, 111, 111, 108, 80, 49, 57, 50, 114,
                            49, 34, 13, 10, 32, 32, 125, 44, 13, 10, 32, 32, 34, 73, 115, 80, 114, 105, 109, 101, 34, 58, 32, 102,
                            97, 108, 115, 101, 44, 13, 10, 32, 32, 34, 73, 115, 67, 104, 97, 114, 97, 99, 116, 101, 114, 105, 115,
                            116, 105, 99, 50, 34, 58, 32, 102, 97, 108, 115, 101, 44, 13, 10, 32, 32, 34, 73, 115, 69, 120, 112,
                            108, 105, 99, 105, 116, 34, 58, 32, 102, 97, 108, 115, 101, 44, 13, 10, 32, 32, 34, 73, 115, 78, 97,
                            109, 101, 100, 34, 58, 32, 116, 114, 117, 101, 13, 10, 125

        },
        Algorithm.EcDSA,
        new byte[] {
                58, 238,
                    5, 1, 0,
                    10, 0,
                    10, 1,
                    16, 24,
                        30, 85, 30, 243, 193, 131, 193, 248, 41, 244, 83, 206,
                        154, 79, 111, 148, 222, 162, 141, 172, 165, 74, 178, 177,
                    16, 24,
                        21, 52, 66, 9, 109, 160, 78, 48, 249, 74, 228, 82,
                    200, 57, 244, 102, 239, 23, 51, 162, 217, 165, 252, 229, 0,
                    17, 176,
                        123, 13, 10, 32, 32, 34, 79, 105, 100, 34, 58, 32, 123, 13, 10, 32, 32, 32, 32, 34, 86, 97, 108, 117,
                        101, 34, 58, 32, 110, 117, 108, 108, 44, 13, 10, 32, 32, 32, 32, 34, 70, 114, 105, 101, 110, 100, 108,
                        121, 78, 97, 109, 101, 34, 58, 32, 34, 98, 114, 97, 105, 110, 112, 111, 111, 108, 80, 49, 57, 50, 114,
                        49, 34, 13, 10, 32, 32, 125, 44, 13, 10, 32, 32, 34, 73, 115, 80, 114, 105, 109, 101, 34, 58, 32, 102,
                        97, 108, 115, 101, 44, 13, 10, 32, 32, 34, 73, 115, 67, 104, 97, 114, 97, 99, 116, 101, 114, 105, 115,
                        116, 105, 99, 50, 34, 58, 32, 102, 97, 108, 115, 101, 44, 13, 10, 32, 32, 34, 73, 115, 69, 120, 112,
                        108, 105, 99, 105, 116, 34, 58, 32, 102, 97, 108, 115, 101, 44, 13, 10, 32, 32, 34, 73, 115, 78, 97,
                        109, 101, 100, 34, 58, 32, 116, 114, 117, 101, 13, 10, 125
         })]
    [TestCase(new byte[] { 37, 8, 0, 0, 40, 4, 16, 0, 16, 0 }, Algorithm.RSA, new byte[] { 40, 4, 16, 0, 16, 0 })]
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