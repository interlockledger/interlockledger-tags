/******************************************************************************************************************************
 
Copyright (c) 2018-2020 InterlockLedger Network
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this
  list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice,
  this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution.

* Neither the name of the copyright holder nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

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