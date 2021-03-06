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

using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

namespace InterlockLedger.Tags
{
    public record TagKeyParts(Algorithm Algorithm, byte[] Data) { }

    [TypeConverter(typeof(TagPubKeyConverter))]
    [JsonConverter(typeof(JsonCustomConverter<TagPubKey>))]
    public class TagPubKey : ILTagExplicit<TagKeyParts>, IEquatable<TagPubKey>, ITextual<TagPubKey>
    {
        public TagPubKey() : this(Algorithm.Invalid, Array.Empty<byte>()) {
        }

        public Algorithm Algorithm => Value.Algorithm;
        public byte[] Data => Value.Data;
        public TagHash Hash => TagHash.HashSha256Of(Data);
        public bool IsEmpty { get; }
        public bool IsInvalid { get; }
        public virtual KeyStrength Strength => KeyStrength.Normal;
        public string TextualRepresentation => $"PubKey!{Data.ToSafeBase64()}#{Algorithm}";

        public static bool operator !=(TagPubKey left, TagPubKey right) => !(left == right);

        public static bool operator ==(TagPubKey left, TagPubKey right) => left?.Equals(right) ?? right is null;

        public static TagPubKey Resolve(X509Certificate2 certificate) {
            var RSA = certificate.GetRSAPublicKey();
            if (RSA != null)
                return new TagPubRSAKey(RSA.ExportParameters(false));
            var ECDsa = certificate.GetECDsaPublicKey();
            if (ECDsa == null)
                throw new NotSupportedException("Only support RSA certificates for now!!!");
            throw new NotSupportedException("Not yet supporting ECDsa certificates!");
        }

        public static TagPubKey Resolve(string textualRepresentation) {
            if (string.IsNullOrWhiteSpace(textualRepresentation))
                throw new ArgumentException("Can't have empty pubkey textual representation!!!", nameof(textualRepresentation));
            var parts = textualRepresentation.Split('!', '#');
            if (parts.Length != 3 || (!parts[0].Equals("PubKey", StringComparison.OrdinalIgnoreCase)) || !Enum.TryParse(parts[2], out Algorithm algorithm))
                throw new ArgumentException($"Bad format of pubkey textual representation: '{textualRepresentation}'!!!", nameof(textualRepresentation));
            if (algorithm == Algorithm.RSA)
                return new TagPubRSAKey(parts[1].FromSafeBase64());
            if (algorithm == Algorithm.EcDSA)
                throw new NotSupportedException("Not yet supporting EcDSA certificates!");
            throw new NotSupportedException("Only support RSA certificates for now!!!");
        }

        public virtual byte[] Encrypt(byte[] bytes) => throw new NotImplementedException();

        public override bool Equals(object obj) => Equals(obj as TagPubKey);

        public bool Equals(TagPubKey other) => (other != null) && (Algorithm == other.Algorithm) && Data.HasSameBytesAs(other.Data);

        public override int GetHashCode() => HashCode.Combine(Algorithm, Data);

        public override string ToString() => TextualRepresentation;

        public virtual bool Verify<T>(T data, TagSignature signature) where T : Signable<T>, new() => false;
        public virtual bool Verify(byte[] data, TagSignature signature) => false;

        internal static TagPubKey Resolve(Stream s) {
            var pubKey = new TagPubKey(s);
            return pubKey.Algorithm == Algorithm.RSA ? new TagPubRSAKey(pubKey.Data) : pubKey;
        }

        protected TagPubKey(Algorithm algorithm, byte[] data) : base(ILTagId.PubKey, new TagKeyParts(algorithm, data)) {
        }

        protected override TagKeyParts FromBytes(byte[] bytes)
            => FromBytesHelper(bytes,
                s => new TagKeyParts((Algorithm)s.BigEndianReadUShort(), s.ReadBytes(bytes.Length - sizeof(ushort))));

        protected override byte[] ToBytes(TagKeyParts value)
            => TagHelpers.ToBytesHelper(s => s.BigEndianWriteUShort((ushort)value.Algorithm).WriteBytes(Value.Data));

        private TagPubKey(Stream s) : base(ILTagId.PubKey, s) {
        }
    }

    public class TagPubKeyConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            => destinationType == typeof(InstanceDescriptor) || destinationType == typeof(string);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object Value) {
            if (Value is string text) {
                text = text.Trim();
                return TagPubKey.Resolve(text);
            }
            return base.ConvertFrom(context, culture, Value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object Value, Type destinationType)
            => destinationType == null
                ? throw new ArgumentNullException(nameof(destinationType))
                : Value == null
                    ? throw new ArgumentNullException(nameof(Value))
                    : destinationType != typeof(string) || !(Value is TagPubKey)
                        ? throw new InvalidOperationException("Can only convert TagPubKey to string!!!")
                        : Value.ToString();
    }
}