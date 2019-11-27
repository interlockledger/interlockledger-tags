/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using NUnit.Framework;

namespace InterlockLedger.Tags
{
    [TestFixture]
    public class ILTagJsonTests
    {
        [OneTimeSetUp]
        public void _SetupConverters() {
            _jsonOptions.Converters.Add(new JsonInterlockIdConverter());
            _jsonOptions.Converters.Add(new JsonCustomConverter<EnumerationItems>());
            _jsonOptions.Converters.Add(new JsonCustomConverter<InterlockColor>());
        }

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
            Assert.AreEqual(datafield, payload);
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

        [Test]
        public void InterlockColorConverter() {
            static void TestWith(InterlockColor color) {
                var json = JsonSerializer.Serialize(color, _jsonOptions);
                TestContext.WriteLine(json);
                var payload = JsonSerializer.Deserialize<InterlockColor>(json, _jsonOptions);
                Assert.IsInstanceOf<InterlockColor>(payload);
                Assert.AreEqual(color, payload);
                var wrapper = new ColorWrapper(color, color.Opposite);
                var biggerJson = JsonSerializer.Serialize(wrapper, _jsonOptions);
                TestContext.WriteLine(biggerJson);
                var biggerPayload = JsonSerializer.Deserialize<ColorWrapper>(biggerJson, _jsonOptions);
                Assert.IsInstanceOf<ColorWrapper>(biggerPayload);
                Assert.AreEqual(wrapper, biggerPayload);
            }
            TestWith(InterlockColor.Gainsboro);
            TestWith(InterlockColor.DeepPink.Opposite);
            TestWith(InterlockColor.Transparent);
            TestWith(InterlockColor.WhiteSmoke);
        }

        [Test]
        public void InterlockIdConverter() {
            var owner = new OwnerId("Tester".HashOf());
            var json = JsonSerializer.Serialize(owner, _jsonOptions);
            TestContext.WriteLine(json);
            var payload = JsonSerializer.Deserialize<BaseKeyId>(json, _jsonOptions);
            Assert.IsInstanceOf<OwnerId>(payload);
            Assert.AreEqual(owner, payload);
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

        private class ColorWrapper : IEquatable<ColorWrapper>
        {
            public ColorWrapper() { }

            public ColorWrapper(InterlockColor foreground, InterlockColor background) {
                Foreground = foreground;
                Background = background;
            }

            public InterlockColor Background { get; set; }
            public InterlockColor Foreground { get; set; }

            public static bool operator !=(ColorWrapper left, ColorWrapper right) => !(left == right);

            public static bool operator ==(ColorWrapper left, ColorWrapper right) => EqualityComparer<ColorWrapper>.Default.Equals(left, right);

            public override bool Equals(object obj) => Equals(obj as ColorWrapper);

            public bool Equals([AllowNull] ColorWrapper other) => other != null && EqualityComparer<InterlockColor>.Default.Equals(Background, other.Background) && EqualityComparer<InterlockColor>.Default.Equals(Foreground, other.Foreground);

            public override int GetHashCode() => HashCode.Combine(Background, Foreground);
        }
    }
}