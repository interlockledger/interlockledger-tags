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

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace InterlockLedger.Tags
{
    [TestFixture]
    public class SignableTests
    {
        [TestCase(32ul, new byte[] { 250, 15, 65, 72, 5, 5, 1, 0, 10, 32 })]
        public void NewSignableFromStream(ulong ilint, byte[] bytes) {
            using var ms = new MemoryStream(bytes);
            var tag = ms.Decode<TestSignable.Payload>();
            Assert.AreEqual(TestSignable.FieldTagId, tag.TagId);
            Assert.AreEqual(ilint, tag.Value.SomeILInt);
            Assert.IsTrue(tag.ValueIs<ISignable>(out _));
            Assert.AreEqual(new TestSignable().FieldModel, tag.Value.FieldModel);
            Assert.AreEqual(2, tag.Value.FieldModel.SubDataFields.SafeCount());
            Assert.AreEqual(nameof(TestSignable.SomeILInt), tag.Value.FieldModel.SubDataFields.Last().Name);
        }

        [TestCase(32ul, ExpectedResult = new byte[] { 250, 15, 65, 72, 5, 5, 1, 0, 10, 32 })]
        public byte[] SerializeSignable(ulong someILInt) {
            var encodedBytes = new TestSignable(someILInt).AsPayload.EncodedBytes;
            TestContext.WriteLine(encodedBytes.AsLiteral());
            return encodedBytes;
        }
    }
}