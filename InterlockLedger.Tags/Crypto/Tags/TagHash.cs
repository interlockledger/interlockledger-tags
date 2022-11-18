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

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace InterlockLedger.Tags;

[TypeConverter(typeof(TypeCustomConverter<TagHash>))]
[JsonConverter(typeof(JsonCustomConverter<TagHash>))]
public sealed partial class TagHash : ILTagExplicit<TagHashParts>, ITextual<TagHash>
{
    public static TagHash Empty { get; }  = new(HashAlgorithm.SHA256, HashSha256(Array.Empty<byte>()));

    public TagHash(HashAlgorithm algorithm, byte[]? data) : this(new TagHashParts { Algorithm = algorithm, Data = data }) { }

    public HashAlgorithm Algorithm => Value.Algorithm;
    public override object AsJson => TextualRepresentation;
    public byte[]? Data => Value.Data;
    public bool IsEmpty => Data.EqualTo(Empty.Data);
    public bool IsInvalid => Data.None();
    public static Regex Mask { get; } = AnythingRegex();
    public static string MessageForMissing { get; } = "No hash";
    public string? InvalidityCause { get; private init; }

    public static TagHash Parse(string s, IFormatProvider? provider) => FromString(s);
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out TagHash result) =>
        ITextual<TagHash>.TryParse(s, out result);
    public static string MessageForInvalid(string? textualRepresentation) => $"Invalid hash '{textualRepresentation}'";

    public static TagHash FromString(string textualRepresentation) => new(Split(textualRepresentation.Safe().Trim()));
    public static TagHash InvalidBy(string cause) => new(HashAlgorithm.SHA1, Array.Empty<byte>() ) { InvalidityCause = cause };

    public static TagHash HashSha256Of(byte[] data) => new(HashAlgorithm.SHA256, HashSha256(data));

    public static TagHash HashSha256Of(IEnumerable<byte> data) => HashSha256Of(data.ToArray());

    public static implicit operator string(TagHash Value) => Value.ToString();

    public static bool operator !=(TagHash a, TagHash b) => !(a == b);

    public static bool operator ==(TagHash a, TagHash b) => a?.Equals(b) ?? b is null;

    public override bool Equals(object? obj) => Equals(obj as TagHash);

    public bool Equals(TagHash? other) => _traits.EqualsForAnyInstances(other);

    private ITextual<TagHash> _traits => this;

    public bool EqualsForValidInstances(TagHash other) => Algorithm == other.Algorithm && DataEquals(other.Data);

    public override int GetHashCode() => HashCode.Combine(_dataHashCode, Algorithm);

    public override string ToString() => TextualRepresentation;

    internal TagHash(Stream s) : base(ILTagId.Hash, s) => TextualRepresentation = $"{Data?.ToSafeBase64() ?? ""}#{Algorithm}";
  
    internal static TagHash HashFrom(X509Certificate2 certificate)
        => new(HashAlgorithm.SHA1, certificate.Required().GetCertHash());

    protected override TagHashParts FromBytes(byte[] bytes) =>
        FromBytesHelper(bytes, s => new TagHashParts {
            Algorithm = (HashAlgorithm)s.BigEndianReadUShort(),
            Data = s.ReadBytes(bytes.Length - sizeof(ushort))
        });

    protected override byte[] ToBytes(TagHashParts value)
        => TagHelpers.ToBytesHelper(s => s.BigEndianWriteUShort((ushort)value.Algorithm).WriteBytes(Data.OrEmpty()));

    private TagHash(TagHashParts parts) : base(ILTagId.Hash, parts) => TextualRepresentation = $"{Data.OrEmpty().ToSafeBase64()}#{Algorithm}";

    private int _dataHashCode => Data?.Aggregate(19, (sum, b) => sum + b) ?? 19;

    private static byte[] HashSha256(byte[] data) {
        using var hasher = SHA256.Create();
        hasher.Initialize();
        return hasher.ComputeHash(data);
    }

    private static TagHashParts Split(string textualRepresentation) {
        var parts = textualRepresentation.Split('#');
        var algorithm = parts.Length < 2 ? HashAlgorithm.SHA256 : (HashAlgorithm)Enum.Parse(typeof(HashAlgorithm), parts[1], ignoreCase: true);
        return new TagHashParts { Algorithm = algorithm, Data = parts[0].FromSafeBase64() };
    }

    private bool DataEquals(byte[]? otherData) => Data.None() && otherData.None() || Data.OrEmpty().HasSameBytesAs(otherData.OrEmpty());
    
    [GeneratedRegex(".+")]
    private static partial Regex AnythingRegex();
 }
