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
    public class TagIdentifiedSignature : ILTagExplicit<TagIdentifiedSignature.Parts>
    {
        public TagIdentifiedSignature(TagSignature signature, BaseKeyId id, TagPubKey publicKey) : base(ILTagId.IdentifiedSignature, new Parts(signature, id, publicKey)) {
        }

        public TagPubKey PublicKey => Value.PublicKey;
        public TagSignature Signature => Value.Signature;
        public BaseKeyId SignerId => Value.SignerId;

        public bool Verify(byte[] data) => PublicKey.Verify(data, Signature);

        public struct Parts : IEquatable<Parts>
        {
            public readonly TagPubKey PublicKey;
            public readonly TagSignature Signature;
            public readonly BaseKeyId SignerId;

            public Parts(TagSignature signature, BaseKeyId id, TagPubKey publicKey) {
                Signature = signature ?? throw new ArgumentNullException(nameof(signature));
                SignerId = id ?? throw new System.ArgumentNullException(nameof(id));
                PublicKey = publicKey ?? throw new System.ArgumentNullException(nameof(publicKey));
            }

            public static bool operator !=(Parts left, Parts right) => !(left == right);

            public static bool operator ==(Parts left, Parts right) => left.Equals(right);

            public override bool Equals(object obj) => obj is Parts parts && Equals(parts);

            public bool Equals(Parts other) => EqualityComparer<TagPubKey>.Default.Equals(PublicKey, other.PublicKey) && EqualityComparer<TagSignature>.Default.Equals(Signature, other.Signature) && EqualityComparer<BaseKeyId>.Default.Equals(SignerId, other.SignerId);

            public override int GetHashCode() {
                var hashCode = -1433170334;
                hashCode = hashCode * -1521134295 + EqualityComparer<TagPubKey>.Default.GetHashCode(PublicKey);
                hashCode = hashCode * -1521134295 + EqualityComparer<TagSignature>.Default.GetHashCode(Signature);
                hashCode = hashCode * -1521134295 + EqualityComparer<BaseKeyId>.Default.GetHashCode(SignerId);
                return hashCode;
            }

            public override string ToString() => $"Signature by signer {SignerId} with public key {PublicKey}";
        }

        internal TagIdentifiedSignature(Stream s) : base(ILTagId.IdentifiedSignature, s) {
        }

        protected override Parts FromBytes(byte[] bytes) =>
            FromBytesHelper(bytes, s => new Parts(s.Decode<TagSignature>(), s.Decode<BaseKeyId>(), s.Decode<TagPubKey>()));

        protected override byte[] ToBytes()
            => ToBytesHelper(s => s.EncodeTag(Value.Signature).EncodeTag(Value.SignerId).EncodeTag(Value.PublicKey));
    }
}