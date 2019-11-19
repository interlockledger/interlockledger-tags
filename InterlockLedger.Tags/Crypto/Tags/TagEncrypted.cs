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
    public struct TagEncryptedParts : IEquatable<TagEncryptedParts>
    {
        public CipherAlgorithm Algorithm;
        public byte[] CipherData;

        public static bool operator !=(TagEncryptedParts left, TagEncryptedParts right) => !(left == right);

        public static bool operator ==(TagEncryptedParts left, TagEncryptedParts right) => left.Equals(right);

        public override bool Equals(object obj) => obj is TagEncryptedParts parts && Equals(parts);

        public bool Equals(TagEncryptedParts other) => Algorithm == other.Algorithm && EqualityComparer<byte[]>.Default.Equals(CipherData, other.CipherData);

        public override int GetHashCode() {
            var hashCode = 462703764;
            hashCode = hashCode * -1521134295 + Algorithm.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<byte[]>.Default.GetHashCode(CipherData);
            return hashCode;
        }
    }

    public class TagEncrypted : ILTagExplicit<TagEncryptedParts>
    {
        public TagEncrypted(CipherAlgorithm algorithm, byte[] data) :
            base(ILTagId.Encrypted, new TagEncryptedParts { Algorithm = algorithm, CipherData = data }) {
        }

        public CipherAlgorithm Algorithm => Value.Algorithm;

        public byte[] CipherData => Value.CipherData;

        internal TagEncrypted(Stream s) : base(ILTagId.Encrypted, s) {
        }

        protected override TagEncryptedParts FromBytes(byte[] bytes) =>
            FromBytesHelper(bytes, s => new TagEncryptedParts {
                Algorithm = (CipherAlgorithm)s.BigEndianReadUShort(),
                CipherData = s.DecodeByteArray(),
            });

        protected override byte[] ToBytes()
            => ToBytesHelper(s => s.BigEndianWriteUShort((ushort)Algorithm).EncodeByteArray(CipherData));
    }
}