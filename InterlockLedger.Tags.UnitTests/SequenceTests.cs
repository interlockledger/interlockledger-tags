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

using System.Text.Json;

namespace InterlockLedger.Tags;
[TestFixture]
public class SequenceTests
{
    [TestCase("Test", 1024ul, new byte[] { 22, 11, 2, 17, 4, 84, 101, 115, 116, 10, 249, 3, 8 }, TestName = "Deserialize_One_Sequence_with_a_string_and_one_ilint")]
    public void DeserializeHeterogenousILTagSequence(string firstElement, ulong secondElement, byte[] encodedBytes) {
        using var ms = new MemoryStream(encodedBytes);
        var tagValue = ms.DecodeTag();
        Assert.That(tagValue, Is.InstanceOf<ILTagSequence>());
        var value = ((ILTagSequence)tagValue).Value;
        Assert.That(value, Has.Length.EqualTo(2));
        Assert.Multiple(() => {
            Assert.That(value[0].TextualRepresentation, Is.EqualTo(firstElement));
            Assert.That((value[1] as ILTagILInt)?.Value, Is.EqualTo(secondElement));
        });
    }

    [TestCase(new byte[0], new byte[0], new byte[] { 22, 1, 0 }, TestName = "Deserialize_an_Empty_Sequence")]
    [TestCase(new byte[0], new byte[] { 0 }, new byte[] { 22, 3, 1, 16, 0 }, TestName = "Deserialize_One_Sequence_with_One_Byte_Generic")]
    [TestCase(new byte[] { 1, 2, 3, 2 }, new byte[] { 4 }, new byte[] { 22, 7, 1, 16, 4, 1, 2, 3, 2 }, TestName = "Deserialize_One_Sequence_with_Four_Bytes_Generic")]
    [TestCase(new byte[] { 1, 2, 3, 2 }, new byte[] { 2, 4 }, new byte[] { 22, 9, 2, 16, 2, 1, 2, 16, 2, 3, 2 }, TestName = "Deserialize_Two_Sequences_with_Two_Bytes_Generic")]
    [TestCase(new byte[] { 1, 2, 3, 2 }, new byte[] { 3, 4 }, new byte[] { 22, 9, 2, 16, 3, 1, 2, 3, 16, 1, 2 }, TestName = "Deserialize_Two_Sequences_with_one_and_Three_Bytes_Generic")]
    public void DeserializeILTagSequence(byte[] bytes, byte[] splits, byte[] encodedBytes) {
        using var ms = new MemoryStream(encodedBytes);
        var tagValue = ms.DecodeTag();
        Assert.That(tagValue, Is.InstanceOf<ILTagSequence>());
        var value = ((ILTagSequence)tagValue).Value;

        var array = BuildArrayOfArrays(bytes, splits);
        if (array == null) {
            Assert.That(value, Is.EqualTo(null));
        } else {
            Assert.That(value, Has.Length.EqualTo(array.Length));
            for (var i = 0; i < array.Length; i++) {
                var arrayValue = array[i].Value;
                var valueValue = (value[i] as ILTagByteArray)?.Value;
                if (arrayValue.Length == 0 && valueValue == null)
                    Assert.Pass();
                Assert.That(valueValue, Is.EqualTo(arrayValue));
            }
        }
    }

    [Test]
    public void JsonSerialization() {
        var seq = new ILTagSequence(new ILTagString("JsonTest"), new ILTagILInt(13), ILTagBool.False);
        var jsonSeq = JsonSerializer.Serialize(seq, ILTagDictionaryTests.JsonOptions);
        TestContext.WriteLine(jsonSeq);
        Assert.That(jsonSeq.RegexReplace("\r\n","\n"), Is.EqualTo(/*lang=json,strict*/ """
            {
              "Content": [
                {
                  "TagId": 17,
                  "Content": "JsonTest"
                },
                {
                  "TagId": 10,
                  "Content": 13
                },
                {
                  "TagId": 1,
                  "Content": false
                }
              ],
              "TagId": 22
            }
            """));
    }

    [TestCase("Test", 1024ul, ExpectedResult = new byte[] { 22, 11, 2, 17, 4, 84, 101, 115, 116, 10, 249, 3, 8 }, TestName = "Serialize_One_Sequence_with_a_string_and_one_ilint")]
    public byte[] SerializeHeterogenousILTagSequence(string firstElement, ulong secondElement)
        => new ILTagSequence(new ILTagString(firstElement), new ILTagILInt(secondElement)).EncodedBytes;

    [TestCase(new byte[0], new byte[0], ExpectedResult = new byte[] { 22, 1, 0 }, TestName = "Serialize_an_Empty_Sequence")]
    [TestCase(new byte[0], new byte[] { 0 }, ExpectedResult = new byte[] { 22, 3, 1, 16, 0 }, TestName = "Serialize_One_Sequence_with_One_Byte")]
    [TestCase(new byte[] { 1, 2, 3, 2 }, new byte[] { 4 }, ExpectedResult = new byte[] { 22, 7, 1, 16, 4, 1, 2, 3, 2 }, TestName = "Serialize_One_Sequence_with_Four_Bytes")]
    [TestCase(new byte[] { 1, 2, 3, 2 }, new byte[] { 2, 4 }, ExpectedResult = new byte[] { 22, 9, 2, 16, 2, 1, 2, 16, 2, 3, 2 }, TestName = "Serialize_Two_Sequences_with_Two_Bytes")]
    [TestCase(new byte[] { 1, 2, 3, 2 }, new byte[] { 3, 4 }, ExpectedResult = new byte[] { 22, 9, 2, 16, 3, 1, 2, 3, 16, 1, 2 }, TestName = "Serialize_Two_Sequences_with_One_and_Three_Bytes")]
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