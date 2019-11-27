/******************************************************************************************************************************

Copyright (c) 2018-2019 InterlockLedger Network
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
using NUnit.Framework;

namespace InterlockLedger.Tags
{
    [TestFixture]
    public class TagHelpersTests
    {
        [Test]
        public void DecodedTest() {
            var tag = new ILTagILInt(14);
            var decoded = tag.EncodedBytes.Decoded();
            Assert.AreEqual(tag, decoded);
        }

        [Test]
        public void ToBytesHelperTest() {
            var tag = new ILTagILInt(23);
            var bytes = TagHelpers.ToBytesHelper((s) => s.EncodeILInt(23));
            Assert.IsNotNull(bytes);
            Assert.That(tag.EncodedBytes, Is.EquivalentTo(bytes));
            var e = Assert.Throws<ArgumentNullException>(() => TagHelpers.ToBytesHelper(null));
            Assert.AreEqual("serialize", e.ParamName);
        }

        [Test]
        public void ToTagArrayFromTest() {
            var data = new string[] { "1", "2", "3", "5", "7", "11" };
            var tagArray = data.ToTagArrayFrom(v => new ILTagString(v));
            Assert.IsNotNull(tagArray);
            Assert.AreEqual(data.Length, tagArray.Value.Length);
            Assert.That(tagArray.GetValues<string>(), Is.EquivalentTo(data));
            var e = Assert.Throws<ArgumentNullException>(() => data.ToTagArrayFrom(null));
            Assert.AreEqual("convert", e.ParamName);
        }

        [Test]
        public void ToTagArrayTest() {
            var data = new ulong[] { 1, 2, 3, 5, 7, 11 };
            var tagArray = data.ToTagArray(v => new ILTagILInt(v));
            Assert.IsNotNull(tagArray);
            Assert.AreEqual(data.Length, tagArray.Value.Length);
            Assert.That(tagArray.GetValues<ulong>(), Is.EquivalentTo(data));
            var e = Assert.Throws<ArgumentNullException>(() => data.ToTagArray<ulong, ILTagILInt>(null));
            Assert.AreEqual("convert", e.ParamName);
        }
    }
}