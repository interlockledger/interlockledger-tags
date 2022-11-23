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

using System.Security.Cryptography.X509Certificates;

namespace InterlockLedger.Tags;
public record TagKeyParts(Algorithm Algorithm, byte[] Data) { }

[TypeConverter(typeof(TypeCustomConverter<TagPubKey>))]
[JsonConverter(typeof(JsonCustomConverter<TagPubKey>))]
public partial class TagPubKey : ILTagExplicit<TagKeyParts>, ITextual<TagPubKey>
{

    public TagPubKey() : this(Algorithm.Invalid, Array.Empty<byte>()) { }
    public Algorithm Algorithm => Value.Algorithm;
    public byte[] Data => Value.Data;
    public TagHash Hash => TagHash.HashSha256Of(Data);
    public virtual KeyStrength Strength => KeyStrength.Normal;
    public bool IsEmpty { get; private init; }

    public static TagPubKey Resolve(X509Certificate2 certificate) {
        var RSA = certificate.GetRSAPublicKey();
        var ECDsa = certificate.GetECDsaPublicKey();
        return RSA != null
            ? new TagPubRSAKey(RSA.ExportParameters(false))
            : ECDsa != null
                ? new TagPubECKey(ECDsa.ExportParameters(false))
                : throw new NotSupportedException("Not yet supporting other kinds of certificates!");
    }


    public static TagPubKey Empty { get; } = new TagPubKey() { IsEmpty = true };
    public static Regex Mask { get; } = AnythingRegex();
    public static string InvalidTextualRepresentation { get; } = "?";
    public string? InvalidityCause { get; init; }
    public ITextual<TagPubKey> Textual => this;

    [GeneratedRegex(".+")]
    private static partial Regex AnythingRegex();

    public virtual byte[] Encrypt(byte[] bytes) => throw new NotImplementedException();

    public override bool Equals(object? obj) => Equals(obj as TagPubKey);
    public bool Equals(TagPubKey? other) => Textual.EqualsForAnyInstances(other);
    public override int GetHashCode() => HashCode.Combine(Algorithm, Data);
    public override string ToString() => Textual.FullRepresentation;
    public static TagPubKey FromString(string textualRepresentation) {
        if (string.IsNullOrWhiteSpace(textualRepresentation))
            throw new ArgumentException("Can't have empty pubkey textual representation!!!", nameof(textualRepresentation));
        var parts = textualRepresentation.Split('!', '#');
        return parts.Length != 3
               || (!parts[0].Equals("PubKey", StringComparison.OrdinalIgnoreCase))
               || !Enum.TryParse(parts[2], ignoreCase: true, out Algorithm algorithm)
            ? throw new ArgumentException($"Bad format of pubkey textual representation: '{textualRepresentation}'!!!", nameof(textualRepresentation))
            : ResolveAs(algorithm, parts[1].FromSafeBase64());
    }

    bool ITextual<TagPubKey>.EqualsForValidInstances(TagPubKey other) => (Algorithm == other.Algorithm) && Data.HasSameBytesAs(other.Data);

    public virtual bool Verify<T>(T data, TagSignature signature) where T : Signable<T>, new() => false;

    public virtual bool Verify(byte[] data, TagSignature signature) => false;

    internal static TagPubKey Resolve(Stream s) {
        var pubKey = new TagPubKey(s);
        return ResolveAs(pubKey.Algorithm, pubKey.Data);
    }

    protected TagPubKey(Algorithm algorithm, byte[] data) : base(ILTagId.PubKey, new TagKeyParts(algorithm, data)) =>
        TextualRepresentation = BuildTextualRepresentation();

    protected override TagKeyParts FromBytes(byte[] bytes)
        => FromBytesHelper(bytes,
            s => new TagKeyParts((Algorithm)s.BigEndianReadUShort(), s.ReadBytes(bytes.Length - sizeof(ushort))));
    protected override byte[] ToBytes(TagKeyParts value)
        => TagHelpers.ToBytesHelper(s => s.BigEndianWriteUShort((ushort)value.Algorithm).WriteBytes(Value.Data));
    private TagPubKey(Stream s) : base(ILTagId.PubKey, s) { }
    private string BuildTextualRepresentation() => $"PubKey!{Data.ToSafeBase64()}#{Algorithm}";
    private static TagPubKey ResolveAs(Algorithm algorithm, byte[] data)
        => algorithm switch {
            Algorithm.RSA => new TagPubRSAKey(data),
            Algorithm.EcDSA => TagPubECKey.From(data),
            _ => throw new NotSupportedException("Only support RSA/EcDSA certificates for now!!!")
        };
}
