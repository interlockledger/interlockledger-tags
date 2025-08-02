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

namespace InterlockLedger.Tags;

public sealed partial class DataField : IEquatable<DataField>
{
    public const ushort CurrentVersion = 5;

    public DataField(string name, ulong tagId, string? description = null) : this() {
        name.Required();
        Description = description.TrimToNull();
        Name = name;
        TagId = tagId;
        Version = 1;
    }

    public DataField() => Name = "Invalid";

    public CastType Cast { get; set; }

    public string? Description { get; set; }

    public ulong ElementTagId { get; set; }

    public EnumerationItems? Enumeration {
        get => EnumerationDefinition?.AsEnumerationItems;
        set => EnumerationDefinition = value?.ToDefinition();
    }

    public bool EnumerationAsFlags { get; set; }

    [JsonIgnore]
    public EnumerationDictionary? EnumerationDefinition { get; set; }

    // tags that have children
    [JsonIgnore]
    public bool HasSubFields => SubDataFields.SafeAny();

    [JsonIgnore]
    public bool IsArray => ElementTagId > 0 && TagId == ILTagId.ILTagArray;

    public bool IsDeprecated { get; set; }

    [JsonIgnore]
    public bool IsEnumeration => EnumerationDefinition.SafeAny();

    // treat as opaque byte array
    public bool IsOpaque { get; set; }

    [JsonIgnore]
    public bool IsVersion => Name.Equals("Version", StringComparison.OrdinalIgnoreCase) && TagId == ILTagId.UInt16;

    // Case-insensitive (can't contain dots or whitespace)
    public string Name { get; set; }

    [JsonIgnore]
    public ushort SerializationVersion { get; set; } = CurrentVersion;

    public IEnumerable<DataField>? SubDataFields { get; set; }

    public ulong TagId { get; set; }

    public ushort Version { get; set; }

    public static ulong AsNumber(ILTag tag)
        => tag is null
            ? throw new ArgumentNullException(nameof(tag))
            : tag.TagId switch {
                2 => (ulong)((ILTagInt8)tag).Value,
                3 => ((ILTagUInt8)tag).Value,
                4 => (ulong)((ILTagInt16)tag).Value,
                5 => ((ILTagUInt16)tag).Value,
                6 => (ulong)((ILTagInt32)tag).Value,
                7 => ((ILTagUInt32)tag).Value,
                8 => (ulong)((ILTagInt64)tag).Value,
                9 => ((ILTagUInt64)tag).Value,
                10 => ((ILTagILInt)tag).Value,
                _ => throw new InvalidCastException("Not an integral numeric ILTag"),
            };

    public static ILTag AsNumericTag(ulong tagId, ulong value) => tagId switch {
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

    public static bool operator ==(DataField left, DataField right) => left.Equals(right);

    public ILTag EnumerationFromString(string value) {
        return string.IsNullOrWhiteSpace(value) || EnumerationDefinition is null || EnumerationDefinition.Count == 0 || value == "?"
            ? ILTagNull.Instance
            : AsNumericTag(TagId, Parse(value));

        ulong Parse(string value) {
            return !EnumerationAsFlags ? FindKey(value.Trim()) : SplitAndParse(value);

            ulong FindKey(string value) {
                try {
                    return NumbersRegex().IsMatch(value)
                        ? ulong.Parse(value[1..], CultureInfo.InvariantCulture)
                        : EnumerationDefinition.First(kp => kp.Value.Name.Equals(value, StringComparison.InvariantCultureIgnoreCase)).Key;
                } catch (Exception e) {
                    throw new InvalidDataException($"Value '{value}' is not valid for the enumeration for field [{Name}]", e);
                }
            }

            ulong SplitAndParse(string value) {
                ulong numericValue = 0;
                foreach (var partValue in value.Split(_separators, StringSplitOptions.TrimEntries))
                    numericValue |= FindKey(partValue.Trim());
                return numericValue;
            }
        }
    }
    private static readonly char[] _separators = ['|', ','];

    public string EnumerationToString(ILTag value) => EnumerationToString(AsNumber(value));

    public string EnumerationToString(ulong number) {
        try {
            return EnumerationDefinition is null || EnumerationDefinition.Count == 0
                ? "?"
                : EnumerationAsFlags
                    ? ToList(number)
                    : EnumerationDefinition.TryGetValue(number, out var value) ? value.Name : $"?{number}";
        } catch {
            return "*ERROR*";
        }

        string ToList(ulong number) {
            var names = new List<string>();
            foreach (ulong k in EnumerationDefinition.Keys) if (number == k || (number & k) == k && k > 0) {
                    number ^= k;
                    names.Add(EnumerationDefinition[k].Name);
                }
            if (number > 0)
                names.Add($"?{number}");
            return names.JoinedBy(", ");
        }
    }

    public override bool Equals(object? obj) => Equals(obj as DataField);

    public bool Equals(DataField? other) =>
        other is not null &&
        TagId == other.TagId &&
        Cast == other.Cast &&
        Description.Safe().SafeTrimmedEqualsTo(other.Description.Safe()) &&
        ElementTagId == other.ElementTagId &&
        EnumerationAsFlags == other.EnumerationAsFlags &&
        IsDeprecated == other.IsDeprecated &&
        IsOpaque == other.IsOpaque &&
        Name.SafeEqualsTo(other.Name) &&
        SerializationVersion == other.SerializationVersion &&
        SubDataFields.Safe().EqualTo(other.SubDataFields.Safe()) &&
        Version == other.Version &&
        CompareEnumeration(other);

    public override int GetHashCode() {
        var hash = new HashCode();
        hash.Add(Cast);
        hash.Add(Description.TrimToNull());
        hash.Add(ElementTagId);
        hash.Add(EnumerationAsFlags);
        hash.Add(EnumerationDefinition);
        hash.Add(IsDeprecated);
        hash.Add(IsOpaque);
        hash.Add(Name);
        hash.Add(SerializationVersion);
        hash.Add(SubDataFields);
        hash.Add(TagId);
        hash.Add(Version);
        return hash.ToHashCode();
    }

    public bool IsVisibleMatch(string name) => Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) && !IsOpaque;

    public override string ToString() => $"{Name} #{TagId} {EnumerationDefinition?.Values.JoinedBy(",")}";

    public DataField WithName(string newName) => new(this) {
        Name = newName.Required()
    };

    internal bool IsOptional_Deprecated;

    internal DataField(DataField source) {
        Cast = source.Required().Cast;
        Description = source.Description;
        ElementTagId = source.ElementTagId;
        EnumerationAsFlags = source.EnumerationAsFlags;
        EnumerationDefinition = source.EnumerationDefinition;
        IsDeprecated = source.IsDeprecated;
        IsOpaque = source.IsOpaque;
        Name = source.Name;
        SerializationVersion = source.SerializationVersion;
        SubDataFields = source.SubDataFields.NullIfEmpty();
        TagId = source.TagId;
        Version = source.Version;
    }

    private bool CompareEnumeration(DataField other) =>
        (EnumerationDefinition.None() && other.EnumerationDefinition.None()) ||
        ((EnumerationDefinition.SafeAny() && other.EnumerationDefinition.SafeAny()) && EnumerationDefinition.EquivalentTo(other.EnumerationDefinition));

    [GeneratedRegex("^\\?\\d+$")]
    private static partial Regex NumbersRegex();
}