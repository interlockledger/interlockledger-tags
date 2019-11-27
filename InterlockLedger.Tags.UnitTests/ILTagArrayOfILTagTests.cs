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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace InterlockLedger.Tags
{
    [TestFixture]
    public class ILTagArrayOfILTagTests
    {
        [TestCase(null, new byte[] { 21, 0 }, TestName = "DecodeArray a Null Array")]
        [TestCase(new byte[0], new byte[] { 21, 1, 0 }, TestName = "DecodeArray an Empty Array")]
        [TestCase(new byte[] { 1, 2, 3 }, new byte[] { 21, 10, 3, 0, 1, 1, 0, 1, 2, 0, 1, 3 }, TestName = "DecodeArray One Array with Four Explicitly Tagged Bytes")]
        public void DecodeArray(byte[] bytes, byte[] encodedBytes) {
            using var ms = new MemoryStream(encodedBytes);
            var value = ms.DecodeArray<byte, TestTagOfOneByte>(s => new TestTagOfOneByte(s.DecodeTagId(), s));
            Assert.AreEqual(bytes, value);
        }

        [TestCase(null, new byte[0], new byte[] { 21, 0 }, TestName = "Deserialize a Null Array")]
        [TestCase(new byte[0], new byte[0], new byte[] { 21, 1, 0 }, TestName = "Deserialize an Empty Array")]
        [TestCase(new byte[0], new byte[] { 0 }, new byte[] { 21, 3, 1, 16, 0 }, TestName = "Deserialize One Array with Zero Bytes")]
        [TestCase(new byte[] { 1, 2, 3, 2 }, new byte[] { 4 }, new byte[] { 21, 7, 1, 16, 4, 1, 2, 3, 2 }, TestName = "Deserialize One Array with Four Bytes")]
        [TestCase(new byte[] { 1, 2, 3, 2 }, new byte[] { 2, 4 }, new byte[] { 21, 9, 2, 16, 2, 1, 2, 16, 2, 3, 2 }, TestName = "Deserialize Two Arrays with Two Bytes")]
        [TestCase(new byte[] { 1, 2, 3, 2 }, new byte[] { 3, 4 }, new byte[] { 21, 9, 2, 16, 3, 1, 2, 3, 16, 1, 2 }, TestName = "Deserialize Two Arrays with one and Three Bytes")]
        public void DeserializeILTagILTagArray(byte[] bytes, byte[] splits, byte[] encodedBytes) {
            using var ms = new MemoryStream(encodedBytes);
            var value = ms.DecodeTagArray<ILTagByteArray>();
            var array = BuildArrayOfArrays(bytes, splits);
            CompareArrays<ILTagByteArray, byte[]>(array, value);
        }

        [TestCase(null, new byte[0], new byte[] { 21, 0 }, TestName = "Deserialize a Null Array Generic")]
        [TestCase(new byte[0], new byte[0], new byte[] { 21, 1, 0 }, TestName = "Deserialize an Empty Array Generic")]
        [TestCase(new byte[0], new byte[] { 0 }, new byte[] { 21, 3, 1, 16, 0 }, TestName = "Deserialize One Array with Zero Bytes Generic")]
        [TestCase(new byte[] { 1, 2, 3, 2 }, new byte[] { 4 }, new byte[] { 21, 7, 1, 16, 4, 1, 2, 3, 2 }, TestName = "Deserialize One Array with Four Bytes Generic")]
        [TestCase(new byte[] { 1, 2, 3, 2 }, new byte[] { 2, 4 }, new byte[] { 21, 9, 2, 16, 2, 1, 2, 16, 2, 3, 2 }, TestName = "Deserialize Two Arrays with Two Bytes Generic")]
        [TestCase(new byte[] { 1, 2, 3, 2 }, new byte[] { 3, 4 }, new byte[] { 21, 9, 2, 16, 3, 1, 2, 3, 16, 1, 2 }, TestName = "Deserialize Two Arrays with one and Three Bytes Generic")]
        public void DeserializeILTagILTagArrayGeneric(byte[] bytes, byte[] splits, byte[] encodedBytes) {
            using var ms = new MemoryStream(encodedBytes);
            var tagValue = ms.DecodeTag();
            Assert.IsInstanceOf<ILTagArrayOfILTag<ILTag>>(tagValue);
            var value = ((ILTagArrayOfILTag<ILTag>)tagValue).Value;
            var array = BuildArrayOfArrays(bytes, splits);
            CompareArrays<ILTagByteArray, byte[]>(array, value);
        }

        [Test]
        public void GuaranteeBijectiveBehaviorEmptyArray()
            => GuaranteeBijectiveBehavior(Array.Empty<ILTagBool>());

        [Test]
        public void GuaranteeBijectiveBehaviorFourElementsArray()
            => GuaranteeBijectiveBehavior(new ILTagBool[] { ILTagBool.False, ILTagBool.True, ILTagBool.True, ILTagBool.True });

        [Test]
        public void GuaranteeBijectiveBehaviorNullArray()
            => GuaranteeBijectiveBehavior(null);

        [Test]
        public void GuaranteeBijectiveBehaviorTwoElementsArray()
            => GuaranteeBijectiveBehavior(new ILTagBool[] { ILTagBool.False, ILTagBool.True });

        [TestCase(null, new byte[0], ExpectedResult = new byte[] { 21, 0 }, TestName = "Serialize a Null Array")]
        [TestCase(new byte[0], new byte[0], ExpectedResult = new byte[] { 21, 1, 0 }, TestName = "Serialize an Empty Array")]
        [TestCase(new byte[0], new byte[] { 0 }, ExpectedResult = new byte[] { 21, 3, 1, 16, 0 }, TestName = "Serialize One Array with One Byte")]
        [TestCase(new byte[] { 1, 2, 3, 2 }, new byte[] { 4 }, ExpectedResult = new byte[] { 21, 7, 1, 16, 4, 1, 2, 3, 2 }, TestName = "Serialize One Array with Four Bytes")]
        [TestCase(new byte[] { 1, 2, 3, 2 }, new byte[] { 2, 4 }, ExpectedResult = new byte[] { 21, 9, 2, 16, 2, 1, 2, 16, 2, 3, 2 }, TestName = "Serialize Two Arrays with Two Bytes")]
        [TestCase(new byte[] { 1, 2, 3, 2 }, new byte[] { 3, 4 }, ExpectedResult = new byte[] { 21, 9, 2, 16, 3, 1, 2, 3, 16, 1, 2 }, TestName = "Serialize Two Arrays with One and Three Bytes")]
        public byte[] SerializeILTagILTagArray(byte[] bytes, byte[] splits)
            => new ILTagArrayOfILTag<ILTagByteArray>(BuildArrayOfArrays(bytes, splits)).EncodedBytes;

        private static ILTagByteArray[] BuildArrayOfArrays(byte[] bytes, byte[] splits) {
            if (bytes == null)
                return null;
            var list = new List<ILTagByteArray>();
            if ((splits?.Length ?? 0) > 0) {
                var lastSplit = 0;
                foreach (var split in splits) {
                    var length = split - lastSplit;
                    var partialBytes = new byte[length];
                    Array.ConstrainedCopy(bytes, lastSplit, partialBytes, 0, length);
                    list.Add(new ILTagByteArray(partialBytes));
                    lastSplit = split;
                }
            }
            return list.ToArray();
        }

        private static void CompareArrays<T, TT>(T[] array, ILTag[] value) where T : ILTagImplicit<TT> {
            if (array == null)
                Assert.IsNull(value);
            else {
                Assert.AreEqual(array.Length, value.Length);
                for (var i = 0; i < array.Length; i++) {
                    var arrayValue = array[i].Value;
                    var valueValue = ((T)value[i]).Value;
                    Assert.AreEqual(arrayValue, valueValue);
                }
            }
        }

        private static void GuaranteeBijectiveBehavior(ILTagBool[] array) {
            var ilarray = new ILTagArrayOfILTag<ILTagBool>(array);
            var encodedBytes = ilarray.EncodedBytes;
            using var ms = new MemoryStream(encodedBytes);
            var value = ms.DecodeTagArray<ILTagBool>();
            CompareArrays<ILTagBool, bool>(array, value);
            var newEncodedBytes = new ILTagArrayOfILTag<ILTagBool>(value).EncodedBytes;
            Assert.AreEqual(encodedBytes, newEncodedBytes);
        }

        private class TestTagOfOneByte : ILTagExplicit<byte>
        {
            public TestTagOfOneByte(ulong tagId, Stream s) : base(tagId, s) {
            }

            protected override byte FromBytes(byte[] bytes) => bytes?.FirstOrDefault() ?? 0;

            protected override byte[] ToBytes() => new byte[] { Value };
        }
    }
}