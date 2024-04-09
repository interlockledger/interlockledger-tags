// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2024 InterlockLedger Network
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


using System.Security.Cryptography;

namespace InterlockLedger.Tags;

[JsonConverter(typeof(JsonCustomConverter<TagHmac>))]
public sealed partial class TagHmac : ILTagOfExplicit<TagHash.Parts>, ITextual<TagHmac>
{
    private TagHmac() : this(HashAlgorithm.Invalid, []) { }
    public static TagHmac InvalidBy(string cause) =>
        new() { InvalidityCause = cause };
    public TagHmac(HashAlgorithm algorithm, byte[] data) : this(new TagHash.Parts { Algorithm = algorithm, Data = data }) { }

    public HashAlgorithm Algorithm => Value?.Algorithm ?? HashAlgorithm.Invalid;
    public byte[] Data => Value?.Data ?? [];
    public bool IsEmpty => Data is not null && Data.None();
    public string? InvalidityCause { get; init; }
    public override string ToString() => Textual.FullRepresentation();
    public ITextual<TagHmac> Textual => this;
    public static TagHmac Empty { get; } = new TagHmac(HashAlgorithm.SHA256, []);
    public static Regex Mask { get; } = AnythingRegex();

    private static readonly string[] _prefixSeparator = ["#HMAC-"];

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out TagHmac result) {
        result = Parse(s.Safe(), provider);
        return !result.IsInvalid();
    }
    public static TagHmac Parse(string s, IFormatProvider? provider) => new(Split(s));
    public bool Equals(TagHmac? other) => other is not null && Algorithm == other.Algorithm && DataEquals(other.Data);
    public static TagHmac HmacSha256Of(byte[] key, byte[] content) {
        using var hash = new HMACSHA256(key);
        return new TagHmac(HashAlgorithm.SHA256, hash.ComputeHash(content));
    }
    [GeneratedRegex(".+")]
    private static partial Regex AnythingRegex();
    private TagHmac(TagHash.Parts parts) : base(ILTagId.Hmac, parts) { }
    internal TagHmac(Stream s) : base(ILTagId.Hmac, s) { }
    private static TagHash.Parts Split(string textualRepresentation) {
        if (string.IsNullOrWhiteSpace(textualRepresentation))
            throw new ArgumentNullException(nameof(textualRepresentation));
        var parts = textualRepresentation.Split(_prefixSeparator, StringSplitOptions.None);
        var algorithm = parts.Length < 2 ? HashAlgorithm.SHA256 : (HashAlgorithm)Enum.Parse(typeof(HashAlgorithm), parts[1], ignoreCase: true);
        return new TagHash.Parts { Algorithm = algorithm, Data = parts[0].FromSafeBase64() };
    }
    private bool DataEquals(byte[]? otherData) => (Data.None() && otherData.None()) || Data.OrEmpty().HasSameBytesAs(otherData.OrEmpty());
    protected override string BuildTextualRepresentation() => $"{Data?.ToSafeBase64() ?? ""}#HMAC-{Algorithm}";
    protected override TagHash.Parts? ValueFromStream(WrappedReadonlyStream s) => new() {
        Algorithm = (HashAlgorithm)s.BigEndianReadUShort(),
        Data = s.ReadAllBytesAsync().WaitResult()
    };
    protected override Stream ValueToStream(Stream s) => s.BigEndianWriteUShort((ushort)Value!.Algorithm).WriteBytes(Data.OrEmpty());
}