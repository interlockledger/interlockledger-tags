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
public class ILTagDictionaryTests
{
    public static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    [TestCase(new string[0], new byte[0], new byte[0], new byte[] { 30, 1, 0 }, TestName = "Deserialize_an_Empty_Dictionary")]
    [TestCase(new string[] { "@" }, new byte[0], new byte[] { 0 }, new byte[] { 30, 6, 1, 17, 1, 64, 16, 0 }, TestName = "Deserialize_One_Dictionary_with_Zero_Bytes")]
    [TestCase(new string[] { "A" }, new byte[] { 1, 2, 3, 2 }, new byte[] { 4 }, new byte[] { 30, 10, 1, 17, 1, 65, 16, 4, 1, 2, 3, 2 }, TestName = "Deserialize_One_Dictionary_with_Four_Bytes")]
    [TestCase(new string[] { "B", "C" }, new byte[] { 1, 2, 3, 2 }, new byte[] { 2, 4 }, new byte[] { 30, 15, 2, 17, 1, 66, 16, 2, 1, 2, 17, 1, 67, 16, 2, 3, 2 }, TestName = "Deserialize_One_Dictionary_with_Two_Bytes")]
    [TestCase(new string[] { "D", "E" }, new byte[] { 1, 2, 3, 2 }, new byte[] { 3, 4 }, new byte[] { 30, 15, 2, 17, 1, 68, 16, 3, 1, 2, 3, 17, 1, 69, 16, 1, 2 }, TestName = "Deserialize_One_Dictionary_with_one_and_Three_Bytes")]
    public void DeserializeILTagDictionaryOfILTagArray(string[] keys, byte[] bytes, byte[] splits, byte[] encodedBytes) {
        using var ms = new MemoryStream(encodedBytes);
        var value = ms.DecodeDictionary<ILTagByteArray>();
        var dict = BuildDictionary(keys, bytes, splits);
        CompareDicts<ILTagByteArray, byte[]>(dict, value);
    }

    [TestCase(new string[0], new byte[0], new byte[0], new byte[] { 30, 1, 0 }, TestName = "Deserialize_an_Empty_Dictionary_Generic")]
    [TestCase(new string[] { "@" }, new byte[0], new byte[] { 0 }, new byte[] { 30, 6, 1, 17, 1, 64, 16, 0 }, TestName = "Deserialize_One_Dictionary_with_Zero_Bytes_Generic")]
    [TestCase(new string[] { "A" }, new byte[] { 1, 2, 3, 2 }, new byte[] { 4 }, new byte[] { 30, 10, 1, 17, 1, 65, 16, 4, 1, 2, 3, 2 }, TestName = "Deserialize_One_Dictionary_with_Four_Bytes_Generic")]
    [TestCase(new string[] { "B", "C" }, new byte[] { 1, 2, 3, 2 }, new byte[] { 2, 4 }, new byte[] { 30, 15, 2, 17, 1, 66, 16, 2, 1, 2, 17, 1, 67, 16, 2, 3, 2 }, TestName = "Deserialize_One_Dictionary_with_Two_Bytes_Generic")]
    [TestCase(new string[] { "D", "E" }, new byte[] { 1, 2, 3, 2 }, new byte[] { 3, 4 }, new byte[] { 30, 15, 2, 17, 1, 68, 16, 3, 1, 2, 3, 17, 1, 69, 16, 1, 2 }, TestName = "Deserialize_One_Dictionary_with_one_and_Three_Bytes_Generic")]
    public void DeserializeILTagDictionaryOfILTagArrayGeneric(string[] keys, byte[] bytes, byte[] splits, byte[] encodedBytes) {
        using var ms = new MemoryStream(encodedBytes);
        var tagValue = ms.DecodeTag();
        Assert.IsInstanceOf<ILTagDictionary<ILTag>>(tagValue);
        var value = ((ILTagDictionary<ILTag>)tagValue).Value;
        var dict = BuildDictionary(keys, bytes, splits);
        CompareSimilarDicts<ILTagByteArray, ILTag>(dict, value);
    }

    [TestCase(new string[] { "B", "C" }, new string[] { "b", "c" }, new byte[] { 31, 13, 2, 17, 1, 66, 17, 1, 98, 17, 1, 67, 17, 1, 99 }, TestName = "Deserialize_One_String_Dictionary_with_two_strings")]
    [TestCase(new string[] { "D", "E" }, new string[] { "Demo", "" }, new byte[] { 31, 15, 2, 17, 1, 68, 17, 4, 68, 101, 109, 111, 17, 1, 69, 17, 0 }, TestName = "Deserialize_One_String_Dictionary_with_one_string_and_one_empty_string")]
    [TestCase(new string[] { "F", "G" }, new string[] { "Demo", null }, new byte[] { 31, 14, 2, 17, 1, 70, 17, 4, 68, 101, 109, 111, 17, 1, 71, 0 }, TestName = "Deserialize_One_String_Dictionary_with_one_string_and_a_null")]
    public void DeserializeILTagStringDictionary(string[] keys, string[] values, byte[] encodedBytes) {
        using var ms = new MemoryStream(encodedBytes);
        var value = ms.DecodeStringDictionary();
        var dict = BuildStringDictionary(keys, values);
        CompareStringDicts(dict, value);
    }

    [Test]
    public void GuaranteeBijectiveBehaviorEmptyArray()
        => GuaranteeBijectiveBehavior(new Dictionary<string, ILTagBool>());

    [TestCase(new string[] { "B", "C" }, new string[] { "b", "c" }, TestName = "Guarantee_Bijective_Behavior_for_One_String_Dictionary_with_two_strings")]
    [TestCase(new string[] { "D", "E" }, new string[] { "Demo", null }, TestName = "Guarantee_Bijective_Behavior_for_One_String_Dictionary_with_one_string_and_a_null")]
    [TestCase(new string[] { "F", "G" }, new string[] { "Demo", "" }, TestName = "Guarantee_Bijective_Behavior_for_One_String_Dictionary_with_one_non_empty_string_and_one_empty_string")]
    public void GuaranteeBijectiveBehaviorForStringTest(string[] keys, string[] values)
        => GuaranteeBijectiveBehaviorForString(BuildStringDictionary(keys, values));

    [Test]
    public void GuaranteeBijectiveBehaviorFourElementsArray()
        => GuaranteeBijectiveBehavior(new Dictionary<string, ILTagBool> { ["C"] = ILTagBool.False, ["D"] = ILTagBool.True, ["E"] = ILTagBool.True, ["F"] = ILTagBool.True });

    [Test]
    public void GuaranteeBijectiveBehaviorTwoElementsArray()
        => GuaranteeBijectiveBehavior(new Dictionary<string, ILTagBool> { ["A"] = ILTagBool.False, ["B"] = ILTagBool.True });

    [Test]
    public void JsonSerializationDictionary() {
        var dict = new ILTagDictionary<ILTag>(("Key1", ILTagBool.False), ("Key2", new ILTagString("Value2")));
        var jsonModel = dict.AsJson;
        var json = JsonSerializer.Serialize(jsonModel, JsonOptions);
        TestContext.WriteLine(json);
        var parsedJson = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        var backDict = TagProvider.DeserializeFromJson(ILTagId.Dictionary, parsedJson);
        Assert.IsTrue(dict.Equals(backDict));
    }

    [Test]
    public void JsonSerializationStringDictionary() {
        var dict = new ILTagStringDictionary(("Key1", "Value1"), ("Key2", "Value2"));
        var jsonModel = dict.AsJson;
        var json = JsonSerializer.Serialize(jsonModel, JsonOptions);
        TestContext.WriteLine(json);
        var parsedJson = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        var backDict = TagProvider.DeserializeFromJson(ILTagId.StringDictionary, parsedJson);
        Assert.IsTrue(dict.Equals(backDict));
    }

    [TestCase(new string[0], null, new byte[0], ExpectedResult = new byte[] { 30, 0 }, TestName = "Serialize_a_Null_Dictionary")]
    [TestCase(new string[0], new byte[0], new byte[0], ExpectedResult = new byte[] { 30, 1, 0 }, TestName = "Serialize_an_Empty_Dictionary")]
    [TestCase(new string[] { "@" }, new byte[0], new byte[] { 0 }, ExpectedResult = new byte[] { 30, 6, 1, 17, 1, 64, 16, 0 }, TestName = "Serialize_One_Dictionary_with_One_Byte")]
    [TestCase(new string[] { "A" }, new byte[] { 1, 2, 3, 2 }, new byte[] { 4 }, ExpectedResult = new byte[] { 30, 10, 1, 17, 1, 65, 16, 4, 1, 2, 3, 2 }, TestName = "Serialize_One_Dictionary_with_Four_Bytes")]
    [TestCase(new string[] { "B", "C" }, new byte[] { 1, 2, 3, 2 }, new byte[] { 2, 4 }, ExpectedResult = new byte[] { 30, 15, 2, 17, 1, 66, 16, 2, 1, 2, 17, 1, 67, 16, 2, 3, 2 }, TestName = "Serialize_One_Dictionary_with_Two_Bytes")]
    [TestCase(new string[] { "D", "E" }, new byte[] { 1, 2, 3, 2 }, new byte[] { 3, 4 }, ExpectedResult = new byte[] { 30, 15, 2, 17, 1, 68, 16, 3, 1, 2, 3, 17, 1, 69, 16, 1, 2 }, TestName = "Serialize_One_Dictionary_with_One_and_Three_Bytes")]
    public byte[] SerializeILTagDictionaryOfILTagArray(string[] keys, byte[] bytes, byte[] splits)
        => new ILTagDictionary<ILTagByteArray>(BuildDictionary(keys, bytes, splits)).EncodedBytes;

    [TestCase(new string[] { "B", "C" }, new string[] { "b", "c" }, ExpectedResult = new byte[] { 31, 13, 2, 17, 1, 66, 17, 1, 98, 17, 1, 67, 17, 1, 99 }, TestName = "Serialize_One_String_Dictionary_with_two_strings")]
    [TestCase(new string[] { "D", "E" }, new string[] { "Demo", null }, ExpectedResult = new byte[] { 31, 14, 2, 17, 1, 68, 17, 4, 68, 101, 109, 111, 17, 1, 69, 0 }, TestName = "Serialize_One_String_Dictionary_with_one_string_and_a_null")]
    [TestCase(new string[] { "F", "G" }, new string[] { "Demo", "" }, ExpectedResult = new byte[] { 31, 15, 2, 17, 1, 70, 17, 4, 68, 101, 109, 111, 17, 1, 71, 17, 0 }, TestName = "Serialize_One_String_Dictionary_with_one_non_empty_string_and_one_empty_string")]
    public byte[] SerializeILTagStringDictionary(string[] keys, string[] values)
        => new ILTagStringDictionary(BuildStringDictionary(keys, values)).EncodedBytes;

    private static Dictionary<string, ILTagByteArray> BuildDictionary(string[] keys, byte[] bytes, byte[] splits) {
        if (bytes == null)
            return null;
        var dict = new Dictionary<string, ILTagByteArray>();
        if ((splits?.Length ?? 0) > 0) {
            if (splits.Length != keys.Length)
                throw new InvalidDataException("Non matching keys and splits arrays");
            var lastSplit = 0;
            var currentKey = 0;
            foreach (var split in splits) {
                var length = split - lastSplit;
                var partialBytes = new byte[length];
                Array.ConstrainedCopy(bytes, lastSplit, partialBytes, 0, length);
                dict.Add(keys[currentKey], new ILTagByteArray(partialBytes));
                lastSplit = split;
                currentKey++;
            }
        }
        return dict;
    }

    private static Dictionary<string, string> BuildStringDictionary(string[] keys, string[] values) {
        var dict = new Dictionary<string, string>();
        for (int i = 0; i < keys.Length; i++) {
            dict.Add(keys[i], values[i]);
        }
        return dict;
    }

    private static void CompareDicts<T, TT>(Dictionary<string, T> dict, Dictionary<string, T> value) where T : ILTagOf<TT> {
        if (dict == null)
            Assert.IsNull(value);
        else {
            Assert.AreEqual(dict.SafeCount(), value.SafeCount());
            foreach (var key in dict.Keys) {
                var dictValue = dict[key].Value;
                var valueValue = value[key].Value;
                Assert.AreEqual(dictValue, valueValue);
            }
        }
    }

    private static void CompareSimilarDicts<T, TT>(Dictionary<string, T> dict, Dictionary<string, TT> value) where T : ILTag where TT : ILTag {
        if (dict == null)
            Assert.IsNull(value);
        else {
            Assert.AreEqual(dict.SafeCount(), value.SafeCount());
            foreach (var key in dict.Keys) {
                var dictValue = dict[key];
                var valueValue = value[key];
                Assert.AreEqual(dictValue.TagId, valueValue.TagId);
                Assert.AreEqual(dictValue.EncodedBytes, valueValue.EncodedBytes);
            }
        }
    }

    private static void CompareStringDicts(Dictionary<string, string> dict, Dictionary<string, string> value) {
        if (dict == null)
            Assert.IsNull(value);
        else {
            Assert.AreEqual(dict.SafeCount(), value.SafeCount());
            foreach (var key in dict.Keys) {
                var dictValue = dict[key];
                var valueValue = value[key];
                Assert.AreEqual(dictValue, valueValue);
            }
        }
    }

    private static void GuaranteeBijectiveBehavior(Dictionary<string, ILTagBool> dict) {
        var ilarray = new ILTagDictionary<ILTagBool>(dict);
        var encodedBytes = ilarray.EncodedBytes;
        using var ms = new MemoryStream(encodedBytes);
        var value = ms.DecodeDictionary<ILTagBool>();
        CompareDicts<ILTagBool, bool>(dict, value);
        var newEncodedBytes = new ILTagDictionary<ILTagBool>(value).EncodedBytes;
        Assert.AreEqual(encodedBytes, newEncodedBytes);
    }

    private static void GuaranteeBijectiveBehaviorForString(Dictionary<string, string> dict) {
        var ilarray = new ILTagStringDictionary(dict);
        var encodedBytes = ilarray.EncodedBytes;
        using var ms = new MemoryStream(encodedBytes);
        var value = ms.DecodeStringDictionary();
        CompareStringDicts(dict, value);
        var newEncodedBytes = new ILTagStringDictionary(value).EncodedBytes;
        Assert.AreEqual(encodedBytes, newEncodedBytes);
    }
}