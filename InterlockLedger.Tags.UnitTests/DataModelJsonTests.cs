// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2025 InterlockLedger Network
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

#nullable enable

using System.Text.Json;
using System.Text.Json.Serialization;

namespace InterlockLedger.Tags;
[Flags]
public enum SomeEnumeration
{
    None = 0,
    Some = 1,
    Lots = 2
}

[TestFixture]
public partial class DataModelJsonTests
{
    static DataModelJsonTests() {
        if (_options.Converters.None(c => c.GetType() == typeof(JsonStringEnumConverter)))
            _options.Converters.Add(new JsonStringEnumConverter());
    }

    [Test]
    public void FromJsonObjectV0WithRemainingBytes() => FromJsonObjectBaseTest(
        new { Version = 0, Id = 123, Name = "DataModelToJson", _RemainingBytes_ = "ERNIaWRkZW4gb24gVmVyc2lvbiAw" },
            249, 49, 221, 43,
                5, 0, 0,
                10, 123,
                17, 15, 68, 97, 116, 97, 77, 111, 100, 101, 108, 84, 111, 74, 115, 111, 110,
                17, 19, 72, 105, 100, 100, 101, 110, 32, 111, 110, 32, 86, 101, 114, 115, 105, 111, 110, 32, 48
        );

    [Test]
    public void FromJsonObjectV1() => FromJsonObjectBaseTest(
        new { Version = 1, Id = 123, Name = "DataModelToJson", Hidden = "Shown From Version 1" },
            249, 49, 221, 44,
                5, 1, 0,
                10, 123,
                17, 15, 68, 97, 116, 97, 77, 111, 100, 101, 108, 84, 111, 74, 115, 111, 110,
                17, 20, 83, 104, 111, 119, 110, 32, 70, 114, 111, 109, 32, 86, 101, 114, 115, 105, 111, 110, 32, 49
        );

    [Test]
    public void FromJsonObjectV2() => FromJsonObjectBaseTest(
        new { Version = 2, Id = 3, Name = "Test", Hidden = "No More", SemanticVersion = "1.2.3.4" },
            249, 49, 221, 38,
                5, 2, 0,
                10, 3,
                17, 4, 84, 101, 115, 116,
                17, 7, 78, 111, 32, 77, 111, 114, 101,
                24, 16, 1, 0, 0, 0, 2, 0, 0, 0, 3, 0, 0, 0, 4, 0, 0, 0
        );

    [Test]
    public void FromJsonObjectV3() => FromJsonObjectBaseTest(
        new {
            Version = 3,
            Id = 3,
            Name = "Test",
            Hidden = "No More",
            SemanticVersion = "1.2.3.4",
            Values = new byte[] { 5, 6, 7 }
        },
            249, 49, 221, 44,
                5, 3, 0,
                10, 3,
                17, 4, 84, 101, 115, 116,
                17, 7, 78, 111, 32, 77, 111, 114, 101,
                24, 16, 1, 0, 0, 0, 2, 0, 0, 0, 3, 0, 0, 0, 4, 0, 0, 0,
                20, 4, 3, 5, 6, 7
        );

    [Test]
    public void FromJsonObjectV4() => FromJsonObjectBaseTest(
        new {
            Version = 4,
            Id = 333,
            Name = "DataModelToJson",
            Hidden = "Shown From Version 1",
            SemanticVersion = "1.0.1.33",
            Values = new byte[] { 20, 21 },
            Fancy = new {
                Id = 3330,
                Name = "Fancy DataModelToJson"
            }
        },
        249, 49, 221, 99,
            5, 4, 0,
            10, 248, 85,
            17, 15, 68, 97, 116, 97, 77, 111, 100, 101, 108, 84, 111, 74, 115, 111, 110,
            17, 20, 83, 104, 111, 119, 110, 32, 70, 114, 111, 109, 32, 86, 101, 114, 115, 105, 111, 110, 32, 49,
            24, 16, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 33, 0, 0, 0,
            20, 3, 2, 20, 21,
            249, 49, 222, 27,
                10, 249, 12, 10,
                17, 21, 70, 97, 110, 99, 121, 32, 68, 97, 116, 97, 77, 111, 100, 101, 108, 84, 111, 74, 115, 111, 110
        );

    [Test]
    public void FromJsonObjectV5() => FromJsonObjectBaseTest(
        new {
            Version = 5,
            Id = 111,
            Name = "DataModelToJson",
            Hidden = "Shown From Version 1",
            SemanticVersion = "1.0.1.33",
            Values = new byte[] { 5, 11 },
            Fancy = new {
                Id = 1110,
                Name = "Fancy DataModelToJson"
            },
            Ranges = new {
                Elements = _rangesList,
                ElementTagId = 23
            }
        },
        249, 49, 221, 111,
            5, 5, 0,
            10, 111,
            17, 15, 68, 97, 116, 97, 77, 111, 100, 101, 108, 84, 111, 74, 115, 111, 110,
            17, 20, 83, 104, 111, 119, 110, 32, 70, 114, 111, 109, 32, 86, 101, 114, 115, 105, 111, 110, 32, 49,
            24, 16, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 33, 0, 0, 0,
            20, 3, 2, 5, 11,
            249, 49, 222, 27,
                10, 249, 3, 94,
                17, 21, 70, 97, 110, 99, 121, 32, 68, 97, 116, 97, 77, 111, 100, 101, 108, 84, 111, 74, 115, 111, 110,
            21, 11, 2,
                23, 3, 10, 5, 0,
                23, 3, 21, 13, 0);

    [Test]
    public void FromJsonObjectV6() => FromJsonObjectBaseTest(
        new {
            Version = 6,
            Id = 32,
            Name = "DataModelToJson",
            Hidden = "Shown From Version 1",
            SemanticVersion = "1.0.1.33",
            Values = new byte[] { 6, 12 },
            Fancy = new {
                Id = 320,
                Name = "Fancy DataModelToJson"
            },
            Ranges = new {
                Elements = _rangesList,
                ElementTagId = 23
            },
            Bytes = "Bgw="
        },
        249, 49, 221, 114,
            5, 6, 0,
            10, 32,
            17, 15, 68, 97, 116, 97, 77, 111, 100, 101, 108, 84, 111, 74, 115, 111, 110,
            17, 20, 83, 104, 111, 119, 110, 32, 70, 114, 111, 109, 32, 86, 101, 114, 115, 105, 111, 110, 32, 49,
            24, 16, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 33, 0, 0, 0,
            20, 3, 2, 6, 12,
            249, 49, 222, 26,
                10, 248, 72,
                17, 21, 70, 97, 110, 99, 121, 32, 68, 97, 116, 97, 77, 111, 100, 101, 108, 84, 111, 74, 115, 111, 110,
            21, 11, 2,
                23, 3, 10, 5, 0,
                23, 3, 21, 13, 0,
            16, 2, 6, 12);

    [Test]
    public void FromJsonObjectV7() => FromJsonObjectBaseTest(
        new {
            Version = 7,
            Id = 32,
            Name = "DataModelToJson",
            Hidden = "Shown From Version 1",
            SemanticVersion = "1.0.1.33",
            Values = new byte[] { 6, 12 },
            Fancy = new {
                Id = 320,
                Name = "Fancy DataModelToJson"
            },
            Ranges = new {
                Elements = _rangesList,
                ElementTagId = 23
            },
            Bytes = "Bgw=",
            Enumeration = "Some|Lots"
        },
        249, 49, 221, 116,
            5, 7, 0,
            10, 32,
            17, 15, 68, 97, 116, 97, 77, 111, 100, 101, 108, 84, 111, 74, 115, 111, 110,
            17, 20, 83, 104, 111, 119, 110, 32, 70, 114, 111, 109, 32, 86, 101, 114, 115, 105, 111, 110, 32, 49,
            24, 16, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 33, 0, 0, 0,
            20, 3, 2, 6, 12,
            249, 49, 222, 26,
                10, 248, 72,
                17, 21, 70, 97, 110, 99, 121, 32, 68, 97, 116, 97, 77, 111, 100, 101, 108, 84, 111, 74, 115, 111, 110,
            21, 11, 2,
                23, 3, 10, 5, 0,
                23, 3, 21, 13, 0,
            16, 2, 6, 12,
            3, 3);
    [Test]
    public void FromJsonObjectV8() => FromJsonObjectBaseTest(
        new {
            Version = 8,
            Id = 32,
            Name = "DataModelToJson",
            Hidden = "Shown From Version 1",
            SemanticVersion = "1.0.1.33",
            Values = new byte[] { 6, 12 },
            Fancy = new {
                Id = 320,
                Name = "Fancy DataModelToJson"
            },
            Ranges = new {
                Elements = _rangesList,
                ElementTagId = 23
            },
            Bytes = "Bgw=",
            Enumeration = "Some|Lots",
            Timestamp = "1980-01-01 13:00:00Z"
        },
        249, 49, 221, 123,
            5, 8, 0,
            10, 32,
            17, 15, 68, 97, 116, 97, 77, 111, 100, 101, 108, 84, 111, 74, 115, 111, 110,
            17, 20, 83, 104, 111, 119, 110, 32, 70, 114, 111, 109, 32, 86, 101, 114, 115, 105, 111, 110, 32, 49,
            24, 16, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 33, 0, 0, 0,
            20, 3, 2, 6, 12,
            249, 49, 222, 26,
                10, 248, 72,
                17, 21, 70, 97, 110, 99, 121, 32, 68, 97, 116, 97, 77, 111, 100, 101, 108, 84, 111, 74, 115, 111, 110,
            21, 11, 2,
                23, 3, 10, 5, 0,
                23, 3, 21, 13, 0,
            16, 2, 6, 12,
            3, 3,
            15, 252, 146, 244, 5, 24, 8);

    [Test]
    public void ToFromJsonV0()
        => ToFromBaseTest(
            new JsonTestTaggedData(0, 123, "DataModelToJson", "Hidden on Version 0").AsPayload,
            s => s.Replace($"{Environment.NewLine}}}", $",{Environment.NewLine}  \"_RemainingBytes_\": \"ERNIaWRkZW4gb24gVmVyc2lvbiAw\"{Environment.NewLine}}}", StringComparison.Ordinal));

    [Test]
    public void ToFromJsonV1()
        => ToFromBaseTest(new JsonTestTaggedData(1, 123, "DataModelToJson", "Shown From Version 1").AsPayload);

    [Test]
    public void ToFromJsonV2()
        => ToFromBaseTest(new JsonTestTaggedData(2, 456, "DataModelToJson", "Shown From Version 1").AsPayload);

    [Test]
    public void ToFromJsonV3()
        => ToFromBaseTest(new JsonTestTaggedData(3, 789, "DataModelToJson", "Shown From Version 1", some: default, 10, 11).AsPayload);

    [Test]
    public void ToFromJsonV4()
        => ToFromBaseTest(new JsonTestTaggedData(4, 333, "DataModelToJson", "Shown From Version 1", some: default, 20, 21).AsPayload);

    [Test]
    public void ToFromJsonV5()
        => ToFromBaseTest(new JsonTestTaggedData(5, 111, "DataModelToJson", "Shown From Version 1", some: default, 5, 11).AsPayload);

    [Test]
    public void ToFromJsonV6()
        => ToFromBaseTest(new JsonTestTaggedData(6, 32, "DataModelToJson", "Shown From Version 1", some: default, 6, 12).AsPayload);

    [Test]
    public void ToFromJsonV7()
        => ToFromBaseTest(new JsonTestTaggedData(7, 32, "DataModelToJson", "Shown From Version 1", some: SomeEnumeration.Lots | SomeEnumeration.Some, 7, 12).AsPayload);

    [Test]
    public void ToFromJsonV8()
        => ToFromBaseTest(new JsonTestTaggedData(8, 32, "DataModelToJson", "Shown From Version 1", some: SomeEnumeration.Lots | SomeEnumeration.Some, 7, 12).AsPayload);

    [Test]
    public void DataModelAsJsonAndBack() {
        var json = JsonSerializer.Serialize(JsonTestTaggedData.Model, _modelOptions);
        TestContext.Out.Write(json);
        Assert.That(json, Is.EqualTo(_dataModelAsJson));
        var model = JsonSerializer.Deserialize<DataModel>(json, _modelOptions);
        Assert.That(model, Is.Not.Null);
        Assert.That(model, Is.EqualTo(JsonTestTaggedData.Model));
    }

    private const string _dataModelAsJson = /*lang=json,strict*/ """
        {
          "dataFields": [
            {
              "name": "Version",
              "tagId": 5
            },
            {
              "name": "Id",
              "tagId": 10
            },
            {
              "name": "Name",
              "tagId": 17
            },
            {
              "name": "Hidden",
              "tagId": 17,
              "version": 1
            },
            {
              "name": "SemanticVersion",
              "tagId": 24,
              "version": 2
            },
            {
              "name": "Values",
              "tagId": 20,
              "version": 3
            },
            {
              "name": "Fancy",
              "subDataFields": [
                {
                  "name": "Id",
                  "tagId": 10
                },
                {
                  "name": "Name",
                  "tagId": 17
                }
              ],
              "tagId": 13014,
              "version": 4
            },
            {
              "elementTagId": 23,
              "name": "Ranges",
              "tagId": 21,
              "version": 5
            },
            {
              "name": "Bytes",
              "tagId": 16,
              "version": 6
            },
            {
              "enumeration": "#0|None|Nothing|#1|Some|A small number|#2|Lots|A large number|",
              "enumerationAsFlags": true,
              "name": "Enumeration",
              "tagId": 3,
              "version": 7
            },
            {
              "name": "Timestamp",
              "tagId": 15,
              "version": 8
            }
          ],
          "payloadName": "JsonTestTaggedData",
          "payloadTagId": 13013,
          "version": 2
        }
        """;
    private static readonly JsonSerializerOptions _options = new() {
        WriteIndented = true,
        PropertyNamingPolicy = null,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static readonly JsonSerializerOptions _modelOptions = new() {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        AllowTrailingCommas = true,
        IgnoreReadOnlyProperties = true        
    };


    private static readonly string[] _rangesList = ["[10-14]", "[21-33]"];

    private static void FromJsonObjectBaseTest(object json, params byte[] expectedBytes) {
        var tag = JsonTestTaggedData.Model.FromJson(json);
        byte[] encodedBytes = tag.EncodedBytes();
        TestContext.Out.WriteLine("===== Expected");
        TestContext.Out.WriteLine(expectedBytes.WithCommas());
        TestContext.Out.WriteLine("===== Actual");
        TestContext.Out.WriteLine(encodedBytes.WithCommas());
        Assert.That(encodedBytes, Is.EqualTo(expectedBytes));
    }

    private static void ToFromBaseTest(ILTag data, Func<string, string>? adjustExpected = null) {
        var encodedBytes = data.EncodedBytes();
        var modelJson = JsonTestTaggedData.Model.ToJson(encodedBytes);
        var expected = JsonSerializer.Serialize(data.Content, _options);
        if (adjustExpected is not null)
            expected = adjustExpected(expected);
        var actual = JsonSerializer.Serialize(modelJson, _options);
        TestContext.Out.WriteLine("===== Expected");
        TestContext.Out.WriteLine(expected);
        TestContext.Out.WriteLine("===== Actual");
        TestContext.Out.WriteLine(actual);
        Assert.That(actual, Is.EqualTo(expected));
        TestContext.Out.WriteLine("===== Initial EncodedBytes");
        TestContext.Out.WriteLine(encodedBytes.WithCommas());
        var backFrom = JsonTestTaggedData.Model.FromNavigable(modelJson);
        Assert.That(backFrom, Is.InstanceOf<ILTag>());
        byte[] backFromEncodedBytes = backFrom.EncodedBytes();
        TestContext.Out.WriteLine("===== Back EncodedBytes");
        TestContext.Out.WriteLine(backFromEncodedBytes.WithCommas());
        Assert.That(backFromEncodedBytes, Is.EqualTo(encodedBytes), "Bytes back from modelJson did not match");
        var parsedFrom = JsonTestTaggedData.Model.FromNavigable(JsonSerializer.Deserialize<Dictionary<string, object?>>(actual));
        Assert.That(parsedFrom, Is.InstanceOf<ILTag>());
        byte[] parsedFromEncodedBytes = parsedFrom.EncodedBytes();
        TestContext.Out.WriteLine("===== Parsed EncodedBytes");
        TestContext.Out.WriteLine(parsedFromEncodedBytes.WithCommas());
        //Assert.AreEqual(encodedBytes, parsedFromEncodedBytes, "Bytes parsed from json did not match");
    }

    private partial class JsonTestTaggedData : IRecordData<JsonTestTaggedData>
    {
        public const ulong DataTagId = 13014;

        public const ulong PayloadTagId = 13013;

        public static readonly DataModel Model = new() {
            PayloadName = nameof(JsonTestTaggedData),
            PayloadTagId = PayloadTagId,
            DataFields = [
                new() {
                    TagId = ILTagId.UInt16,
                    Name = nameof(Version),
                    Version = 0
                },
                new() {
                    TagId = ILTagId.ILInt,
                    Name = nameof(Id),
                    Version = 0
                },
                new() {
                    TagId = ILTagId.String,
                    Name = nameof(Name),
                    Version = 0
                },
                new() {
                    TagId = ILTagId.String,
                    Name = nameof(Hidden),
                    Version = 1
                },
                new() {
                    TagId = ILTagId.Version,
                    Name = nameof(SemanticVersion),
                    Version = 2
                },
                new() {
                    TagId = ILTagId.ILIntArray,
                    Name = nameof(Values),
                    Version = 3
                },
                new() {
                    TagId = DataTagId,
                    Name = nameof(Fancy),
                    Version = 4,
                    SubDataFields = [
                        new() {
                            TagId = ILTagId.ILInt,
                            Name = nameof(Reference.Data.Id)
                        },
                        new() {
                            TagId = ILTagId.String,
                            Name = nameof(Reference.Data.Name)
                        },
                    ]
                },
                new() {
                    TagId = ILTagId.ILTagArray,
                    Name = nameof(Ranges),
                    Version = 5,
                    ElementTagId = ILTagId.Range
                },
                new() {
                    TagId = ILTagId.ByteArray,
                    Name = nameof(Bytes),
                    Version = 6
                },
                new() {
                    TagId = ILTagId.UInt8,
                    Name = nameof(Enumeration),
                    Version = 7,
                    EnumerationDefinition = new EnumerationDictionary {
                        [(byte)SomeEnumeration.None] = new EnumerationDetails(nameof(SomeEnumeration.None), "Nothing"),
                        [(byte)SomeEnumeration.Some] = new EnumerationDetails(nameof(SomeEnumeration.Some), "A small number"),
                        [(byte)SomeEnumeration.Lots] = new EnumerationDetails(nameof(SomeEnumeration.Lots), "A large number"),
                    },
                    EnumerationAsFlags = true
                },
                new() {
                    TagId = ILTagId.Timestamp,
                    Name = nameof(Timestamp),
                    Version = 8
                }
           ]
        };
        private readonly string? _hidden;
        private readonly Reference? _fancy;

        public ushort Version { get; }
        public ulong Id { get; }
        public string? Name { get; }
        public string? Hidden => Version > 0 ? _hidden : null;
        public Version? SemanticVersion { get; }
        public ulong[]? Values { get; }
        public Reference.Data? Fancy => _fancy?.Value;
        public LimitedRange[]? Ranges { get; }
        public byte[]? Bytes { get; }
        public SomeEnumeration? Enumeration { get; }
        public ILTagTimestamp? Timestamp { get; }

        public JsonTestTaggedData() => AsPayload = new Payload<JsonTestTaggedData>(PayloadTagId, this);

        public JsonTestTaggedData(ushort version, ulong id, string name, string hidden, SomeEnumeration some = SomeEnumeration.None, params ulong[] values) {
            Version = version;
            Id = id;
            Name = name.Required();
            _hidden = hidden.Required();
            SemanticVersion = version <= 1 ? null : new Version(1, 0, 1, 33);
            Values = version <= 2 ? null : values;
            _fancy = version <= 3 ? null : new Reference(id * 10, "Fancy " + name);
            Ranges = version <= 4 ? null : [new(10, 5), new(21, 13)];
            Bytes = version <= 5 ? null : Values!.Select(u => (byte)u).ToArray();
            Enumeration = version <= 6 ? null : some;
            Timestamp = version <= 7 ? null : new ILTagTimestamp(new DateTimeOffset(1980, 1, 1, 13, 0, 0, TimeSpan.Zero));
            AsPayload = new Payload<JsonTestTaggedData>(PayloadTagId, this);
        }

        public void ToStream(Stream s) {
            s.EncodeUShort(Version);                 // Field index 0 //
            s.EncodeILInt(Id);                       // Field index 1 //
            s.EncodeString(Name);                    // Field index 2 //
            s.EncodeString(_hidden);                 // Field index 3 //
            if (Version > 1)
                s.EncodeVersion(SemanticVersion!);    // Field index 4 - from version 2 //
            if (Version > 2)
                s.EncodeILIntArray(Values);
            if (Version > 3)
                _ = _fancy!.SerializeIntoAsync(s).Result;
            if (Version > 4)
                s.EncodeTagArray(Ranges!.Select(r => new ILTagRange(r)));
            if (Version > 5)
                s.EncodeByteArray(Bytes);
            if (Version > 6)
                s.EncodeByte((byte)Enumeration!);
            if (Version > 7)
                s.EncodeTag(Timestamp!);
        }

        [JsonIgnore]
        public object AsJson => Version switch {
            0 => AsJsonV0(),
            1 => AsJsonV1(),
            2 => AsJsonV2(),
            3 => AsJsonV3(),
            4 => AsJsonV4(),
            5 => AsJsonV5(),
            6 => AsJsonV6(),
            7 => AsJsonV7(),
            8 => AsJsonV8(),
            _ => throw new InvalidDataException($"Unknown version {Version}"),
        };

        [JsonIgnore]
        public Payload<JsonTestTaggedData> AsPayload { get; }
        public JsonTestTaggedData FromStream(Stream s) => throw new NotImplementedException();

        private ILTagArrayOfILTag<ILTagRange>? _taggedRanges => Ranges is not null ? new(Ranges.Select(r => new ILTagRange(r))) : null;

        private object AsJsonV0() => new { Version, Id, Name };

        private object AsJsonV1() => new { Version, Id, Name, Hidden };

        private object AsJsonV2() => new { Version, Id, Name, Hidden, SemanticVersion = SemanticVersion?.ToString(4) };

        private object AsJsonV3() => new { Version, Id, Name, Hidden, SemanticVersion = SemanticVersion?.ToString(4), Values };

        private object AsJsonV4() => new { Version, Id, Name, Hidden, SemanticVersion = SemanticVersion?.ToString(4), Values, Fancy };

        private object AsJsonV5() => new { Version, Id, Name, Hidden, SemanticVersion = SemanticVersion?.ToString(4), Values, Fancy, Ranges = _taggedRanges };

        private object AsJsonV6() => new { Version, Id, Name, Hidden, SemanticVersion = SemanticVersion?.ToString(4), Values, Fancy, Ranges = _taggedRanges, Bytes };

        private object AsJsonV7() => new { Version, Id, Name, Hidden, SemanticVersion = SemanticVersion?.ToString(4), Values, Fancy, Ranges = _taggedRanges, Bytes, Enumeration };

        private object AsJsonV8() => new { Version, Id, Name, Hidden, SemanticVersion = SemanticVersion?.ToString(4), Values, Fancy, Ranges = _taggedRanges, Bytes, Enumeration, Timestamp!.TextualRepresentation };
    }
}