// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2023 InterlockLedger Network
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



public class InterlockIdPlus : InterlockId
{
    public const byte Chain = 0;
    public const byte Owner = 1;
    public const byte Node = 2;
    public const byte Role = 3;
    public const byte Key = 4;
    public const byte Contract = 5;

    public static void Register() {
        RegisterResolver(Chain, nameof(Chain), parts => new InterlockIdPlus(parts));
        RegisterResolver(Node, nameof(Node), parts => new InterlockIdPlus(parts));
        RegisterResolver(Role, nameof(Role), parts => new InterlockIdPlus(parts));
        RegisterResolver(Contract, nameof(Contract), parts => new InterlockIdPlus(parts));
        DefaultType = Chain;
    }

    protected InterlockIdPlus(Parts parts) : base(parts) {
    }
}

[TestFixture]
public class InterlockIdTests
{
    [OneTimeSetUp]
    public void Setup() => InterlockIdPlus.Register();

    [Test]
    public void IsEmpty() {
        Assert.That(InterlockId.Build("47DEQpj8HBSa-_TImW-5JCeuQeRkm5NMpJWZG3hSuFU").IsEmpty, Is.EqualTo(true));
        Assert.That(InterlockId.Build("#SHA3_256").IsEmpty, Is.EqualTo(false));
    }

    [Test]
    public void ResolveFromStream() {
        Assert.That(InterlockId.Resolve<OwnerId>(ToStream(new byte[] { 43, 5, 1, 0, 0, 0, 0 })), Is.InstanceOf<OwnerId>());
        Assert.That(InterlockId.Resolve<KeyId>(ToStream(new byte[] { 43, 5, 4, 0, 0, 0, 0 })), Is.InstanceOf<KeyId>());
    }

    [Test]
    public void CompareFromTextualRepresentation() {
        var a = InterlockId.Build("Key!AAA#SHA1");
        var b = InterlockId.Build("Owner!AAA#SHA1");
        var c = (KeyId)a;
#pragma warning disable NUnit2010, NUnit2043 // Use EqualConstraint for better assertion messages in case of failure
        Assert.That(a > b);
        Assert.That(a >= b);
        Assert.That(a != b);
        Assert.That(b < a);
        Assert.That(b <= a);
#pragma warning restore NUnit2010, NUnit2043 // Use EqualConstraint for better assertion messages in case of failure
        Assert.That(a, Is.EqualTo(c));
        Assert.That(b, Is.Not.EqualTo(a));
        Assert.That(b.GetHashCode(), Is.Not.EqualTo(a.GetHashCode()));
        Assert.That(c.GetHashCode(), Is.EqualTo(a.GetHashCode()));
    }

    [Test]
    public void ResolveFromTextualRepresentation() {
        Assert.That(InterlockId.Build("Owner!AAA#SHA1"), Is.InstanceOf<OwnerId>());
        Assert.That(InterlockId.Build("Key!AAA#SHA1"), Is.InstanceOf<KeyId>());
    }

    [TestCase(HashAlgorithm.SHA512, new byte[] { }, ExpectedResult = new byte[] { 43, 3, 4, 2, 0 }, TestName = "SerializeKeyIdFromParts#SHA512")]
    public byte[] SerializeKeyIdFromParts(HashAlgorithm algorithm, byte[] data)
        => new KeyId(new TagHash(algorithm, data)).EncodedBytes;

    [TestCase(HashAlgorithm.SHA256, new byte[] { }, ExpectedResult = new byte[] { 43, 3, 1, 1, 0 }, TestName = "SerializeOwnerIdFromParts#SHA512")]
    public byte[] SerializeOwnerIdFromParts(HashAlgorithm algorithm, byte[] data)
        => new OwnerId(new TagHash(algorithm, data)).EncodedBytes;

    [TestCase(new byte[] { 43, 5, 0, 0, 0, 0, 0 }, InterlockIdPlus.Chain, HashAlgorithm.SHA1, new byte[] { 0, 0 }, "AAA#SHA1", "Chain!AAA#SHA1", TestName = "NewChainIdFromShortStringWithSHA1Suffix")]
    [TestCase(new byte[] { 43, 5, 0, 1, 0, 0, 0 }, InterlockIdPlus.Chain, HashAlgorithm.SHA256, new byte[] { 0, 0 }, "AAA", "Chain!AAA#SHA256", TestName = "NewChainIdFromShortStringWithSHA256Suffix")]
    [TestCase(new byte[] { 43, 5, 0, 2, 0, 0, 0 }, InterlockIdPlus.Chain, HashAlgorithm.SHA512, new byte[] { 0, 0 }, "AAA#SHA512", "Chain!AAA#SHA512", TestName = "NewChainIdFromShortStringWithSHA512Suffix")]
    [TestCase(new byte[] { 43, 5, 0, 3, 0, 0, 0 }, InterlockIdPlus.Chain, HashAlgorithm.SHA3_256, new byte[] { 0, 0 }, "AAA#SHA3_256", "Chain!AAA#SHA3_256", TestName = "NewChainIdFromShortStringWithSHA3_256Suffix")]
    [TestCase(new byte[] { 43, 5, 0, 4, 0, 0, 0 }, InterlockIdPlus.Chain, HashAlgorithm.SHA3_512, new byte[] { 0, 0 }, "AAA#SHA3_512", "Chain!AAA#SHA3_512", TestName = "NewChainIdFromShortStringWithSHA3_512Suffix")]
    [TestCase(new byte[] { 43, 5, 0, 255, 255, 0, 0 }, InterlockIdPlus.Chain, HashAlgorithm.Copy, new byte[] { 0, 0 }, "AAA#Copy", "Chain!AAA#Copy", TestName = "NewChainIdFromShortStringWithCopySuffix")]
    [TestCase(new byte[] { 43, 3, 0, 3, 0 }, InterlockIdPlus.Chain, HashAlgorithm.SHA3_256, new byte[] { }, "#SHA3_256", "Chain!#SHA3_256", TestName = "NewChainIdFromStringWithJustTheSuffix")]
    [TestCase(new byte[] { 43, 35, 0, 1, 0, 227, 176, 196, 66, 152, 252, 28, 20, 154, 251, 244, 200, 153, 111, 185, 36, 39, 174, 65, 228, 100, 155, 147, 76, 164, 149, 153, 27, 120, 82, 184, 85 },
InterlockIdPlus.Chain, HashAlgorithm.SHA256, new byte[] { 227, 176, 196, 66, 152, 252, 28, 20, 154, 251, 244, 200, 153, 111, 185, 36, 39, 174, 65, 228, 100, 155, 147, 76, 164, 149, 153, 27, 120, 82, 184, 85 },
"47DEQpj8HBSa-_TImW-5JCeuQeRkm5NMpJWZG3hSuFU", "Chain!47DEQpj8HBSa-_TImW-5JCeuQeRkm5NMpJWZG3hSuFU#SHA256", TestName = "NewChainIdFromLargeStringWithoutPrefixOrSuffix")]
    [TestCase(new byte[] { 43, 5, 0, 0, 0, 0, 0 }, InterlockIdPlus.Chain, HashAlgorithm.SHA1, new byte[] { 0, 0 }, "Chain!AAA#SHA1", "Chain!AAA#SHA1", TestName = "NewChainIdFromStringWithPrefix")]
    [TestCase(new byte[] { 43, 5, 1, 0, 0, 0, 0 }, InterlockIdPlus.Owner, HashAlgorithm.SHA1, new byte[] { 0, 0 }, "Owner!AAA#SHA1", "Owner!AAA#SHA1", TestName = "NewOwnerIdFromString")]
    [TestCase(new byte[] { 43, 5, 2, 0, 0, 0, 0 }, InterlockIdPlus.Node, HashAlgorithm.SHA1, new byte[] { 0, 0 }, "Node!AAA#SHA1", "Node!AAA#SHA1", TestName = "NewNodeIdFromString")]
    [TestCase(new byte[] { 43, 5, 3, 0, 0, 0, 0 }, InterlockIdPlus.Role, HashAlgorithm.SHA1, new byte[] { 0, 0 }, "Role!AAA#SHA1", "Role!AAA#SHA1", TestName = "NewRoleIdFromString")]
    [TestCase(new byte[] { 43, 5, 4, 0, 0, 0, 0 }, InterlockIdPlus.Key, HashAlgorithm.SHA1, new byte[] { 0, 0 }, "Key!AAA#SHA1", "Key!AAA#SHA1", TestName = "NewKeyIdFromString")]
    public void NewInterlockIdFromString(byte[] bytes, byte type, HashAlgorithm algorithm, byte[] data, string textualRepresentation, string fullTextual) {
        _ = data.Required();
        var id = InterlockId.Build(textualRepresentation);
        Assert.Multiple(() => {
            Assert.That(id.TagId, Is.EqualTo(ILTagId.InterlockId));
            Assert.That(id.Algorithm, Is.EqualTo(algorithm));
            Assert.That(id.Data?.Length ?? 0, Is.EqualTo(data.Length));
            Assert.That(id.Data, Is.EqualTo(data));
            Assert.That(id.Type, Is.EqualTo(type));
            Assert.That(id.ToString(), Is.EqualTo(RemoveOptionalParts(textualRepresentation)));
            Assert.That(id.ToFullString(), Is.EqualTo(fullTextual));
            Assert.That(id.EncodedBytes, Is.EqualTo(bytes));
        });
        using var ms = new MemoryStream(bytes);
        var idFromBytes = InterlockId.Resolve<InterlockId>(ms);
        Assert.Multiple(() => {
            Assert.That(id, Is.EqualTo(idFromBytes));
            int hashCode = id.GetHashCode();
            int expectedHashCode = idFromBytes.GetHashCode();
            Assert.That(hashCode, Is.EqualTo(expectedHashCode), "GetHashCode()");
            Assert.That(idFromBytes, Is.EqualTo(id)); //operators
            Assert.That(idFromBytes, Is.EqualTo(id)); //operators
        });
    }

    private static string RemoveOptionalParts(string id) {
        if (id is null)
            return null;
        if (id.StartsWith("Chain!", StringComparison.OrdinalIgnoreCase))
            id = id[6..];
        if (id.EndsWith("#SHA256", StringComparison.OrdinalIgnoreCase))
            id = id[0..^7];
        return id;
    }


    private static MemoryStream ToStream(byte[] bytes) => new(bytes);
}