/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using Newtonsoft.Json;
using NUnit.Framework;

namespace InterlockLedger.Tags
{
    [TestFixture]
    public class ILTagJsonTests
    {
        [TestCase("true", TestName = "BoolFromJsonTrue", ExpectedResult = true)]
        [TestCase("false", TestName = "BoolFromJsonFalse", ExpectedResult = false)]
        public bool BoolFrom(string json) {
            var payload = JsonConvert.DeserializeObject(json);
            var tag = ILTag.DeserializeFromJson(ILTagId.Bool, payload);
            Assert.IsInstanceOf<ILTagBool>(tag);
            Assert.AreEqual(payload, tag.AsJson);
            return ((ILTagBool)tag).Value;
        }

        [TestCase("0", TestName = "Int8FromJson0", ExpectedResult = (sbyte)0)]
        [TestCase("127", TestName = "Int8FromJson127", ExpectedResult = (sbyte)127)]
        [TestCase("-127", TestName = "Int8FromJsonMinus127", ExpectedResult = (sbyte)-127)]
        public sbyte Int8From(string json) {
            var payload = JsonConvert.DeserializeObject(json);
            var tag = ILTag.DeserializeFromJson(ILTagId.Int8, payload);
            Assert.IsInstanceOf<ILTagInt8>(tag);
            Assert.AreEqual(payload, tag.AsJson);
            return ((ILTagInt8)tag).Value;
        }

        [TestCase("255", TestName = "Int8FromJson255Throws")]
        [TestCase("-255", TestName = "Int8FromJsonMinus255Throws")]
        [TestCase("1024", TestName = "Int8FromJson1024Throws")]
        public void Int8FromThrows(string json) {
            var payload = JsonConvert.DeserializeObject(json);
            Assert.Throws<OverflowException>(() => ILTag.DeserializeFromJson(ILTagId.Int8, payload));
        }

        [TestCase("", TestName = "NullFromJsonStringEmpty")]
        [TestCase("null", TestName = "NullFromJsonStringLiteralNull")]
        public void NullFrom(string json) {
            var payload = JsonConvert.DeserializeObject(json);
            var tag = ILTag.DeserializeFromJson(ILTagId.Null, payload);
            Assert.IsInstanceOf<ILTagNull>(tag);
            Assert.AreEqual(payload, tag.AsJson);
            var tag2 = ILTag.DeserializeFromJson(ILTagId.Version, payload);
            Assert.IsInstanceOf<ILTagNull>(tag2);
        }

        [TestCase("\"Some String\"", TestName = "TagStringFromSomeString")]
        [TestCase("\"\"", TestName = "TagStringFromEmpty")]
        public void StringFrom(string json) {
            var payload = JsonConvert.DeserializeObject(json);
            var tag = ILTag.DeserializeFromJson(ILTagId.String, payload);
            Assert.AreEqual(payload, tag.AsJson);
            Assert.IsInstanceOf<ILTagString>(tag);
        }

        [TestCase("true")]
        public void ThrowFromUnsupportedTag(string json) {
            var payload = JsonConvert.DeserializeObject(json);
            Assert.Throws<ArgumentException>(() => ILTag.DeserializeFromJson(ulong.MaxValue, payload));
            Assert.Throws<InvalidOperationException>(() => ILTag.DeserializeFromJson(ILTagId.BigInteger, payload));
        }

        [TestCase("0", TestName = "UInt8FromJson0", ExpectedResult = (byte)0)]
        [TestCase("127", TestName = "UInt8FromJson127", ExpectedResult = (byte)127)]
        [TestCase("255", TestName = "UInt8FromJson255", ExpectedResult = (byte)255)]
        public byte UInt8From(string json) {
            var payload = JsonConvert.DeserializeObject(json);
            var tag = ILTag.DeserializeFromJson(ILTagId.UInt8, payload);
            Assert.IsInstanceOf<ILTagUInt8>(tag);
            Assert.AreEqual(payload, tag.AsJson);
            return ((ILTagUInt8)tag).Value;
        }

        [TestCase("-127", TestName = "UInt8FromJsonMinus127Throws")]
        [TestCase("-255", TestName = "UInt8FromJsonMinus255Throws")]
        [TestCase("1024", TestName = "UInt8FromJson1024Throws")]
        public void UInt8FromThrows(string json) {
            var payload = JsonConvert.DeserializeObject(json);
            Assert.Throws<OverflowException>(() => ILTag.DeserializeFromJson(ILTagId.UInt8, payload));
        }
    }
}