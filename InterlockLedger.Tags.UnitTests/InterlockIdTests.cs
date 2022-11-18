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
public class InterlockIdTests
{
    [Test]
    public void IsEmpty() {
        InterlockId.DefaultType = OwnerId.TypeId;
        Assert.AreEqual(true, InterlockId.FromString("47DEQpj8HBSa-_TImW-5JCeuQeRkm5NMpJWZG3hSuFU").IsEmpty);
        Assert.AreEqual(false, InterlockId.FromString("#SHA3_256").IsEmpty);
    }

    [Test]
    public void ResolveFromStream() {
        Assert.IsInstanceOf<OwnerId>(InterlockId.Resolve(ToStream(new byte[] { 43, 5, 1, 0, 0, 0, 0 })));
        Assert.IsInstanceOf<KeyId>(InterlockId.Resolve(ToStream(new byte[] { 43, 5, 4, 0, 0, 0, 0 })));
    }

    [Test]
    public void ResolveFromTextualRepresentation() {
        Assert.IsInstanceOf<OwnerId>(InterlockId.FromString("Owner!AAA#SHA1"));
        Assert.IsInstanceOf<KeyId>(InterlockId.FromString("Key!AAA#SHA1"));
    }

    [TestCase(HashAlgorithm.SHA512, new byte[] { }, ExpectedResult = new byte[] { 43, 3, 4, 2, 0 }, TestName = "SerializeKeyIdFromParts#SHA512")]
    public byte[] SerializeKeyIdFromParts(HashAlgorithm algorithm, byte[] data)
        => new KeyId(new TagHash(algorithm, data)).EncodedBytes;

    [TestCase(HashAlgorithm.SHA256, new byte[] { }, ExpectedResult = new byte[] { 43, 3, 1, 1, 0 }, TestName = "SerializeOwnerIdFromParts#SHA512")]
    public byte[] SerializeOwnerIdFromParts(HashAlgorithm algorithm, byte[] data)
        => new OwnerId(new TagHash(algorithm, data)).EncodedBytes;

    private static MemoryStream ToStream(byte[] bytes) => new(bytes);
}