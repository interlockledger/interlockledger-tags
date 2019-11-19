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
    public class TagReader : ILTagExplicit<TagReader.Parts>
    {
        public TagReader(string id, TagPubKey publicKey) : base(ILTagId.Reader, new Parts(id, publicKey)) {
        }

        public string Id => Value.ReaderId;
        public TagPubKey PublicKey => Value.PublicKey;

        public struct Parts : IEquatable<Parts>
        {
            public readonly TagPubKey PublicKey;
            public readonly string ReaderId;

            public Parts(string id, TagPubKey publicKey) {
                if (string.IsNullOrWhiteSpace(id))
                    throw new ArgumentException("Must provide a non-empty id for this reader", nameof(id));
                ReaderId = id;
                PublicKey = publicKey ?? throw new ArgumentNullException(nameof(publicKey));
            }

            public static bool operator !=(Parts left, Parts right) => !(left == right);

            public static bool operator ==(Parts left, Parts right) => left.Equals(right);

            public override bool Equals(object obj) => obj is Parts parts && Equals(parts);

            public bool Equals(Parts other) => EqualityComparer<TagPubKey>.Default.Equals(PublicKey, other.PublicKey) && ReaderId == other.ReaderId;

            public override int GetHashCode() {
                var hashCode = 1852549026;
                hashCode = hashCode * -1521134295 + EqualityComparer<TagPubKey>.Default.GetHashCode(PublicKey);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ReaderId);
                return hashCode;
            }

            public override string ToString() => $"Reader {ReaderId} with public key {PublicKey}";
        }

        internal TagReader(Stream s) : base(ILTagId.Reader, s) {
        }

        protected override Parts FromBytes(byte[] bytes) =>
            FromBytesHelper(bytes, s => new Parts(s.DecodeString(), s.Decode<TagPubKey>()));

        protected override byte[] ToBytes()
            => ToBytesHelper(s => s.EncodeString(Value.ReaderId).EncodeTag(Value.PublicKey));
    }
}