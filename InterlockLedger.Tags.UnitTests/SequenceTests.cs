/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;

namespace InterlockLedger.Tags
{
    [TestFixture]
    public class SequenceTests
    {
        [TestCase("Test", 1024ul, new byte[] { 22, 11, 2, 17, 4, 84, 101, 115, 116, 10, 249, 3, 8 }, TestName = "Deserialize One Sequence with a string and one ilint")]
        public void DeserializeHeterogenousILTagSequence(string firstElement, ulong secondElement, byte[] encodedBytes) {
            using (var ms = new MemoryStream(encodedBytes)) {
                var tagValue = ms.DecodeTag();
                Assert.IsInstanceOf<ILTagSequence>(tagValue);
                var value = ((ILTagSequence)tagValue).Value;

                Assert.AreEqual(2, value.Length);
                Assert.AreEqual(firstElement, value[0].AsString());
                Assert.AreEqual(secondElement, (value[1] as ILTagILInt).Value);
            }
        }

        [TestCase(null, new byte[0], new byte[] { 22, 0 }, TestName = "Deserialize a Null Sequence")]
        [TestCase(new byte[0], new byte[0], new byte[] { 22, 1, 0 }, TestName = "Deserialize an Empty Sequence")]
        [TestCase(new byte[0], new byte[] { 0 }, new byte[] { 22, 3, 1, 16, 0 }, TestName = "Deserialize One Sequence with One Byte Generic")]
        [TestCase(new byte[] { 1, 2, 3, 2 }, new byte[] { 4 }, new byte[] { 22, 7, 1, 16, 4, 1, 2, 3, 2 }, TestName = "Deserialize One Sequence with Four Bytes Generic")]
        [TestCase(new byte[] { 1, 2, 3, 2 }, new byte[] { 2, 4 }, new byte[] { 22, 9, 2, 16, 2, 1, 2, 16, 2, 3, 2 }, TestName = "Deserialize Two Sequences with Two Bytes Generic")]
        [TestCase(new byte[] { 1, 2, 3, 2 }, new byte[] { 3, 4 }, new byte[] { 22, 9, 2, 16, 3, 1, 2, 3, 16, 1, 2 }, TestName = "Deserialize Two Sequences with one and Three Bytes Generic")]
        public void DeserializeILTagSequence(byte[] bytes, byte[] splits, byte[] encodedBytes) {
            using (var ms = new MemoryStream(encodedBytes)) {
                var tagValue = ms.DecodeTag();
                Assert.IsInstanceOf<ILTagSequence>(tagValue);
                var value = ((ILTagSequence)tagValue).Value;

                var array = BuildArrayOfArrays(bytes, splits);
                if (array == null) {
                    Assert.IsTrue(value == null);
                } else {
                    Assert.AreEqual(array.Length, value.Length);
                    for (var i = 0; i < array.Length; i++) {
                        var arrayValue = array[i].Value;
                        var valueValue = (value[i] as ILTagByteArray)?.Value;
                        if (arrayValue.Length == 0 && valueValue == null)
                            Assert.Pass();
                        Assert.AreEqual(arrayValue, valueValue);
                    }
                }
            }
        }

        [Test]
        public void JsonSerialization() {
            var seq = new ILTagSequence(new ILTagString("JsonTest"), new ILTagILInt(13), ILTagBool.False);
            var jsonModel = seq.AsJson;
            var json = JsonConvert.SerializeObject(jsonModel, Formatting.Indented);
            TestContext.WriteLine(json);
            var parsedJson = JsonConvert.DeserializeObject<List<object>>(json);
            var backSeq = ILTag.DeserializeFromJson(ILTagId.Sequence, parsedJson);
            Assert.IsTrue(seq.Equals(backSeq));
        }

        [TestCase("Test", 1024ul, ExpectedResult = new byte[] { 22, 11, 2, 17, 4, 84, 101, 115, 116, 10, 249, 3, 8 }, TestName = "Serialize One Sequence with a string and one ilint")]
        public byte[] SerializeHeterogenousILTagSequence(string firstElement, ulong secondElement)
            => new ILTagSequence(new ILTagString(firstElement), new ILTagILInt(secondElement)).EncodedBytes;

        [TestCase(null, new byte[0], ExpectedResult = new byte[] { 22, 0 }, TestName = "Serialize a Null Sequence")]
        [TestCase(new byte[0], new byte[0], ExpectedResult = new byte[] { 22, 1, 0 }, TestName = "Serialize an Empty Sequence")]
        [TestCase(new byte[0], new byte[] { 0 }, ExpectedResult = new byte[] { 22, 3, 1, 16, 0 }, TestName = "Serialize One Sequence with One Byte")]
        [TestCase(new byte[] { 1, 2, 3, 2 }, new byte[] { 4 }, ExpectedResult = new byte[] { 22, 7, 1, 16, 4, 1, 2, 3, 2 }, TestName = "Serialize One Sequence with Four Bytes")]
        [TestCase(new byte[] { 1, 2, 3, 2 }, new byte[] { 2, 4 }, ExpectedResult = new byte[] { 22, 9, 2, 16, 2, 1, 2, 16, 2, 3, 2 }, TestName = "Serialize Two Sequences with Two Bytes")]
        [TestCase(new byte[] { 1, 2, 3, 2 }, new byte[] { 3, 4 }, ExpectedResult = new byte[] { 22, 9, 2, 16, 3, 1, 2, 3, 16, 1, 2 }, TestName = "Serialize Two Sequences with One and Three Bytes")]
        public byte[] SerializeILTagSequence(byte[] bytes, byte[] splits)
            => new ILTagSequence(BuildArrayOfArrays(bytes, splits)).EncodedBytes;

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
    }
}