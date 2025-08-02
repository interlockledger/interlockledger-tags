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

using System.Security.Cryptography.X509Certificates;

namespace InterlockLedger.Tags;
public record TagKeyParts(Algorithm Algorithm, byte[] Data) { }

[TypeConverter(typeof(TypeCustomConverter<TagPubKey>))]
[JsonConverter(typeof(JsonCustomConverter<TagPubKey>))]
public partial class TagPubKey : ILTagOfExplicitTextual<TagKeyParts>, ITextual<TagPubKey>
{
    public Algorithm Algorithm => Value!.Algorithm;
    public byte[] Data => Value!.Data;
    public TagHash Hash => TagHash.HashSha256Of(Data);
    public virtual KeyStrength Strength => KeyStrength.Normal;
    public bool IsEmpty { get; private init; }
    public static TagPubKey Resolve(X509Certificate2 certificate) {
        var RSA = certificate.GetRSAPublicKey();
        var EcDSA = certificate.GetECDsaPublicKey();
        return RSA is not null
            ? new TagPubRSAKey(RSA.ExportParameters(false))
            : EcDSA is not null
            ? new TagPubEcDSAKey(EcDSA.ExportParameters(false))
            : throw new NotSupportedException("Not yet supporting other kinds of certificates, than RSA and EcDSA!");
    }
    private static TagPubKey ResolveAs(Algorithm algorithm, byte[] data)
        => algorithm switch {
            Algorithm.RSA => new TagPubRSAKey(data),
            Algorithm.EcDSA => new TagPubEcDSAKey(data),
            Algorithm.EdDSA => new TagPublicEdDSAKey(data),
            _ => throw new NotSupportedException("Only support RSA/EcDSA/EdDSA for now!!!")
        };
    public static TagPubKey InvalidBy(string cause) => throw new NotSupportedException(cause);
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out TagPubKey result) {
        result = Parse(s.Safe(), provider);
        return !result.IsInvalid();
    }
    public static TagPubKey Parse(string s, IFormatProvider? provider) {
        if (string.IsNullOrWhiteSpace(s))
            throw new ArgumentException("Can't have empty pubkey textual representation!!!", nameof(s));
        var parts = s.Split('!', '#');
        return parts.Length != 3
               || (!parts[0].Equals("PubKey", StringComparison.OrdinalIgnoreCase))
               || !Enum.TryParse(parts[2], ignoreCase: true, out Algorithm algorithm)
            ? throw new ArgumentException($"Bad format of pubkey textual representation: '{s}'!!!", nameof(s))
            : ResolveAs(algorithm, parts[1].FromSafeBase64());
    }
    public static TagPubKey Empty { get; } = new TagPubKey() { IsEmpty = true };
    public static Regex Mask { get; } = PubKeyRegex();
    public ITextual<TagPubKey> Textual => this;
    [GeneratedRegex(@"PubKey!.+#\w+")]
    private static partial Regex PubKeyRegex();
    public virtual byte[] Encrypt(byte[] bytes) => throw new NotImplementedException();
    protected override bool AreEquivalent(ILTagOf<TagKeyParts?> other) => other.Value is not null && other.Value.Algorithm == Algorithm && other.Value.Data.HasSameBytesAs(Data);
    public override string ToString() => Textual.FullRepresentation();
    public bool Equals(TagPubKey? other) => other is not null && (Algorithm == other.Algorithm) && Data.HasSameBytesAs(other.Data);
    public virtual bool Verify(Stream dataStream, TagSignature signature) => false;
    internal static TagPubKey Resolve(Stream s) {
        var pubKey = new TagPubKey(s);
        return ResolveAs(pubKey.Algorithm, pubKey.Data);
    }
    protected TagPubKey(Algorithm algorithm, byte[] data) : base(ILTagId.PubKey, new TagKeyParts(algorithm, data)) { }
    private TagPubKey(Stream s) : base(ILTagId.PubKey, s) { }
    protected override string BuildTextualRepresentation() => $"PubKey!{Data.ToSafeBase64()}#{Algorithm}";
    protected override async Task<TagKeyParts?> ValueFromStreamAsync(WrappedReadonlyStream s) =>
        new((Algorithm)s.BigEndianReadUShort(), await s.ReadAllBytesAsync().ConfigureAwait(false));
    protected override Task<Stream> ValueToStreamAsync(Stream s) =>
        Task.FromResult(s.BigEndianWriteUShort((ushort)Value!.Algorithm).WriteBytes(Value.Data));
    private TagPubKey() : this(Algorithm.Invalid, []) { }
}