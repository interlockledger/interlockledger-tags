/******************************************************************************************************************************
 
Copyright (c) 2018-2019 InterlockLedger Network
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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;

namespace InterlockLedger.Tags
{
    [TypeConverter(typeof(InterlockIdConverter))]
    [JsonConverter(typeof(JsonInterlockIdConverter))]
    public class InterlockId : ILTagExplicit<InterlockIdParts>, IEquatable<InterlockId>, IComparable<InterlockId>
    {
        public static byte DefaultType {
            get => InterlockIdParts.DefaultType;
            set {
                if (InterlockIdParts.DefaultType != 0 && InterlockIdParts.DefaultType != value)
                    throw new InvalidOperationException("InterlockId default type is already set");
                InterlockIdParts.DefaultType = value;
            }
        }

        public override object AsJson => TextualRepresentation;
        public override string Formatted => ToString();
        public bool IsEmpty => Data.HasSameBytesAs(TagHash.Empty.Data);
        public string TextualRepresentation => ToString();
        public static IEnumerable<string> AllTypes => InterlockIdParts.AllTypes;
        public static HashAlgorithm DefaultAlgorithm => InterlockIdParts.DefaultAlgorithm;
        public HashAlgorithm Algorithm => Value.Algorithm;
        public byte[] Data => Value.Data;
        public byte Type => Value.Type;

        public static bool operator !=(InterlockId a, InterlockId b) => !(a == b);

        public static bool operator <(InterlockId a, InterlockId b) => SafeCompare(a, b) < 0;

        public static bool operator <=(InterlockId a, InterlockId b) => SafeCompare(a, b) <= 0;

        public static bool operator ==(InterlockId a, InterlockId b) => a?.Equals(b) ?? b is null;

        public static bool operator >(InterlockId a, InterlockId b) => SafeCompare(a, b) > 0;

        public static bool operator >=(InterlockId a, InterlockId b) => SafeCompare(a, b) >= 0;

        public static InterlockId Resolve(string textualRepresentation) => new InterlockIdParts(textualRepresentation).Resolve();

        public static InterlockId Resolve(Stream s) => new InterlockIdParts(s).Resolve();

        public int CompareTo(InterlockId other) => string.CompareOrdinal(ToFullString(), other?.ToFullString());

        public override bool Equals(object obj) => Equals(obj as InterlockId);

        public bool Equals(InterlockId other) => (!(other is null)) && Type == other.Type && Algorithm == other.Algorithm && DataEquals(other.Data);

        public override int GetHashCode() => -1_574_110_226 + _dataHashCode + Algorithm.GetHashCode();

        public string ToFullString() => Value.ToFullString();

        public override string ToString() => Value.ToShortString();

        internal static ILTag DeserializeAndResolve(Stream s) => new InterlockIdParts(s, (int)s.ILIntDecode()).Resolve();

        protected InterlockId(string textualRepresentation) : this(new InterlockIdParts(textualRepresentation)) {
        }

        protected InterlockId(byte type, TagHash hash) : this(type, hash?.Algorithm ?? throw new ArgumentNullException(nameof(hash)), hash.Data) {
        }

        protected InterlockId(byte type, HashAlgorithm algorithm, byte[] data) : this(new InterlockIdParts(algorithm, data, type)) {
        }

        protected InterlockId(InterlockIdParts parts) : base(ILTagId.InterlockId, parts) {
        }

        protected static void RegisterResolver(byte type, string typeName, Func<InterlockIdParts, InterlockId> resolver) {
            if (string.IsNullOrWhiteSpace(typeName))
                throw new ArgumentNullException(nameof(typeName));
            if (resolver is null)
                throw new ArgumentNullException(nameof(resolver));
            InterlockIdParts.RegisterResolver(type, typeName, resolver);
        }

        protected void CheckType(byte type) {
            if (Type != type)
                throw new InvalidDataException($"This is not a {GetType().Name}");
        }

        protected override InterlockIdParts FromBytes(byte[] bytes) => FromBytesHelper(bytes, s => new InterlockIdParts(s, bytes.Length));

        protected override byte[] ToBytes() => ToBytesHelper(s => Value.ToStream(s));

        private int _dataHashCode => Data?.Aggregate(19, (sum, b) => sum + b) ?? 19;

        private static bool IsNullOrEmpty(byte[] data) => data is null || data.Length == 0;

        private static int SafeCompare(InterlockId a, InterlockId b) => (a == null) ? ((b == null) ? 0 : -1) : a.CompareTo(b);

        private bool DataEquals(byte[] otherData) => (IsNullOrEmpty(Data) && IsNullOrEmpty(otherData)) || Data.HasSameBytesAs(otherData);
    }

    public class InterlockIdConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            if (destinationType == typeof(InstanceDescriptor)) {
                return true;
            }
            return destinationType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object Value) {
            if (Value is string text) {
                text = text.Trim();
                return InterlockId.Resolve(text);
            }
            return base.ConvertFrom(context, culture, Value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object Value, Type destinationType) {
            if (destinationType == null)
                throw new ArgumentNullException(nameof(destinationType));
            if (Value == null)
                throw new ArgumentNullException(nameof(Value));
            if (destinationType != typeof(string) || !(Value is InterlockId))
                throw new InvalidOperationException("Can only convert InterlockId to string!!!");
            return Value.ToString();
        }
    }

    public sealed class InterlockIdParts
    {
        public HashAlgorithm Algorithm;
        public byte[] Data;
        public byte Type;

        public InterlockIdParts() { }

        public override string ToString() => ToShortString();

        internal static byte DefaultType;

        internal InterlockIdParts(Stream s) {
            if (s.ILIntDecode() != ILTagId.InterlockId)
                throw new InvalidDataException($"This is not an {nameof(InterlockId)}");
            FromStream(s, (int)s.ILIntDecode());
        }

        internal InterlockIdParts(HashAlgorithm algorithm, byte[] data, byte type) {
            Algorithm = algorithm;
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Type = type;
        }

        internal InterlockIdParts(string textualRepresentation) {
            static HashAlgorithm ToHashAlgorithm(string suffix) => (HashAlgorithm)Enum.Parse(typeof(HashAlgorithm), suffix, ignoreCase: true);
            static (string strippedOfSuffix, HashAlgorithm algorithm) ParseSuffix(string textualRepresentation) {
                var parts = textualRepresentation.Split(_suffixSeparator);
                return (parts[0], algorithm: parts.Length < 2 ? _defaultAlgorithm : ToHashAlgorithm(parts[1]));
            }
            (string strippedOfSuffix, var algorithm) = ParseSuffix(NormalizePrefix(textualRepresentation));
            var parts = strippedOfSuffix.Split(_prefixSeparator);
            Algorithm = algorithm;
            Data = parts[1].FromSafeBase64();
            Type = ToType(parts[0]);
        }

        internal InterlockIdParts(Stream s, int length) => FromStream(s, length);

        internal static IEnumerable<string> AllTypes => _knownTypes.OrderBy(p => p.Key).Select(p => $"#{p.Key} - {p.Value.typeName} {(p.Key == DefaultType ? "[default] (if prefix is ommited)" : string.Empty)}");

        internal static HashAlgorithm DefaultAlgorithm => _defaultAlgorithm;

        internal static void RegisterResolver(byte type, string typeName, Func<InterlockIdParts, InterlockId> resolver) {
            if (resolver is null)
                throw new ArgumentNullException(nameof(resolver));
            _knownTypes[type] = (typeName, resolver);
        }

        internal InterlockId Resolve() {
            if (_knownTypes.ContainsKey(Type))
                return _knownTypes[Type].resolver(this);
            throw new InvalidDataException($"Could not match this InterlockId type {Type}");
        }

        internal string ToFullString() => $"{_typePrefix}{_dataInfix}{_algorithmSuffix}";

        internal string ToShortString() => $"{_conditionalTypePrefix}{_dataInfix}{_conditionalAlgorithmSuffix}";

        internal void ToStream(Stream s) => s.WriteSingleByte(Type).BigEndianWriteUShort((ushort)Algorithm).WriteBytes(Data);

        private const HashAlgorithm _defaultAlgorithm = HashAlgorithm.SHA256;
        private const char _prefixSeparator = '!';
        private const char _suffixSeparator = '#';

        private static readonly Dictionary<byte, (string typeName, Func<InterlockIdParts, InterlockId> resolver)> _knownTypes =
            new Dictionary<byte, (string typeName, Func<InterlockIdParts, InterlockId> resolver)>();

        private string _algorithmSuffix => $"{_suffixSeparator}{Algorithm}";
        private string _conditionalAlgorithmSuffix => Algorithm == _defaultAlgorithm ? string.Empty : _algorithmSuffix;
        private string _conditionalTypePrefix => Type == DefaultType ? string.Empty : _typePrefix;
        private string _dataInfix => Data?.ToSafeBase64() ?? string.Empty;
        private string _typePrefix => BuildTypePrefix(Type);

        private static string BuildTypePrefix(byte type) => $"{ToTypeName(type)}{_prefixSeparator}";

        private static string NormalizePrefix(string s) => s.Contains(_prefixSeparator) ? s : $"{BuildTypePrefix(DefaultType)}{s}";

        private static byte ToType(string prefix) => _knownTypes.First(t => t.Value.typeName.Equals(prefix.Trim(), StringComparison.InvariantCultureIgnoreCase)).Key;

        private static string ToTypeName(byte type) => _knownTypes.ContainsKey(type) ? _knownTypes[type].typeName : "?";

        private void FromStream(Stream s, int length) {
            Type = s.ReadSingleByte();
            Algorithm = (HashAlgorithm)s.BigEndianReadUShort();
            Data = s.ReadBytes(length - sizeof(ushort) - 1);
        }
    }
}