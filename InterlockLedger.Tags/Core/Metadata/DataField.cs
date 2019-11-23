/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

#pragma warning disable CA2227 // Collection properties should be read only

namespace InterlockLedger.Tags
{
    public sealed class DataField : IEquatable<DataField>
    {
        public const ushort CurrentVersion = 4;

        public DataField(string name, ulong tagId, string description = null) {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Need a name", nameof(name));
            Description = description;
            Name = name;
            TagId = tagId;
            Version = 1;
        }

        public DataField() => Version = 1;

        [JsonProperty(Order = 5)]
        public CastType Cast { get; set; }

        [JsonProperty(Order = 8)]
        public string Description { get; set; }

        // for ArrayOfTags
        [JsonProperty(Order = 6)]
        public ulong ElementTagId { get; set; }

        [JsonProperty(Order = 10, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<ulong, Pair> Enumeration { get; set; }

        [JsonProperty(Order = 11, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool EnumerationAsFlags { get; set; }

        // tags that have children
        [JsonIgnore]
        public bool HasSubFields => SubDataFields.SafeAny();

        [JsonIgnore]
        public bool IsEnumeration => Enumeration.SafeAny();

        // treat as opaque byte array
        [JsonProperty(Order = 3)]
        public bool IsOpaque { get; set; }

        [JsonProperty(Order = 2)]
        public bool IsOptional { get; set; }

        [JsonIgnore]
        public bool IsVersion => Name == "Version" && TagId == ILTagId.UInt16;

        // Case-insensitive (can't contain dots or whitespace)
        [JsonProperty(Order = 1)]
        public string Name { get; set; }

        [JsonProperty(Order = 9)]
        public ushort SerializationVersion { get; set; } = CurrentVersion;

        [JsonProperty(Order = 4)]
        public IEnumerable<DataField> SubDataFields { get; set; }

        [JsonProperty(Order = 0)]
        public ulong TagId { get; set; }

        [JsonProperty(Order = 7)]
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

        public string Enumerated(ILTag value) => Enumerated(AsNumber(value));

        public string Enumerated(ulong number) {
            try {
                if (Enumeration is null || Enumeration.Count == 0)
                    return "?";
                if (!EnumerationAsFlags)
                    return Enumeration.ContainsKey(number) ? Enumeration[number].Name : "?";
                return Enumeration.Keys.Where(k => (number & k) == k && k > 0).Select(k => Enumeration[k].Name).JoinedBy("|");
            } catch {
                return "*error*";
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
            var hashCode = 679_965_117;
            hashCode = hashCode * -1_521_134_295 + TagId.GetHashCode();
            hashCode = hashCode * -1_521_134_295 + IsOptional.GetHashCode();
            hashCode = hashCode * -1_521_134_295 + IsOpaque.GetHashCode();
            hashCode = hashCode * -1_521_134_295 + EqualityComparer<string>.Default.GetHashCode(Name ?? string.Empty);
            hashCode = hashCode * -1_521_134_295 + EqualityComparer<IEnumerable<DataField>>.Default.GetHashCode(SubDataFields ?? Enumerable.Empty<DataField>());
            hashCode = hashCode * -1_521_134_295 + Cast.GetHashCode();
            hashCode = hashCode * -1_521_134_295 + ElementTagId.GetHashCode();
            hashCode = hashCode * -1_521_134_295 + Version.GetHashCode();
            hashCode = hashCode * -1_521_134_295 + SerializationVersion.GetHashCode();
            hashCode = hashCode * -1_521_134_295 + EqualityComparer<string>.Default.GetHashCode(Description ?? string.Empty);
            hashCode = hashCode * -1_521_134_295 + (Enumeration ?? _noEnumeration).GetHashCode();
            return hashCode;
        }

        public override string ToString() => $"{Name} #{TagId} {Enumeration?.Values.JoinedBy(",")}";

        public class Pair : IEquatable<Pair>
        {
            public readonly string Description;
            public readonly string Name;

            public Pair(string name, string description) {
                Description = description ?? throw new ArgumentNullException(nameof(description));
                Name = name ?? throw new ArgumentNullException(nameof(name));
            }

            public override bool Equals(object obj) => Equals(obj as Pair);

            public bool Equals(Pair other) => other != null && Description == other.Description && Name == other.Name;

            public override int GetHashCode() {
                var hashCode = -1174144137;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Description);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
                return hashCode;
            }

            public override string ToString() => $"{Name}: {Description}";
        }

        private static readonly IEnumerable<KeyValuePair<ulong, Pair>> _noEnumeration = Enumerable.Empty<KeyValuePair<ulong, Pair>>();

        private bool CompareEnumeration(DataField other) => Enumeration.SafeAny() ? Enumeration.SequenceEqual(other.Enumeration ?? _noEnumeration) : !other.Enumeration.SafeAny();
    }

    public class ILTagDataField : ILTagExplicit<DataField>
    {
        public ILTagDataField(DataField field) : base(ILTagId.DataField, field) {
        }

        public ILTagDataField(Stream s) : base(ILTagId.DataField, s) {
        }

        protected override DataField FromBytes(byte[] bytes) {
            ushort serVersion = 0;
            return FromBytesHelper(bytes, s => {
                return new DataField {
                    Version = s.DecodeUShort(),
                    TagId = s.DecodeILInt(),
                    Name = s.DecodeString(),
                    IsOptional = s.DecodeBool(),
                    IsOpaque = s.DecodeBool(),
                    ElementTagId = s.DecodeILInt(),
                    SubDataFields = s.DecodeTagArray<ILTagDataField>()?.Select(t => t.Value),
                    Cast = s.HasBytes() ? (CastType)s.DecodeByte() : CastType.None,
                    SerializationVersion = (serVersion = s.HasBytes() ? s.DecodeUShort() : (ushort)0),
                    Description = (serVersion > 1) ? s.DecodeString() : null,
                    Enumeration = (serVersion > 2) ? DecodeEnumeration(s) : null,
                    EnumerationAsFlags = (serVersion > 3) && s.DecodeBool()
                };
            });
        }

        protected override byte[] ToBytes()
            => ToBytesHelper(s => {
                s.EncodeUShort(Value.Version);
                s.EncodeILInt(Value.TagId);
                s.EncodeString(Value.Name);
                s.EncodeBool(Value.IsOptional);
                s.EncodeBool(Value.IsOpaque);
                s.EncodeILInt(Value.ElementTagId);
                s.EncodeTagArray(Value.SubDataFields?.Select(df => new ILTagDataField(df)));
                s.EncodeByte((byte)Value.Cast);
                s.EncodeUShort(Value.SerializationVersion);
                s.EncodeString(Value.Description);
                EncodeEnumeration(s, Value.Enumeration);
                s.EncodeBool(Value.EnumerationAsFlags);
            });

        private Dictionary<ulong, DataField.Pair> DecodeEnumeration(Stream s) {
            var triplets = s.DecodeArray<Triplet, Triplet.Tag>(s => new Triplet.Tag(s));
            return triplets.SkipNulls().ToDictionary(t => t.Value, t => new DataField.Pair(t.Name, t.Description));
        }

        private void EncodeEnumeration(Stream s, Dictionary<ulong, DataField.Pair> enumeration)
            => s.EncodeTagArray(enumeration?.Select(p => new Triplet(p.Key, p.Value.Name, p.Value.Description).AsTag));

        private class Triplet : DataField.Pair
        {
            public readonly ulong Value;

            public Triplet(ulong value, string name, string description) : base(name, description) => Value = value;

            public Tag AsTag => new Tag(this);

            public class Tag : ILTagExplicit<Triplet>
            {
                public Tag(Triplet v) : base(0, v) { }

                public Tag(Stream s) : base(s.DecodeTagId(), s) { }

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