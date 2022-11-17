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

namespace InterlockLedger.Tags;

[JsonConverter(typeof(JsonCustomConverter<TagHmac>))]
public sealed partial class TagHmac : ILTagExplicit<TagHashParts>, IEquatable<TagHmac>, ITextual<TagHmac>
{
    public static TagHmac HmacSha256Of(byte[] key, byte[] content) {
        using var hash = new HMACSHA256(key);
        return new TagHmac(HashAlgorithm.SHA256, hash.ComputeHash(content));
    }

    public TagHmac(HashAlgorithm algorithm, byte[]? data) : this(new TagHashParts { Algorithm = algorithm, Data = data }) {
    }

    public HashAlgorithm Algorithm => Value?.Algorithm ?? HashAlgorithm.SHA256;
    public byte[]? Data => Value?.Data;
    public bool IsEmpty => Data is not null && Data.None();
    public bool IsInvalid => Data is null;

    public override string ToString() => TextualRepresentation;

    public bool Equals(TagHmac other) => (other is not null) && Algorithm == other.Algorithm && DataEquals(other.Data);

    public override bool Equals(object? obj) => Equals(obj as TagHmac);

    public override int GetHashCode() => ToString().GetHashCode(StringComparison.InvariantCulture);

    public static TagHmac Empty { get; } = new TagHmac(HashAlgorithm.SHA256, Array.Empty<byte>());
    public static Regex Mask { get; } = AnythingRegex();
    public static string MessageForMissing { get; } = "No Hmac";
    public string? InvalidityCause { get; private init; }
    static TagHmac ITextual<TagHmac>.FromString(string textualRepresentation) => new(Split(textualRepresentation));
    static TagHmac ITextual<TagHmac>.InvalidBy(string cause) => new(HashAlgorithm.SHA256, null) { InvalidityCause = cause};
    bool ITextual<TagHmac>.EqualsForValidInstances(TagHmac other) => throw new NotImplementedException();

    public static TagHmac Parse(string s, IFormatProvider? provider) => ITextual<TagHmac>.Parse(s);
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out TagHmac result) =>
        ITextual<TagHmac>.TryParse(s, out result);
    public static string MessageForInvalid(string? textualRepresentation) => $"Invalid Hmac '{textualRepresentation}'";

    [GeneratedRegex(".+")]
    private static partial Regex AnythingRegex();

    private TagHmac(TagHashParts parts) : base(ILTagId.Hmac, parts) => TextualRepresentation = $"{Data?.ToSafeBase64() ?? ""}#HMAC-{Algorithm}";

    internal TagHmac(Stream s) : base(ILTagId.Hmac, s) => TextualRepresentation = $"{Data?.ToSafeBase64() ?? ""}#HMAC-{Algorithm}";


    protected override TagHashParts FromBytes(byte[] bytes) =>
        FromBytesHelper(bytes, s => new TagHashParts {
            Algorithm = (HashAlgorithm)s.BigEndianReadUShort(),
            Data = s.ReadBytes(bytes.Length - sizeof(ushort))
        });

    protected override byte[] ToBytes(TagHashParts value)
        => TagHelpers.ToBytesHelper(s => s.BigEndianWriteUShort((ushort)value.Algorithm).WriteBytes(Value.Data));

    private static bool IsNullOrEmpty(byte[] data) => data is null || data.Length == 0;

    private static TagHashParts Split(string textualRepresentation) {
        if (string.IsNullOrWhiteSpace(textualRepresentation))
            throw new ArgumentNullException(nameof(textualRepresentation));
        var parts = textualRepresentation.Split(new string[] { "#HMAC-" }, StringSplitOptions.None);
        var algorithm = parts.Length < 2 ? HashAlgorithm.SHA256 : (HashAlgorithm)Enum.Parse(typeof(HashAlgorithm), parts[1], ignoreCase: true);
        return new TagHashParts { Algorithm = algorithm, Data = parts[0].FromSafeBase64() };
    }

    private bool DataEquals(byte[] otherData) => (IsNullOrEmpty(Data) && IsNullOrEmpty(otherData)) || Data.HasSameBytesAs(otherData);

}