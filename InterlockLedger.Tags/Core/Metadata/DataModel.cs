/******************************************************************************************************************************

Copyright (c) 2018-2020 InterlockLedger Network
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
using System.Globalization;
using System.IO;
using System.Linq;
using InterlockLedger.Tags;

namespace InterlockLedger.Tags
{
    public class DataModel : IEquatable<DataModel>, IDataModel, IVersion
    {
        public const ushort CurrentVersion = 2;

        public IEnumerable<DataField> DataFields { get; set; }

        public string Description { get; set; }

        public IEnumerable<DataIndex> Indexes { get; set; }

        public string PayloadName { get; set; }

        public ulong PayloadTagId { get; set; }

        public ushort Version { get; set; } = CurrentVersion;

        public override bool Equals(object obj) => Equals(obj as DataModel);

        public bool Equals(DataModel other) =>
            other != null &&
            PayloadName.SafeEqualsTo(other.PayloadName) &&
            PayloadTagId == other.PayloadTagId &&
            DataFields.EqualTo(other.DataFields) &&
            Indexes.EqualTo(other.Indexes) &&
            Version.Equals(other.Version) &&
            Description.SafeEqualsTo(other.Description);

        public ILTag FromJson(object o) => FromNavigable(o.AsNavigable() as Dictionary<string, object>);

        public ILTag FromNavigable(Dictionary<string, object> json) => FromPartialNavigable(json, PayloadTagId, DataFields, this);

        public override int GetHashCode() => HashCode.Combine(DataFields, Indexes, PayloadName, PayloadTagId, Description, Version);

        public bool HasField(string fieldName) {
            return Find(DataFields, fieldName?.Split('.'));

            static bool Find(IEnumerable<DataField> fields, string[] names) {
                return !(names is null || names.AnyNullOrWhiteSpace()) && FindFieldInPath(fields, names.AsEnumerable().GetEnumerator());
                static bool FindFieldInPath(IEnumerable<DataField> fields, IEnumerator<string> names) {
                    bool result = true;
                    if (names.MoveNext()) {
                        var field = fields?.FirstOrDefault(df => df.IsVisibleMatch(names.Current));
                        result = field != null && FindFieldInPath(field.SubDataFields, names);
                    }
                    names.Dispose();
                    return result;
                }
            }
        }

        public bool IsCompatible(DataModel other) => other != null && other.PayloadName == PayloadName && other.PayloadTagId == PayloadTagId && ExpandsOver(other);

        public Dictionary<string, object> ToJson(byte[] bytes) {
            ulong offset = 0;
            return ToJson(bytes, PayloadTagId, DataFields, ref offset);
        }

        public override string ToString() => $"{PayloadName ?? "Unnamed"} #{PayloadTagId}";

        private static bool CompareFields(IEnumerable<DataField> oldFields, IEnumerable<DataField> newFields) {
            if (newFields == null)
                return false;
            if (!oldFields.SafeAny())
                return true;
            var fields = newFields.ToArray();
            if (fields.Length < oldFields.Count())
                return false; // too few
            var i = 0;
            foreach (var oldField in oldFields) {
                var newField = fields[i++];
                if (oldField.Name != newField.Name)
                    return false; // Names diverge
                if (oldField.Version > newField.Version)
                    return false; // Misversioning
                if (oldField.TagId != newField.TagId && !oldField.IsOpaque.GetValueOrDefault())
                    return false; // Changing type is only allowed if previously it was an opaque type
                if (oldField.ElementTagId != newField.ElementTagId)
                    return false; // Changing type of array elements is bad
                if (newField.IsOpaque.GetValueOrDefault() && !oldField.IsOpaque.GetValueOrDefault())
                    return false; // Can't make field opaque afterwards
                if (oldField.HasSubFields && !CompareFields(oldField.SubDataFields, newField.SubDataFields))
                    return false; // Incompatible subfields
                if (!ExpandEnumeration(oldField.Enumeration, newField.Enumeration))
                    return false; // Incompatible enumerations
            }
            return true;
        }

        private static bool CompareIndexes(IEnumerable<DataIndex> oldIndexes, IEnumerable<DataIndex> newIndexes) {
            if (newIndexes == null)
                return false;
            if (!oldIndexes.SafeAny())
                return true;
            var indexes = newIndexes.ToArray();
            if (indexes.Length < oldIndexes.Count())
                return false; // too few
            var i = 0;
            foreach (var oldIndex in oldIndexes) {
                var newIndex = indexes[i++];
                if (oldIndex.Name != newIndex.Name)
                    return false; // Names diverge
                if (oldIndex.IsUnique != newIndex.IsUnique)
                    return false; // Uniquenesses diverge
                if (oldIndex.ElementsAsString != newIndex.ElementsAsString)
                    return false; // Can't change composition
            }
            return true;
        }

        private static ILTag DecodePartial(ulong tagId, Span<byte> bytes, ref ulong offset) {
            using var ms = new MemoryStream(bytes.ToArray(), (int)offset, bytes.Length - (int)offset);
            var tag = ILTag.DeserializeFrom(ms);
            if (tag.TagId != tagId && !tag.IsNull)
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

        private static ILTag DeserializePartialFromJson(DataField field, object fieldValue) => field.HasSubFields
            ? FromPartialNavigable(fieldValue as Dictionary<string, object>, field.TagId, field.SubDataFields, null)
            : throw new InvalidDataException($"Unknown tagId {field.TagId}");

        private static bool ExpandEnumeration(Dictionary<ulong, EnumerationDetails> oldEnumeration, Dictionary<ulong, EnumerationDetails> newEnumeration)
            => oldEnumeration == null || oldEnumeration.Count == 0 || (newEnumeration?.Take(oldEnumeration.Count).SafeSequenceEqual(oldEnumeration) == true);

        private static ILTag FromPartialNavigable(Dictionary<string, object> json, ulong tagId, IEnumerable<DataField> dataFields, DataModel dataModel) {
            if (json is null || json.Count == 0)
                return ILTagNull.Instance;
            const ushort minVersion = 1;
            ushort version = minVersion;
            var isVersioned = IsVersioned(dataFields);
            var firstField = true;
            var tags = new List<ILTag>();
            foreach (var field in dataFields) {
                if (!json.TryGetValue(field.Name, out var fieldValue))
                    continue;
                if (isVersioned) {
                    if (firstField) {
                        version = Math.Max(Convert.ToUInt16(fieldValue, CultureInfo.InvariantCulture), minVersion);
                        firstField = false;
                    } else if (field.Version > version)
                        break;
                }
                if (fieldValue is ILTag tag) {
                    if (tag.TagId != field.TagId)
                        throw new InvalidCastException($"Value for field {field.Name} is a tag {tag.TagId} != {field.TagId}");
                    tags.Add(tag);
                } else {
                    var item = DeserializeItem(field, fieldValue);
                    tags.Add(item);
                }
            }
            var tagsAsBytes = tags.Select(t => t.EncodedBytes).SelectMany(b => b).ToArray();
            if (json.ContainsKey("_RemainingBytes_")) {
                var bytesValues = json["_RemainingBytes_"];
                var remainingBytes = bytesValues is string ? Convert.FromBase64String(bytesValues as string) : (byte[])bytesValues;
                tagsAsBytes = tagsAsBytes.SafeConcat(remainingBytes).ToArray();
            }
            return dataModel is null ? new ILTagUnknown(tagId, tagsAsBytes) : new ILTagUnknown(dataModel, tagsAsBytes);

            static ILTag DeserializeItem(DataField field, object fieldValue) {
                try {
                    return ILTag.HasDeserializer(field.TagId) ? ILTag.DeserializeFromJson(field.TagId, fieldValue) : DeserializePartialFromJson(field, fieldValue);
                } catch (Exception e) {
                    throw new InvalidOperationException($"Could not deserialize from json field {field.Name} of type {field.TagId}\r\nfrom {fieldValue}", e);
                }
            }
        }

        private static bool IsVersioned(IEnumerable<DataField> dataFields) => dataFields?.FirstOrDefault()?.IsVersion ?? false;

        private static Dictionary<string, object> ToJson(Span<byte> bytes, ulong expectedTagId, IEnumerable<DataField> dataFields, ref ulong offset) {
            var json = new Dictionary<string, object>();
            ulong tagId = DecodePartialILInt(bytes, ref offset);
            if (tagId != expectedTagId)
                throw new InvalidOperationException($"Expecting tagId {expectedTagId} but came {tagId}");
            ulong length = DecodePartialILInt(bytes, ref offset);
            if (length > (((ulong)bytes.Length) - offset))
                throw new InvalidOperationException($"Invalid number of bytes, expected {length + offset} but came {bytes.Length} ");
            ushort version = 0;
            var isVersioned = IsVersioned(dataFields);
            var firstField = true;
            foreach (var field in dataFields) {
                if (isVersioned && field.Version > version)
                    break;
                if (field.HasSubFields) {
                    json[field.Name] = ToJson(bytes, field.TagId, field.SubDataFields, ref offset);
                } else {
                    var value = DecodePartial(field.TagId, bytes, ref offset);
                    json[field.Name] = value.AsJson;
                    if (field.IsEnumeration && !value.IsNull)
                        json[$"__{field.Name}__"] = field.Enumerated(value);
                    if (isVersioned && firstField && field.IsVersion)
                        version = (ushort)value.AsJson;
                }
                firstField = false;
            }
            if (offset < length)
                json["_RemainingBytes_"] = bytes.Slice((int)offset, (int)((ulong)bytes.Length - offset)).ToArray();
            return json;
        }

        private bool ExpandsOver(DataModel dm) => CompareFields(dm.DataFields, DataFields) && CompareIndexes(dm.Indexes, Indexes);
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
                    DataFields = s.DecodeTagArray<ILTagDataField>()?.Select(t => t.Value),
                    Indexes = s.DecodeTagArray<ILTagDataIndex>()?.Select(t => t.Value),
                    PayloadName = s.DecodeString(),
                    Version = s.HasBytes() ? s.DecodeUShort() : (ushort)1,
                    Description = s.HasBytes() ? s.DecodeString() : null
                };
            });

        protected override byte[] ToBytes()
            => ToBytesHelper(s => {
                s.EncodeILInt(Value.PayloadTagId);
                s.EncodeILInt(0); // deprecated field
                s.EncodeTagArray(Value.DataFields?.Select(df => new ILTagDataField(df)));
                s.EncodeTagArray(Value.Indexes?.Select(index => new ILTagDataIndex(index)));
                s.EncodeString(Value.PayloadName);
                s.EncodeUShort(Value.Version);
                s.EncodeString(Value.Description);
            });
    }
}
