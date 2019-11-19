/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using System.IO;
using System.Security.Cryptography;

namespace InterlockLedger.Tags
{
    public sealed class TagHmac : ILTagExplicit<TagHashParts>, IEquatable<TagHmac>
    {
        public TagHmac(HashAlgorithm algorithm, byte[] data) : base(ILTagId.Hmac, new TagHashParts { Algorithm = algorithm, Data = data }) {
        }

        public TagHmac(string textualRepresentation) : base(ILTagId.Hmac, Split(textualRepresentation)) {
        }

        public override string Formatted => ToString();
        public string TextualRepresentation => ToString();
        public HashAlgorithm Algorithm => Value.Algorithm;
        public byte[] Data => Value.Data;

        public static TagHmac HmacSha256Of(byte[] key, byte[] content) {
            using var hash = new HMACSHA256(key);
            return new TagHmac(HashAlgorithm.SHA256, hash.ComputeHash(content));
        }

        public static implicit operator string(TagHmac Value) => Value?.ToString();

        public bool Equals(TagHmac other) => (!(other is null)) && Algorithm == other.Algorithm && DataEquals(other.Data);

        public override bool Equals(object obj) => Equals(obj as TagHmac);

        public override int GetHashCode() => ToString().GetHashCode();

        public override string ToString() => $"{Data?.ToSafeBase64() ?? ""}#HMAC-{Algorithm}";

        internal TagHmac(Stream s) : base(ILTagId.Hmac, s) {
        }

        protected override TagHashParts FromBytes(byte[] bytes) =>
            FromBytesHelper(bytes, s => new TagHashParts {
                Algorithm = (HashAlgorithm)s.BigEndianReadUShort(),
                Data = s.ReadBytes(bytes.Length - sizeof(ushort))
            });

        protected override byte[] ToBytes()
            => ToBytesHelper(s => s.BigEndianWriteUShort((ushort)Value.Algorithm).WriteBytes(Value.Data));

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
}