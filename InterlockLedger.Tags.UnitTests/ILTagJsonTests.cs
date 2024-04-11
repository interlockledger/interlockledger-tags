// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2024 InterlockLedger Network
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
public class ILTagJsonTests
{
    [Test]
    public void AppPermissionsCtors() {
        TestTwiceWith(new AppPermissions(0));
        TestTwiceWith(new AppPermissions(0, null));
        TestTwiceWith(new AppPermissions(1, 100));
        TestTwiceWith(new AppPermissions(2, 100, 101));
        var ap = AppPermissions.Parse("#3,1,2,3", null);
        Assert.That(ap.TextualRepresentation, Is.EqualTo("#3,1,2,3"));
        TestTwiceWith(ap);
        TestTwiceWith(new AppPermissions(4, 100, 4, 101, 3));
        TestTwiceWith(AppPermissions.Empty);
        TestTwiceWith(AppPermissions.InvalidBy("Test"));
    }

    [TestCase("true", TestName = "BoolFromJsonTrue", ExpectedResult = true)]
    [TestCase("false", TestName = "BoolFromJsonFalse", ExpectedResult = false)]
    public bool BoolFrom(string json) {
        var payload = json.DeserializeJson();
        var tag = TagProvider.DeserializeFromJson(ILTagId.Bool, payload);
        Assert.That(tag, Is.InstanceOf<ILTagBool>());
        return ((ILTagBool)tag).Value;
    }

    [Test]
    public void DataFieldToFrom() {
        static void TestWith(DataField datafield) {
            var json = JsonSerializer.Serialize(datafield, _jsonOptions);
            TestContext.WriteLine(json);
            var payload = JsonSerializer.Deserialize<DataField>(json, _jsonOptions);
            Assert.That(payload, Is.InstanceOf<DataField>());
            Assert.That(payload, Is.EqualTo(datafield));
        }
        TestWith(new DataField {
            Name = "TestEnumeration0",
            TagId = 0,
        });
        TestWith(new DataField {
            Name = "TestEnumeration1",
            TagId = 1,
            EnumerationDefinition = new EnumerationDictionary {
                [1ul] = new EnumerationDetails("Name1", "Descr1"),
            }
        });
        TestWith(new DataField {
            Name = "TestEnumeration2",
            TagId = 2,
            EnumerationDefinition = new EnumerationDictionary {
                [1ul] = new EnumerationDetails("Name1", "Descr1"),
                [3ul] = new EnumerationDetails("Name3", null)
            },
            Description = "This with Flags kind of enumeration",
            EnumerationAsFlags = true
        });
        TestWith(new DataField {
            Name = "TestFields",
            TagId = 3,
            SubDataFields = [
                    new() {
                        Name = "Field1",
                        TagId = 31
                    },
                    new() {
                        Name = "Field2",
                        TagId = 32
                    },
                ]
        });
    }

    [Test]
    public void ILTagVersionConverter() {
        TestTwiceWith(ILTagVersion.Empty);
        TestTwiceWith(ILTagVersion.Parse("7.6.5", null));
        TestTwiceWith(ITextual<ILTagVersion>.Parse("7.6.5"));
        TestTwiceWith(new ILTagVersion(new Version(1, 0, 0)));
        TestTwiceWith(new ILTagVersion(new Version(1, 2, 3, 4)));
        TestTwiceWith(new ILTagVersion(new Version(3, 0)));
        TestTwiceWith(new ILTagVersion(new Version()));
    }

    [TestCase("0", TestName = "Int8FromJson0", ExpectedResult = (sbyte)0)]
    [TestCase("127", TestName = "Int8FromJson127", ExpectedResult = (sbyte)127)]
    [TestCase("-127", TestName = "Int8FromJsonMinus127", ExpectedResult = (sbyte)-127)]
    public sbyte Int8From(string json) {
        var payload = json.DeserializeJson();
        var tag = TagProvider.DeserializeFromJson(ILTagId.Int8, payload);
        Assert.That(tag, Is.InstanceOf<ILTagInt8>());
        return ((ILTagInt8)tag).Value;
    }

    [TestCase("255", TestName = "Int8FromJson255Throws")]
    [TestCase("-255", TestName = "Int8FromJsonMinus255Throws")]
    [TestCase("1024", TestName = "Int8FromJson1024Throws")]
    public void Int8FromThrows(string json) {
        var payload = json.DeserializeJson();
        Assert.Throws<OverflowException>(() => TagProvider.DeserializeFromJson(ILTagId.Int8, payload));
    }

    [Test]
    public void InterlockColorConverter() {
        static void TestWith(InterlockColor color) {
            var json = JsonSerializer.Serialize(color, _jsonOptions);
            TestContext.WriteLine(json);
            var payload = JsonSerializer.Deserialize<InterlockColor>(json, _jsonOptions);
            Assert.That(payload, Is.InstanceOf<InterlockColor>());
            Assert.That(payload, Is.EqualTo(color));
            var wrapper = new ColorWrapper(color, color.Opposite);
            var biggerJson = JsonSerializer.Serialize(wrapper, _jsonOptions);
            TestContext.WriteLine(biggerJson);
            var biggerPayload = JsonSerializer.Deserialize<ColorWrapper>(biggerJson, _jsonOptions);
            Assert.That(biggerPayload, Is.InstanceOf<ColorWrapper>());
            Assert.That(biggerPayload, Is.EqualTo(wrapper));
        }
        TestWith(InterlockColor.Gainsboro);
        TestWith(InterlockColor.DeepPink.Opposite);
        TestWith(InterlockColor.Transparent);
        TestWith(InterlockColor.WhiteSmoke);
    }

    [Test]
    public void InterlockIdConverter() {
        TestTwiceWith(new OwnerId("Tester".HashOf()));
        TestTwiceWith(new KeyId("Tester".HashOf()));
    }

    [Test]
    public void LimitedRangeConverter() {
        TestTwiceWith(new LimitedRange(100));
        TestTwiceWith(LimitedRange.Empty);
        TestTwiceWith(new LimitedRange(200, 50));
        TestTwiceWith(LimitedRange.Parse("[10-19]", null));
        //TestTwiceWith(LimitedRange.Parse("10-9", null));
        TestTwiceWith(new ILTagRange(new LimitedRange(300, 50)));
        TestTwiceWith(new ILTagRange(new LimitedRange(400)));
        TestTwiceWith(new ILTagRange(LimitedRange.Empty));
    }

    [TestCase("", TestName = "NullFromJsonStringEmpty")]
    [TestCase("null", TestName = "NullFromJsonStringLiteralNull")]
    public void NullFrom(string json) {
        var payload = json.DeserializeJson();
        var tag = TagProvider.DeserializeFromJson(ILTagId.Null, payload);
        Assert.That(tag, Is.InstanceOf<ILTagNull>());
        var tag2 = TagProvider.DeserializeFromJson(ILTagId.Version, payload);
        Assert.That(tag2, Is.InstanceOf<ILTagNull>());
    }

    [TestCase("\"Some String\"", TestName = "TagStringFromSomeString")]
    [TestCase("\"\"", TestName = "TagStringFromEmpty")]
    public void StringFrom(string json) {
        var payload = json.DeserializeJson();
        var tag = TagProvider.DeserializeFromJson(ILTagId.String, payload);
        Assert.Multiple(() => {
            Assert.That(tag, Is.InstanceOf<ILTagString>());
        });
    }

    [Test]
    public void TagHashConverter() {
        TestTwiceWith(TagHash.Empty);
        TestTwiceWith(new TagHash(HashAlgorithm.SHA3_256, TagHash.HashSha256Of(TagHash.Empty.Data).Data));
        TestTwiceWith(TagHash.HashSha256Of([3, 2, 1]));
    }

    [TestCase("true")]
    public void ThrowFromUnsupportedTag(string json) {
        var payload = json.DeserializeJson();
        Assert.Throws<ArgumentException>(() => TagProvider.DeserializeFromJson(ulong.MaxValue, payload));
        Assert.Throws<InvalidOperationException>(() => TagProvider.DeserializeFromJson(ILTagId.BigInteger, payload));
    }

    [TestCase("0", TestName = "UInt8FromJson0", ExpectedResult = (byte)0)]
    [TestCase("127", TestName = "UInt8FromJson127", ExpectedResult = (byte)127)]
    [TestCase("255", TestName = "UInt8FromJson255", ExpectedResult = (byte)255)]
    public byte UInt8From(string json) {
        var payload = json.DeserializeJson();
        var tag = TagProvider.DeserializeFromJson(ILTagId.UInt8, payload);
        Assert.That(tag, Is.InstanceOf<ILTagUInt8>());
        return ((ILTagUInt8)tag).Value;
    }

    [TestCase("-127", TestName = "UInt8FromJsonMinus127Throws")]
    [TestCase("-255", TestName = "UInt8FromJsonMinus255Throws")]
    [TestCase("1024", TestName = "UInt8FromJson1024Throws")]
    public void UInt8FromThrows(string json) {
        var payload = json.DeserializeJson();
        Assert.Throws<OverflowException>(() => TagProvider.DeserializeFromJson(ILTagId.UInt8, payload));
    }

    private static readonly JsonSerializerOptions _jsonOptions = new() {
        AllowTrailingCommas = true,
        WriteIndented = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
        IgnoreReadOnlyProperties = true
    };

    private static void TestTwiceWith<T>(T value) {
        var json = JsonSerializer.Serialize(value, _jsonOptions);
        var payload = JsonSerializer.Deserialize<T>(json, _jsonOptions);
        Assert.That(payload, Is.InstanceOf<T>());
        TestContext.WriteLine($"({NoLineBreaks(value)}) => {json} => {NoLineBreaks(payload)}");
        Assert.That(payload, Is.EqualTo(value));
        var wrapper = new WrapperOf<T> { Item = value };
        var biggerJson = JsonSerializer.Serialize(wrapper, _jsonOptions);
        TestContext.WriteLine(biggerJson);
        var biggerPayload = JsonSerializer.Deserialize<WrapperOf<T>>(biggerJson, _jsonOptions);
        Assert.That(biggerPayload, Is.InstanceOf<WrapperOf<T>>());
        Assert.That(biggerPayload, Is.EqualTo(wrapper));
    }

    private static string NoLineBreaks<T>(T value) => value.ToString().Replace(Environment.NewLine, " ");

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

        public bool Equals([AllowNull] ColorWrapper other) => other is not null && EqualityComparer<InterlockColor>.Default.Equals(Background, other.Background) && EqualityComparer<InterlockColor>.Default.Equals(Foreground, other.Foreground);

        public override int GetHashCode() => HashCode.Combine(Background, Foreground);
    }

    private class WrapperOf<T> : IEquatable<WrapperOf<T>>
    {
        public T Item { get; set; }

        public static bool operator !=(WrapperOf<T> left, WrapperOf<T> right) => !(left == right);

        public static bool operator ==(WrapperOf<T> left, WrapperOf<T> right) => left.Equals(right);

        public override bool Equals(object obj) => Equals(obj as WrapperOf<T>);

        public bool Equals([AllowNull] WrapperOf<T> other) => other is not null && Item.Equals(other.Item);

        public override int GetHashCode() => HashCode.Combine(Item);
    }
}