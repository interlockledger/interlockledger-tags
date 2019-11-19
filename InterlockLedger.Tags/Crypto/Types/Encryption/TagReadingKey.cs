/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;

namespace InterlockLedger.Tags
{
    public class TagReadingKey : ILTagExplicit<TagReadingKey.Parts>
    {
        public TagReadingKey(string id, TagHash publickKeyHash, byte[] encryptedKey, byte[] encryptedIV)
            : base(ILTagId.ReadingKey, new Parts(id, publickKeyHash, encryptedKey, encryptedIV)) {
        }

        public byte[] EncryptedIV => Value.EncryptedIV;
        public byte[] EncryptedKey => Value.EncryptedKey;
        public TagHash PublickKeyHash => Value.PublickKeyHash;
        public string ReaderId => Value.ReaderId;

        public struct Parts : IEquatable<Parts>
        {
            public readonly byte[] EncryptedIV;
            public readonly byte[] EncryptedKey;

            public readonly TagHash PublickKeyHash;

            public readonly string ReaderId;

            public Parts(string id, TagHash publickKeyHash, byte[] encryptedKey, byte[] encryptedIV) {
                if (string.IsNullOrWhiteSpace(id))
                    throw new ArgumentException("Must provide a non-empty id for this reading key", nameof(id));
                ReaderId = id;
                PublickKeyHash = publickKeyHash;
                EncryptedKey = encryptedKey ?? throw new System.ArgumentNullException(nameof(encryptedKey));
                EncryptedIV = encryptedIV ?? throw new ArgumentNullException(nameof(encryptedIV));
            }

            public static bool operator !=(Parts left, Parts right) => !(left == right);

            public static bool operator ==(Parts left, Parts right) => left.Equals(right);

            public override bool Equals(object obj) => obj is Parts parts && Equals(parts);

            public bool Equals(Parts other) => EqualityComparer<byte[]>.Default.Equals(EncryptedIV, other.EncryptedIV) && EqualityComparer<byte[]>.Default.Equals(EncryptedKey, other.EncryptedKey) && EqualityComparer<TagHash>.Default.Equals(PublickKeyHash, other.PublickKeyHash) && ReaderId == other.ReaderId;

            public override int GetHashCode() {
                var hashCode = 390428901;
                hashCode = hashCode * -1521134295 + EqualityComparer<byte[]>.Default.GetHashCode(EncryptedIV);
                hashCode = hashCode * -1521134295 + EqualityComparer<byte[]>.Default.GetHashCode(EncryptedKey);
                hashCode = hashCode * -1521134295 + EqualityComparer<TagHash>.Default.GetHashCode(PublickKeyHash);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ReaderId);
                return hashCode;
            }

            public override string ToString() => $"Reading key for reader {ReaderId}";
        }

        internal TagReadingKey(Stream s) : base(ILTagId.ReadingKey, s) {
        }

        protected override Parts FromBytes(byte[] bytes) =>
            FromBytesHelper(bytes, s => new Parts(s.DecodeString(), s.Decode<TagHash>(), s.DecodeByteArray(), s.DecodeByteArray()));

        protected override byte[] ToBytes()
            => ToBytesHelper(s => s.EncodeString(ReaderId).EncodeTag(PublickKeyHash).EncodeByteArray(EncryptedKey).EncodeByteArray(EncryptedIV));
    }
}