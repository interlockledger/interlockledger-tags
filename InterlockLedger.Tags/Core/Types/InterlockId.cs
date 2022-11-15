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

#nullable enable

using System.ComponentModel.Design.Serialization;

namespace InterlockLedger.Tags;
[TypeConverter(typeof(InterlockIdConverter))]
[JsonConverter(typeof(JsonInterlockIdConverter))]
public partial class InterlockId : ILTagExplicit<InterlockId.Parts>, IComparable<InterlockId>, ITextual<InterlockId>
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

    public static InterlockId FromString(string textualRepresentation) => new Parts(textualRepresentation).Resolve();

    public static InterlockId Resolve(Stream s) => new Parts(s).Resolve();

    public int CompareTo(InterlockId? other) => string.CompareOrdinal(ToFullString(), other?.ToFullString());

    public override bool Equals(object? obj) => Equals(obj as InterlockId);

    public bool Equals(InterlockId? other) => false;
    public bool EqualsForValidInstances(InterlockId other) =>
        Type == other.Type && Algorithm == other.Algorithm && DataEquals(other.Data);

    public static InterlockId InvalidBy(string cause) => new(byte.MaxValue, HashAlgorithm.SHA1, Array.Empty<byte>()) {
        InvalidityCause = cause
    };

    public override int GetHashCode() => HashCode.Combine(Type, _dataHashCode, Algorithm);

    public string ToFullString() => Value.ToFullString();

    public override string ToString() => Value.ToShortString();

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

    public static InterlockId Empty { get; } = new(DefaultType, TagHash.Empty);
    public bool IsInvalid => InvalidityCause is not null;

    public static Regex Mask { get; } = InterlockIdRegex();
    public static string MessageForMissing { get; } = "Missing id";
    public string? InvalidityCause { get; private init; }

    private static bool IsNullOrEmpty(byte[] data) => data is null || data.Length == 0;

    private static int SafeCompare(InterlockId? a, InterlockId? b) => a is null ? b is null ? 0 : -1 : a.CompareTo(b);

    private bool DataEquals(byte[] otherData) => IsNullOrEmpty(Data) && IsNullOrEmpty(otherData) || Data.HasSameBytesAs(otherData);
    public static InterlockId Parse(string s, IFormatProvider? provider) =>
        ITextual<InterlockId>.Resolve(s);
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out InterlockId result) =>
        ITextual<InterlockId>.TryParse(s, out result);
    static string ITextual<InterlockId>.MessageForInvalid(string? textualRepresentation) => $"Invalid id '{textualRepresentation}'";

    [GeneratedRegex("""^(\w+\!)?(?:[A-Za-z0-9_-]{4}?)*(?:[A-Za-z0-9_-]{2,3})?(#\w+)?$""")]
    private static partial Regex InterlockIdRegex();
}

public class InterlockIdConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => sourceType == typeof(string);

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        => destinationType == typeof(InstanceDescriptor) || destinationType == typeof(string);

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object Value) {
        if (Value is string text) {
            text = text.Trim();
            return InterlockId.FromString(text);
        }
        return base.ConvertFrom(context, culture, Value);
    }

    public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? Value, Type destinationType)
        => destinationType.Required() == typeof(string) && Value is InterlockId id
            ? id.TextualRepresentation
            : throw new InvalidOperationException("Can only convert InterlockId to string!!!");
}