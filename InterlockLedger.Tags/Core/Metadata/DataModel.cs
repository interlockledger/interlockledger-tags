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

public class DataModel : IEquatable<DataModel>, IDataModel, IVersion
{
    public const ushort CurrentVersion = 2;

    public IEnumerable<DataField> DataFields { get; set; } = Enumerable.Empty<DataField>();

    public string? Description { get; set; }

    public IEnumerable<DataIndex> Indexes { get; set; } = Enumerable.Empty<DataIndex>();

    public string? PayloadName { get; set; }

    public ulong PayloadTagId { get; set; }

    public ushort Version { get; set; } = CurrentVersion;

    public override bool Equals(object? obj) => Equals(obj as DataModel);

    public bool Equals(DataModel? other) =>
        other is not null &&
        PayloadName.SafeEqualsTo(other.PayloadName) &&
        PayloadTagId == other.PayloadTagId &&
        DataFields.EqualTo(other.DataFields) &&
        Indexes.EqualTo(other.Indexes) &&
        Version.Equals(other.Version) &&
        Description.SafeEqualsTo(other.Description);

    public ILTag FromJson(object o) => FromNavigable(o.AsNavigable() as Dictionary<string, object?>);

    public ILTag FromNavigable(Dictionary<string, object?>? json)
        => FromPartialNavigable(json, PayloadTagId, DataFields, this);

    public override int GetHashCode()
        => HashCode.Combine(DataFields,
                            Indexes,
                            PayloadName,
                            PayloadTagId,
                            Description.TrimToNull(),
                            Version);

    public bool HasField(string fieldName) {
        return Find(DataFields, fieldName?.Split('.'));

        static bool Find(IEnumerable<DataField> fields, string[]? names) {
            return !(names is null || names.AnyNullOrWhiteSpace()) && FindFieldInPath(fields, names.AsEnumerable().GetEnumerator());
            static bool FindFieldInPath(IEnumerable<DataField>? fields, IEnumerator<string> names) {
                bool result = true;
                if (names.MoveNext()) {
                    var field = fields?.FirstOrDefault(df => df.IsVisibleMatch(names.Current));
                    result = field is not null && FindFieldInPath(field.SubDataFields, names);
                }
                names.Dispose();
                return result;
            }
        }
    }

    public bool IsCompatible(DataModel other)
        => other is not null
           && other.PayloadName == PayloadName
           && other.PayloadTagId == PayloadTagId
           && CompareFields(other.DataFields, DataFields)
           && CompareIndexes(other.Indexes, Indexes);

    public Dictionary<string, object?> ToJson(byte[] bytes) {
        ulong offset = 0;
        return ToJson(bytes, PayloadTagId, DataFields, ref offset);
    }

    public override string ToString() => $"{PayloadName ?? "Unnamed"} #{PayloadTagId}";

    private static bool Compare<T>(IEnumerable<T>? older, IEnumerable<T>? newer, Func<T, T, bool> areCompatible)
        => older.None() // true as anything is acceptable if there was nothing from the previous version
           || newer.Safe().Count() >= older.Count() // false if too few elements in the new version
           && older.Zip(newer!, areCompatible).AllTrue(); // true only if all pre-existing elements are compatible

    private static bool CompareFields(IEnumerable<DataField>? oldFields, IEnumerable<DataField>? newFields)
        => Compare(oldFields,
                   newFields,
                   (o, n) => o.Name == n.Name // Names diverge
                             && (o.TagId == n.TagId || o.IsOpaque) // Changing type is only allowed if previously it was an opaque type
                             && o.Version <= n.Version // Misversioning
                             && o.ElementTagId == n.ElementTagId // Changing type of array elements is bad
                             && (!n.IsOpaque || o.IsOpaque) // Can't make field opaque afterwards
                             && (n.IsDeprecated || !o.IsDeprecated) // Can't undeprecate field
                             && (!o.HasSubFields || CompareFields(o.SubDataFields, n.SubDataFields)) // Incompatible subfields
                             && o.EnumerationDefinition.IsSameAsOrExpandedBy(n.EnumerationDefinition)); // Incompatible enumerations

    private static bool CompareIndexes(IEnumerable<DataIndex>? oldIndexes, IEnumerable<DataIndex>? newIndexes)
        => Compare(oldIndexes.Safe(),
                   newIndexes.Safe(),
                   (o, n) => o.Name == n.Name
                             && o.IsUnique == n.IsUnique
                             && o.ElementsAsString == n.ElementsAsString);

    private static ILTag DecodePartial(ulong tagId, Span<byte> bytes, ref ulong offset) {
        using var ms = new MemoryStream(bytes.ToArray(), (int)offset, bytes.Length - (int)offset);
        var tag = TagProvider.DeserializeFrom(ms);
        if (tag.TagId != tagId && !tag.Traits.IsNull)
            throw new InvalidOperationException($"Expecting tagId {tagId} but came {tag.TagId}");
        offset += (ulong)ms.Position;
        return tag;
    }

    private static ulong DecodePartialILInt(Span<byte> bytes, ref ulong offset) {
        using var ms = new MemoryStream(bytes.ToArray(), (int)offset, bytes.Length - (int)offset);
        var ilint = ms.DecodeTagId();
        offset += (ulong)ms.Position;
        return ilint;
    }

    private static ILTag DeserializeItem(DataField field, object? fieldValue) {
        try {
            if (fieldValue is null)
                return ILTagNull.Instance;
            if (fieldValue is JsonElement je) {
                fieldValue = FromJsonElement(field, je).Required();
            }
            return field.IsEnumeration && fieldValue is string value
                ? field.EnumerationFromString(value)
                : field.IsArray
                    ? SpecialCaseSomeArrays(field, fieldValue)
                    : TagProvider.HasDeserializer(field.TagId)
                        ? TagProvider.DeserializeFromJson(field.TagId, fieldValue)
                        : DeserializePartialFromJson(field, fieldValue);
        } catch (Exception e) {
            throw new InvalidOperationException($"Could not deserialize json field '{field.Name}' of type {field.TagId}\r\nfrom value:\r\n{fieldValue}", e);
        }

        static ILTag SpecialCaseSomeArrays(DataField field, object? fieldValue) {
            object[] array = fieldValue switch {
                object[] arrayData => arrayData,
                Dictionary<string, object> dict when dict.TryGetValue("elements", out var value) => (object[])value,
                _ => throw new InvalidOperationException($"Can't extract array elements from '{fieldValue}'"),
            };
            return field.ElementTagId == 23
                ? new ILTagArrayOfILTag<ILTagRange>(array.Select(r => r is ILTagRange it ? it : ILTagRange.Build((string)r)))
                : new ILTagSequence(array);
        }

        static object? FromJsonElement(DataField field, JsonElement je) {
            return FromJsonElement(je, field.TagId, field.IsArray, field.ElementTagId);

            static object ToTypedArray(ulong tagId, ulong elementTagId, JsonElement.ArrayEnumerator items) =>
                tagId switch {
                    ILTagId.ILIntArray => items.Select(item => item.GetUInt64()).ToArray(),
                    ILTagId.ILTagArray => items.Select(item => DeserializeElement(elementTagId, item)).ToArray(),
                    ILTagId.Sequence => items.Select(item => DeserializeElement(item.GetProperty("TagId").GetUInt64(), item.GetProperty("Value"))),
                    _ => throw new NotSupportedException()
                };

            static ILTag DeserializeElement(ulong elementTagId, JsonElement item)
                => TagProvider.DeserializeFromJson(elementTagId, item);

            static object ToArray(ulong elementTagId, JsonElement items)
                => items.EnumerateArray().Select(item => TagProvider.DeserializeFromJson(elementTagId, FromJsonElement(item, elementTagId, false, 0ul))).ToArray();

            static Dictionary<string, JsonElement> ToDictionary(JsonElement je)
                => je.EnumerateObject().ToDictionary(prop => prop.Name, prop => prop.Value);

            static object? FromJsonElement(JsonElement je, ulong tagId, bool isArray, ulong elementTagId)
                => je.ValueKind switch {
                    JsonValueKind.Number => tagId.In(ILTagId.Int8, ILTagId.Int16, ILTagId.Int32, ILTagId.Int64)
                                                ? je.GetInt64()
                                                : je.GetUInt64(),
                    JsonValueKind.Undefined => null,
                    JsonValueKind.Object => isArray
                                                ? ToArray(je.GetProperty("ElementTagId").GetUInt64(), je.GetProperty("Elements"))
                                                : ToDictionary(je),
                    JsonValueKind.Array => ToTypedArray(tagId, elementTagId, je.EnumerateArray()),
                    JsonValueKind.String => je.GetString(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => null,
                    _ => throw new NotSupportedException()
                };
        }
    }

    private static ILTag DeserializePartialFromJson(DataField field, object fieldValue) => field.HasSubFields
        ? FromPartialNavigable(fieldValue as Dictionary<string, object?>, field.TagId, field.SubDataFields, null)
        : throw new InvalidDataException($"Unknown tagId {field.TagId}");

    private static ILTag FromPartialNavigable(Dictionary<string, object?>? json, ulong tagId, IEnumerable<DataField>? dataFields, DataModel? dataModel) {
        if (json is null || json.Count == 0)
            return ILTagNull.Instance;
        byte[] tagsAsBytes = JsonToBytes(json, dataFields);
        return dataModel is null ? new ILTagUnknown(tagId, tagsAsBytes) : new ILTagUnknown(dataModel, tagsAsBytes);

        byte[] JsonToBytes(Dictionary<string, object?> json, IEnumerable<DataField>? dataFields) {
            const ushort minVersion = 0;
            var tags = new List<ILTag>();
            ushort version = minVersion;
            if (dataFields is null)
                return Array.Empty<byte>();
            bool isVersioned = IsVersioned(dataFields);
            ScanFieldsToTags(ExtractVersion());
            return AppendRemainingBytes(tags.Select(t => t.EncodedBytes).SelectMany(b => b).ToArray());

            IEnumerable<DataField> ExtractVersion() {
                if (!isVersioned) return dataFields;
                if (json.TryGetValue(dataFields.First().Name, out var fieldValue) && fieldValue is not null) {
                    version = ToUInt16(fieldValue);
                }
                tags.Add(new ILTagUInt16(version));
                return dataFields.Skip(1);
            }

            byte[] AppendRemainingBytes(byte[] tagsAsBytes) {
                if (json.TryGetValue("_RemainingBytes_", out object? bytesValues)) {
                    var remainingBytes = bytesValues switch {
                        string st => Convert.FromBase64String(st),
                        JsonElement je when je.ValueKind == JsonValueKind.String => je.GetBytesFromBase64(),
                        byte[] b => b,
                        _ => throw new NotSupportedException()
                    };
                    return tagsAsBytes.SafeConcat(remainingBytes).ToArray();
                }
                return tagsAsBytes;
            }

            void ScanFieldsToTags(IEnumerable<DataField> remainingFields) {
                foreach (var field in remainingFields) {
                    if (isVersioned && field.Version > version) break;
                    var item = json.TryGetValue(field.Name, out var fieldValue)
                        ? fieldValue is ILTag tag ? MapItem(field, tag) : DeserializeItem(field, fieldValue)
                        : ILTagNull.Instance;
                    tags.Add(item);
                }
            }
        }
    }

    private static bool IsVersioned(IEnumerable<DataField> dataFields) => dataFields?.FirstOrDefault()?.IsVersion ?? false;

    private static ILTag MapItem(DataField field, ILTag tag)
        => tag.TagId != field.TagId
            ? throw new InvalidCastException($"Value for field {field.Name} is a tag {tag.TagId} != {field.TagId}")
            : tag;

    private static Dictionary<string, object?> ToJson(Span<byte> bytes, ulong expectedTagId, IEnumerable<DataField>? dataFields, ref ulong offset) {
        var json = new Dictionary<string, object?>();
        ulong tagId = DecodePartialILInt(bytes, ref offset);
        if (tagId != expectedTagId)
            throw new InvalidOperationException($"Expecting tagId {expectedTagId} but came {tagId}");
        ulong length = DecodePartialILInt(bytes, ref offset);
        if (length > (((ulong)bytes.Length) - offset))
            throw new InvalidOperationException($"Invalid number of bytes, expected {length + offset} but came {bytes.Length} ");
        ushort version = 0;
        if (dataFields != null) {
            var isVersioned = IsVersioned(dataFields);
            var firstField = true;
            foreach (var field in dataFields) {
                if (isVersioned && field.Version > version)
                    break;
                if (field.HasSubFields) {
                    json[field.Name] = ToJson(bytes, field.TagId, field.SubDataFields, ref offset);
                } else {
                    var value = DecodePartial(field.TagId, bytes, ref offset);
                    json[field.Name] = ConvertTagToJson(field, value);
                    if (isVersioned && firstField && field.IsVersion)
                        version = value is ILTagUInt16 uInt16 ? uInt16.Value : (ushort)0;

                }
                firstField = false;
            }
        }
        if (offset < length)
            json["_RemainingBytes_"] = bytes.Slice((int)offset, (int)((ulong)bytes.Length - offset)).ToArray();
        return json;

        static object? ConvertTagToJson(DataField field, ILTag value) =>
            field.IsEnumeration && !value.Traits.IsNull
                ? field.EnumerationToString(value)
                : field.IsArray
                    ? ToJsonArray(value)
                    : value.Content;

        static IEnumerable<object>? ToJsonArray(ILTag value) =>
            value.Content is null
            ? null
            : ((IEnumerable<object>)value.Content).Select(o => o is ITextual it ? it.TextualRepresentation : o).ToArray();
    }

    private static ushort ToUInt16(object fieldValue)
        => fieldValue switch {
            IConvertible => Convert.ToUInt16(fieldValue, CultureInfo.InvariantCulture),
            JsonElement je when je.ValueKind == JsonValueKind.Number => je.GetUInt16(),
            _ => throw new InvalidDataException($"Not a number! {fieldValue}")
        };
}

public class ILTagDataModel : ILTagExplicit<DataModel>
{
    public ILTagDataModel(DataModel model) : base(ILTagId.DataModel, model) {
    }

    public ILTagDataModel(Stream s) : base(ILTagId.DataModel, s) {
    }

    protected override DataModel FromBytes(byte[] bytes)
        => FromBytesHelper(bytes, s => {
            var payloadTagId = s.DecodeILInt();
            s.DecodeILInt(); // drop deprecated field
            return new DataModel {
                PayloadTagId = payloadTagId,
                DataFields = s.DecodeTagArray<ILTagDataField>().Safe().Select(t => t.Required().Value.Required()),
                Indexes = s.DecodeTagArray<ILTagDataIndex>().Safe().Select(t => t.Required().Value.Required()),
                PayloadName = s.DecodeString(),
                Version = s.HasBytes() ? s.DecodeUShort() : (ushort)1,
                Description = s.HasBytes() ? s.DecodeString().TrimToNull() : null
            };
        });

    protected override byte[] ToBytes(DataModel value)
        => TagHelpers.ToBytesHelper(s => {
            s.EncodeILInt(Value.Required().PayloadTagId);
            s.EncodeILInt(0); // deprecated field
            s.EncodeTagArray(Value.DataFields.Select(df => new ILTagDataField(df)));
            s.EncodeTagArray(Value.Indexes.Select(index => new ILTagDataIndex(index)));
            s.EncodeString(Value.PayloadName);
            s.EncodeUShort(Value.Version);
            s.EncodeString(Value.Description.TrimToNull());
        });
}