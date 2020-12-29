/******************************************************************************************************************************
 
Copyright (c) 2018-2020 InterlockLedger Network
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
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using InterlockLedger.Tags;

namespace InterlockLedger.Tags
{
    [TypeConverter(typeof(TagHashConverter))]
    [JsonConverter(typeof(JsonCustomConverter<TagHash>))]
    public sealed class TagHash : ILTagExplicit<TagHashParts>, IEquatable<TagHash>, IJsonCustom<TagHash>
    {
        public static readonly TagHash Empty = new TagHash(HashAlgorithm.SHA256, HashSha256(Array.Empty<byte>()));

        public TagHash() : this(HashAlgorithm.Copy, Array.Empty<byte>()) {
        }

        public TagHash(HashAlgorithm algorithm, byte[] data) : base(ILTagId.Hash, new TagHashParts { Algorithm = algorithm, Data = data }) {
        }

        public TagHash(string textualRepresentation) : base(ILTagId.Hash, Split(textualRepresentation)) {
        }

        public HashAlgorithm Algorithm => Value.Algorithm;
        public override object AsJson => TextualRepresentation;
        public byte[] Data => Value.Data;
        public override string Formatted => TextualRepresentation;
        public string TextualRepresentation => ToString();

        public static TagHash From(string textualRepresentation) => textualRepresentation.SafeAny() ? new TagHash(textualRepresentation.Trim()) : null;

        public static TagHash HashSha256Of(byte[] data) => new TagHash(HashAlgorithm.SHA256, HashSha256(data));

        public static TagHash HashSha256Of(IEnumerable<byte> data) => HashSha256Of(data.ToArray());

        public static implicit operator string(TagHash Value) => Value?.ToString();

        public static bool operator !=(TagHash a, TagHash b) => !(a == b);

        public static bool operator ==(TagHash a, TagHash b) => a?.Equals(b) ?? b is null;

        public override bool Equals(object obj) => Equals(obj as TagHash);

        public bool Equals(TagHash other) => !(other is null) && Algorithm == other.Algorithm && DataEquals(other.Data);

        public override int GetHashCode() => -1_574_110_226 + _dataHashCode + Algorithm.GetHashCode();

        public TagHash ResolveFrom(string textualRepresentation) => new TagHash(textualRepresentation);

        public override string ToString() => $"{Data?.ToSafeBase64() ?? ""}#{Algorithm}";

        internal TagHash(Stream s) : base(ILTagId.Hash, s) {
        }

        internal static TagHash HashFrom(X509Certificate2 certificate)
            => new TagHash(HashAlgorithm.SHA1, certificate.Required(nameof(certificate)).GetCertHash());

        protected override TagHashParts FromBytes(byte[] bytes) =>
            FromBytesHelper(bytes, s => new TagHashParts {
                Algorithm = (HashAlgorithm)s.BigEndianReadUShort(),
                Data = s.ReadBytes(bytes.Length - sizeof(ushort))
            });

        protected override byte[] ToBytes()
            => ToBytesHelper(s => s.BigEndianWriteUShort((ushort)Value.Algorithm).WriteBytes(Value.Data));

        private int _dataHashCode => Data?.Aggregate(19, (sum, b) => sum + b) ?? 19;

        private static byte[] HashSha256(byte[] data) {
            using var hasher = SHA256.Create();
            hasher.Initialize();
            return hasher.ComputeHash(data);
        }

        private static bool IsNullOrEmpty(byte[] data) => data is null || data.Length == 0;

        private static TagHashParts Split(string textualRepresentation) {
            if (string.IsNullOrWhiteSpace(textualRepresentation))
                throw new ArgumentNullException(nameof(textualRepresentation));
            var parts = textualRepresentation.Split('#');
            var algorithm = parts.Length < 2 ? HashAlgorithm.SHA256 : (HashAlgorithm)Enum.Parse(typeof(HashAlgorithm), parts[1], ignoreCase: true);
            return new TagHashParts { Algorithm = algorithm, Data = parts[0].FromSafeBase64() };
        }

        private bool DataEquals(byte[] otherData) => (IsNullOrEmpty(Data) && IsNullOrEmpty(otherData)) || Data.HasSameBytesAs(otherData);
    }

    public class TagHashConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            => destinationType == typeof(InstanceDescriptor) || destinationType == typeof(string);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object Value)
            => Value is string text
                ? TagHash.From(text.Trim())
                : base.ConvertFrom(context, culture, Value);

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object Value, Type destinationType)
            => destinationType == typeof(string) && Value is TagHash
                ? Value.ToString()
                : throw new InvalidOperationException("Can only convert TagHash to string!!!");
    }

    public class TagHashParts
    {
        public HashAlgorithm Algorithm;
        public byte[] Data;
    }
}
