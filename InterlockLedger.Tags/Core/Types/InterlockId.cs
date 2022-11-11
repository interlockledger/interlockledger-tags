// ******************************************************************************************************************************
//
// Copyright (c) 2018-2021 InterlockLedger Network
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

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace InterlockLedger.Tags;
[TypeConverter(typeof(InterlockIdConverter))]
[JsonConverter(typeof(JsonInterlockIdConverter))]
public class InterlockId : ILTagExplicit<InterlockId.Parts>, IEquatable<InterlockId>, IComparable<InterlockId>
{
    public static IEnumerable<string> AllTypes => Parts.AllTypes;

    public static HashAlgorithm DefaultAlgorithm => Parts.DefaultAlgorithm;

    public static byte DefaultType {
        get => Parts.DefaultType;
        set {
            if (Parts.DefaultType != 0 && Parts.DefaultType != value)
                throw new InvalidOperationException("InterlockId default type is already set");
            Parts.DefaultType = value;
        }
    }

    public HashAlgorithm Algorithm => Value.Algorithm;
    public override object AsJson => TextualRepresentation;
    public byte[] Data => Value.Data;
    public override string Formatted => ToString();
    public bool IsEmpty => Data.HasSameBytesAs(TagHash.Empty.Data);
    public string TextualRepresentation => ToString();
    public byte Type => Value.Type;

    public static bool operator !=(InterlockId a, InterlockId b) => !(a == b);

    public static bool operator <(InterlockId a, InterlockId b) => SafeCompare(a, b) < 0;

    public static bool operator <=(InterlockId a, InterlockId b) => SafeCompare(a, b) <= 0;

    public static bool operator ==(InterlockId a, InterlockId b) => a?.Equals(b) ?? b is null;

    public static bool operator >(InterlockId a, InterlockId b) => SafeCompare(a, b) > 0;

    public static bool operator >=(InterlockId a, InterlockId b) => SafeCompare(a, b) >= 0;

    public static InterlockId Resolve(string textualRepresentation) => new Parts(textualRepresentation).Resolve();

    public static InterlockId Resolve(Stream s) => new Parts(s).Resolve();

    public int CompareTo(InterlockId other) => string.CompareOrdinal(ToFullString(), other?.ToFullString());

    public override bool Equals(object obj) => Equals(obj as InterlockId);

    public bool Equals(InterlockId other) => (other is not null) && Type == other.Type && Algorithm == other.Algorithm && DataEquals(other.Data);

    public override int GetHashCode() => -1_574_110_226 + _dataHashCode + Algorithm.GetHashCode();

    public string ToFullString() => Value.ToFullString();

    public override string ToString() => Value.ToShortString();

    public sealed class Parts
    {
        public HashAlgorithm Algorithm;
        public byte[] Data;
        public byte Type;

        public Parts() { }

        public override string ToString() => ToShortString();

        internal static byte DefaultType;

        internal Parts(Stream s) {
            if (s.ILIntDecode() != ILTagId.InterlockId)
                throw new InvalidDataException($"This is not an {nameof(InterlockId)}");
            FromStream(s, (int)s.ILIntDecode());
        }

        internal Parts(HashAlgorithm algorithm, byte[] data, byte type) {
            Algorithm = algorithm;
            Data = data.Required();
            Type = type;
        }

        internal Parts(string textualRepresentation) {
            static HashAlgorithm ToHashAlgorithm(string suffix) => (HashAlgorithm)Enum.Parse(typeof(HashAlgorithm), suffix, ignoreCase: true);

            static (string strippedOfSuffix, HashAlgorithm algorithm) ParseSuffix(string textualRepresentation) {
                var parts = textualRepresentation.Split(_suffixSeparator);
                return (parts[0], algorithm: parts.Length < 2 ? _defaultAlgorithm : ToHashAlgorithm(parts[1]));
            }
            (string strippedOfSuffix, var algorithm) = ParseSuffix(textualRepresentation);
            var parts = strippedOfSuffix.Split(_prefixSeparator);
            Algorithm = algorithm;
            if (parts.Length == 1) {
                Data = parts[0].FromSafeBase64();
                Type = DefaultType;
            } else {
                Data = parts[1].FromSafeBase64();
                Type = ToType(parts[0]);
            }
        }

        internal Parts(Stream s, int length) => FromStream(s, length);

        internal static IEnumerable<string> AllTypes => _knownTypes.OrderBy(p => p.Key).Select(p => $"#{p.Key} - {p.Value.typeName} {(p.Key == DefaultType ? "[default] (if prefix is ommited)" : string.Empty)}");

        internal static HashAlgorithm DefaultAlgorithm => _defaultAlgorithm;

        internal static void RegisterResolver(byte type, string typeName, Func<Parts, InterlockId> resolver)
            => _knownTypes[type] = (typeName, resolver.Required());

        internal InterlockId Resolve()
            => _knownTypes.TryGetValue(Type, out var value)
                ? value.resolver(this)
                : throw new InvalidDataException($"Could not match this InterlockId type {Type}");

        internal string ToFullString() => $"{_typePrefix}{_dataInfix}{_algorithmSuffix}";

        internal string ToShortString() => $"{_conditionalTypePrefix}{_dataInfix}{_conditionalAlgorithmSuffix}";

        internal void ToStream(Stream s) => s.WriteSingleByte(Type).BigEndianWriteUShort((ushort)Algorithm).WriteBytes(Data);

        private const HashAlgorithm _defaultAlgorithm = HashAlgorithm.SHA256;
        private const char _prefixSeparator = '!';
        private const char _suffixSeparator = '#';

        private static readonly Dictionary<byte, (string typeName, Func<Parts, InterlockId> resolver)> _knownTypes =
            new();

        private string _algorithmSuffix => $"{_suffixSeparator}{Algorithm}";
        private string _conditionalAlgorithmSuffix => Algorithm == _defaultAlgorithm ? string.Empty : _algorithmSuffix;
        private string _conditionalTypePrefix => Type == DefaultType ? string.Empty : _typePrefix;
        private string _dataInfix => Data?.ToSafeBase64() ?? string.Empty;
        private string _typePrefix => BuildTypePrefix(Type);

        private static string BuildTypePrefix(byte type) => $"{ToTypeName(type)}{_prefixSeparator}";

        private static byte ToType(string prefix) => _knownTypes.First(t => t.Value.typeName.Equals(prefix.Trim(), StringComparison.InvariantCultureIgnoreCase)).Key;

        private static string ToTypeName(byte type) => _knownTypes.TryGetValue(type, out var value) ? value.typeName : "?";

        private void FromStream(Stream s, int length) {
            Type = s.ReadSingleByte();
            Algorithm = (HashAlgorithm)s.BigEndianReadUShort();
            Data = s.ReadBytes(length - sizeof(ushort) - 1);
        }
    }

    internal static ILTag DeserializeAndResolve(Stream s) => new Parts(s, (int)s.ILIntDecode()).Resolve();

    protected InterlockId(string textualRepresentation) : this(new Parts(textualRepresentation)) {
    }

    protected InterlockId(byte type, TagHash hash) : this(type, hash.Required().Algorithm, hash.Data) {
    }

    protected InterlockId(byte type, HashAlgorithm algorithm, byte[] data) : this(new Parts(algorithm, data, type)) {
    }

    protected InterlockId(Parts parts) : base(ILTagId.InterlockId, parts) {
    }

    protected static void RegisterResolver(byte type, string typeName, Func<Parts, InterlockId> resolver) =>
        Parts.RegisterResolver(type, typeName.Required(), resolver.Required());

    protected void CheckType(byte type) {
        if (Type != type)
            throw new InvalidDataException($"This is not a {GetType().Name}");
    }

    protected override Parts FromBytes(byte[] bytes) => FromBytesHelper(bytes, s => new Parts(s, bytes.Length));

    protected override byte[] ToBytes(Parts value) => TagHelpers.ToBytesHelper(s => value.ToStream(s));

    private int _dataHashCode => Data?.Aggregate(19, (sum, b) => sum + b) ?? 19;

    private static bool IsNullOrEmpty(byte[] data) => data is null || data.Length == 0;

    private static int SafeCompare(InterlockId a, InterlockId b) => (a == null) ? ((b == null) ? 0 : -1) : a.CompareTo(b);

    private bool DataEquals(byte[] otherData) => (IsNullOrEmpty(Data) && IsNullOrEmpty(otherData)) || Data.HasSameBytesAs(otherData);
}

public class InterlockIdConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        => destinationType == typeof(InstanceDescriptor) || destinationType == typeof(string);

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object Value) {
        if (Value is string text) {
            text = text.Trim();
            return InterlockId.Resolve(text);
        }
        return base.ConvertFrom(context, culture, Value);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object Value, Type destinationType)
        => destinationType.Required() == typeof(string) && Value is InterlockId id
            ? id.TextualRepresentation
            : throw new InvalidOperationException("Can only convert InterlockId to string!!!");
}