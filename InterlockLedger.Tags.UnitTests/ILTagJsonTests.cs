/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text.Json;
using NUnit.Framework;

namespace InterlockLedger.Tags
{
    [TestFixture]
    public class ILTagJsonTests
    {
        [TestCase("true", TestName = "BoolFromJsonTrue", ExpectedResult = true)]
        [TestCase("false", TestName = "BoolFromJsonFalse", ExpectedResult = false)]
        public bool BoolFrom(string json) {
            var payload = json.DeserializeJson();
            var tag = ILTag.DeserializeFromJson(ILTagId.Bool, payload);
            Assert.IsInstanceOf<ILTagBool>(tag);
            Assert.AreEqual(payload, tag.AsJson);
            return ((ILTagBool)tag).Value;
        }

        [Test]
        public void DataFieldToFrom() {
            var datafield = new DataField {
                Name = "TestEnumeration",
                TagId = 2,
                Enumeration = new Dictionary<ulong, EnumerationDetails> {
                    [1ul] = new EnumerationDetails("Name1", "Descr1"),
                    [3ul] = new EnumerationDetails("Name3", null)
                }
            };
            var json = JsonSerializer.Serialize(datafield, _jsonOptions);
            TestContext.WriteLine(json);
            var payload = JsonSerializer.Deserialize<DataField>(json, _jsonOptions);
            Assert.IsInstanceOf<DataField>(payload);
            Assert.AreEqual(payload, datafield);
        }

        [TestCase("0", TestName = "Int8FromJson0", ExpectedResult = (sbyte)0)]
        [TestCase("127", TestName = "Int8FromJson127", ExpectedResult = (sbyte)127)]
        [TestCase("-127", TestName = "Int8FromJsonMinus127", ExpectedResult = (sbyte)-127)]
        public sbyte Int8From(string json) {
            var payload = json.DeserializeJson();
            var tag = ILTag.DeserializeFromJson(ILTagId.Int8, payload);
            Assert.IsInstanceOf<ILTagInt8>(tag);
            Assert.AreEqual(payload, tag.AsJson);
            return ((ILTagInt8)tag).Value;
        }

        [TestCase("255", TestName = "Int8FromJson255Throws")]
        [TestCase("-255", TestName = "Int8FromJsonMinus255Throws")]
        [TestCase("1024", TestName = "Int8FromJson1024Throws")]
        public void Int8FromThrows(string json) {
            var payload = json.DeserializeJson();
            Assert.Throws<OverflowException>(() => ILTag.DeserializeFromJson(ILTagId.Int8, payload));
        }

        [TestCase("", TestName = "NullFromJsonStringEmpty")]
        [TestCase("null", TestName = "NullFromJsonStringLiteralNull")]
        public void NullFrom(string json) {
            var payload = json.DeserializeJson();
            var tag = ILTag.DeserializeFromJson(ILTagId.Null, payload);
            Assert.IsInstanceOf<ILTagNull>(tag);
            Assert.AreEqual(payload, tag.AsJson);
            var tag2 = ILTag.DeserializeFromJson(ILTagId.Version, payload);
            Assert.IsInstanceOf<ILTagNull>(tag2);
        }

        [TestCase("\"Some String\"", TestName = "TagStringFromSomeString")]
        [TestCase("\"\"", TestName = "TagStringFromEmpty")]
        public void StringFrom(string json) {
            var payload = json.DeserializeJson();
            var tag = ILTag.DeserializeFromJson(ILTagId.String, payload);
            Assert.AreEqual(payload, tag.AsJson);
            Assert.IsInstanceOf<ILTagString>(tag);
        }

        [TestCase("true")]
        public void ThrowFromUnsupportedTag(string json) {
            var payload = json.DeserializeJson();
            Assert.Throws<ArgumentException>(() => ILTag.DeserializeFromJson(ulong.MaxValue, payload));
            Assert.Throws<InvalidOperationException>(() => ILTag.DeserializeFromJson(ILTagId.BigInteger, payload));
        }

        [TestCase("0", TestName = "UInt8FromJson0", ExpectedResult = (byte)0)]
        [TestCase("127", TestName = "UInt8FromJson127", ExpectedResult = (byte)127)]
        [TestCase("255", TestName = "UInt8FromJson255", ExpectedResult = (byte)255)]
        public byte UInt8From(string json) {
            var payload = json.DeserializeJson();
            var tag = ILTag.DeserializeFromJson(ILTagId.UInt8, payload);
            Assert.IsInstanceOf<ILTagUInt8>(tag);
            Assert.AreEqual(payload, tag.AsJson);
            return ((ILTagUInt8)tag).Value;
        }

        [TestCase("-127", TestName = "UInt8FromJsonMinus127Throws")]
        [TestCase("-255", TestName = "UInt8FromJsonMinus255Throws")]
        [TestCase("1024", TestName = "UInt8FromJson1024Throws")]
        public void UInt8FromThrows(string json) {
            var payload = json.DeserializeJson();
            Assert.Throws<OverflowException>(() => ILTag.DeserializeFromJson(ILTagId.UInt8, payload));
        }

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { AllowTrailingCommas = true, WriteIndented = true };
    }
}