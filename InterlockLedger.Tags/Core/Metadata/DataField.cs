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
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

//#pragma warning disable CA2227 // Collection properties should be read only

namespace InterlockLedger.Tags
{
    public sealed class DataField : IEquatable<DataField>
    {
        public const ushort CurrentVersion = 4;

        public DataField(string name, ulong tagId, string description = null) {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Need a name", nameof(name));
            Description = description.TrimToNull();
            Name = name;
            TagId = tagId;
            Version = 1;
        }

        public DataField() => Version = 1;

        public CastType? Cast { get; set; }

        public string Description { get; set; }

        public ulong? ElementTagId { get; set; }

        public EnumerationItems Enumeration {
            get => EnumerationDefinition?.AsEnumerationItems;
            set => EnumerationDefinition = value?.UpdateFrom();
        }

        public bool? EnumerationAsFlags { get; set; }

        [JsonIgnore]
        public EnumerationDictionary EnumerationDefinition { get; set; }

        // tags that have children
        [JsonIgnore]
        public bool HasSubFields => SubDataFields.SafeAny();

        [JsonIgnore]
        public bool IsEnumeration => EnumerationDefinition.SafeAny();

        // treat as opaque byte array
        public bool? IsOpaque { get; set; }

        public bool? IsOptional { get; set; }

        [JsonIgnore]
        public bool IsVersion => Name.Equals("Version", StringComparison.OrdinalIgnoreCase) && TagId == ILTagId.UInt16;

        // Case-insensitive (can't contain dots or whitespace)
        public string Name { get; set; }

        [JsonIgnore]
        public ushort SerializationVersion { get; set; } = CurrentVersion;

        public IEnumerable<DataField> SubDataFields { get; set; }

        public ulong TagId { get; set; }

        public ushort Version { get; set; }

        public static ulong AsNumber(ILTag value) {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            return value.TagId switch
            {
                2 => (ulong)((ILTagInt8)value).Value,
                3 => ((ILTagUInt8)value).Value,
                4 => (ulong)((ILTagInt16)value).Value,
                5 => ((ILTagUInt16)value).Value,
                6 => (ulong)((ILTagInt32)value).Value,
                7 => ((ILTagUInt32)value).Value,
                8 => (ulong)((ILTagInt64)value).Value,
                9 => ((ILTagUInt64)value).Value,
                10 => ((ILTagILInt)value).Value,
                _ => throw new InvalidCastException("Not an integral numeric ILTag"),
            };
        }

        public static ILTag AsNumericTag(ulong tagId, ulong value) => tagId switch
        {
            2 => new ILTagInt8((sbyte)value),
            3 => new ILTagUInt8((byte)value),
            4 => new ILTagInt16((short)value),
            5 => new ILTagUInt16((ushort)value),
            6 => new ILTagInt32((int)value),
            7 => new ILTagUInt32((uint)value),
            8 => new ILTagInt64((long)value),
            9 => new ILTagUInt64(value),
            10 => new ILTagILInt(value),
            _ => throw new InvalidCastException("Not an integral numeric ILTag"),
        };

        public static bool operator !=(DataField left, DataField right) => !(left == right);

        public static bool operator ==(DataField left, DataField right) => EqualityComparer<DataField>.Default.Equals(left, right);

        public ILTag EnumerationFromString(string value) {
            return string.IsNullOrWhiteSpace(value) || EnumerationDefinition is null || EnumerationDefinition.Count == 0 || value == "?"
                ? ILTagNull.Instance
                : AsNumericTag(TagId, Parse(value));

            ulong Parse(string value) {
                return !_isFlags ? FindKey(value) : SplitAndParse(value);

                ulong FindKey(string value) {
                    try {
                        return Regex.IsMatch(value, @"^\?\d+$")
                            ? ulong.Parse(value.Substring(1), CultureInfo.InvariantCulture)
                            : EnumerationDefinition.First(kp => kp.Value.Name.Equals(value, StringComparison.InvariantCultureIgnoreCase)).Key;
                    } catch (Exception e) {
                        throw new InvalidDataException($"Value '{value}' is not valid for the enumeration for field [{Name}]", e);
                    }
                }

                ulong SplitAndParse(string value) {
                    ulong numericValue = 0;
                    foreach (var partValue in value.Split('|', ','))
                        numericValue |= FindKey(partValue);
                    return numericValue;
                }
            }
        }

        public string EnumerationToString(ILTag value) => EnumerationToString(AsNumber(value));

        public string EnumerationToString(ulong number) {
            try {
                return EnumerationDefinition is null || EnumerationDefinition.Count == 0
                    ? "?"
                    : _isFlags
                        ? ToList(number)
                        : EnumerationDefinition.ContainsKey(number) ? EnumerationDefinition[number].Name : $"?{number}";
            } catch {
                return "*error*";
            }

            string ToList(ulong number) {
                var names = new List<string>();
                foreach (ulong k in EnumerationDefinition.Keys) {
                    if (number == k || ((number & k) == k && k > 0)) {
                        number ^= k;
                        names.Add(EnumerationDefinition[k].Name);
                    }
                }
                if (number > 0)
                    names.Add($"?{number}");
                return names.JoinedBy("|");
            }
        }

        public override bool Equals(object obj) => Equals(obj as DataField);

        public bool Equals(DataField other) =>
            other != null &&
            TagId == other.TagId &&
            IsOptional == other.IsOptional &&
            IsOpaque == other.IsOpaque &&
            Name.SafeEqualsTo(other.Name) &&
            SubDataFields.EqualTo(other.SubDataFields) &&
            Cast == other.Cast &&
            ElementTagId == other.ElementTagId &&
            Version == other.Version &&
            SerializationVersion == other.SerializationVersion &&
            Description.SafeEqualsTo(other.Description) &&
            EnumerationAsFlags == other.EnumerationAsFlags &&
            CompareEnumeration(other);

        public override int GetHashCode() {
            var hash = new HashCode();
            hash.Add(Cast);
            hash.Add(Description);
            hash.Add(ElementTagId);
            hash.Add(EnumerationDefinition);
            hash.Add(EnumerationAsFlags);
            hash.Add(IsOpaque);
            hash.Add(IsOptional);
            hash.Add(IsVersion);
            hash.Add(Name);
            hash.Add(SerializationVersion);
            hash.Add(SubDataFields);
            hash.Add(TagId);
            hash.Add(Version);
            return hash.ToHashCode();
        }

        public bool IsVisibleMatch(string name) => Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) && !IsOpaque.GetValueOrDefault();

        public override string ToString() => $"{Name} #{TagId} {EnumerationDefinition?.Values.JoinedBy(",")}";

        private bool _isFlags => EnumerationAsFlags.GetValueOrDefault();

        private bool CompareEnumeration(DataField other) => EnumerationDefinition.SafeSequenceEqual(other.EnumerationDefinition);
    }

    public class ILTagDataField : ILTagExplicit<DataField>
    {
        public ILTagDataField(DataField field) : base(ILTagId.DataField, field) {
        }

        public ILTagDataField(Stream s) : base(ILTagId.DataField, s) {
        }

        protected override DataField FromBytes(byte[] bytes) {
            ushort serVersion = 0;
            return FromBytesHelper(bytes, s => new DataField {
                Version = s.DecodeUShort(),
                TagId = s.DecodeILInt(),
                Name = s.DecodeString(),
                IsOptional = s.DecodeNullableBool(),
                IsOpaque = s.DecodeNullableBool(),
                ElementTagId = s.DecodeNullableILInt(),
                SubDataFields = s.DecodeTagArray<ILTagDataField>()?.Select(t => t.Value),
                Cast = s.HasBytes() ? (CastType?)s.DecodeNullableByte() : null,
                SerializationVersion = (serVersion = s.HasBytes() ? s.DecodeUShort() : (ushort)0),
                Description = (serVersion > 1) ? s.DecodeString() : null,
                EnumerationDefinition = (serVersion > 2) ? DecodeEnumeration(s) : null,
                EnumerationAsFlags = (serVersion > 3) ? s.DecodeNullableBool() : null,
            });
        }

        protected override byte[] ToBytes()
            => ToBytesHelper(s => {
                s.EncodeUShort(Value.Version);
                s.EncodeILInt(Value.TagId);
                s.EncodeString(Value.Name);
                s.EncodeBool(Value.IsOptional.GetValueOrDefault());
                s.EncodeBool(Value.IsOpaque.GetValueOrDefault());
                s.EncodeILInt(Value.ElementTagId.GetValueOrDefault());
                s.EncodeTagArray(Value.SubDataFields?.Select(df => new ILTagDataField(df)));
                s.EncodeByte((byte)Value.Cast.GetValueOrDefault());
                s.EncodeUShort(Value.SerializationVersion);
                s.EncodeString(Value.Description);
                EncodeEnumeration(s, Value.EnumerationDefinition);
                s.EncodeBool(Value.EnumerationAsFlags.GetValueOrDefault());
            });

        private static EnumerationDictionary DecodeEnumeration(Stream s) {
            var triplets = s.DecodeArray<Triplet, Triplet.Tag>(s => new Triplet.Tag(s));
            return new EnumerationDictionary(triplets.SkipNulls().ToDictionary(t => t.Value, t => new EnumerationDetails(t.Name, t.Description)));
        }

        private static void EncodeEnumeration(Stream s, EnumerationDictionary enumeration)
            => s.EncodeTagArray(enumeration?.Select(p => new Triplet(p.Key, p.Value.Name, p.Value.Description).AsTag));

        private class Triplet : EnumerationDetails
        {
            public readonly ulong Value;

            public Triplet(ulong value, string name, string description) : base(name, description) => Value = value;

            public Tag AsTag => new Tag(this);

            public class Tag : ILTagExplicit<Triplet>
            {
                public Tag(Triplet v) : base(0, v) {
                }

                public Tag(Stream s) : base(s.DecodeTagId(), s) {
                }

                protected override Triplet FromBytes(byte[] bytes) => FromBytesHelper(bytes, s => new Triplet(s.DecodeILInt(), s.DecodeString(), s.DecodeString()));

                protected override byte[] ToBytes() => ToBytesHelper(s => {
                    s.EncodeILInt(Value.Value);
                    s.EncodeString(Value.Name);
                    s.EncodeString(Value.Description);
                });
            }
        }
    }
}
