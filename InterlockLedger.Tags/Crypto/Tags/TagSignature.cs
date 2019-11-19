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
    public struct TagSignatureParts : IEquatable<TagSignatureParts>
    {
        public Algorithm Algorithm;
        public byte[] Data;

        public static bool operator !=(TagSignatureParts left, TagSignatureParts right) => !(left == right);

        public static bool operator ==(TagSignatureParts left, TagSignatureParts right) => left.Equals(right);

        public override bool Equals(object obj) => obj is TagSignatureParts parts && Equals(parts);

        public bool Equals(TagSignatureParts other) => Algorithm == other.Algorithm && EqualityComparer<byte[]>.Default.Equals(Data, other.Data);

        public override int GetHashCode() {
            var hashCode = 699340383;
            hashCode = hashCode * -1521134295 + Algorithm.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<byte[]>.Default.GetHashCode(Data);
            return hashCode;
        }
    }

    public class TagSignature : ILTagExplicit<TagSignatureParts>
    {
        public TagSignature(Algorithm algorithm, byte[] data) : base(ILTagId.Signature, new TagSignatureParts { Algorithm = algorithm, Data = data }) {
        }

        public Algorithm Algorithm => Value.Algorithm;

        public byte[] Data => Value.Data;

        internal TagSignature(Stream s) : base(ILTagId.Signature, s) {
        }

        protected override TagSignatureParts FromBytes(byte[] bytes) =>
            FromBytesHelper(bytes, s => new TagSignatureParts {
                Algorithm = (Algorithm)s.BigEndianReadUShort(),
                Data = s.ReadBytes(bytes.Length - sizeof(ushort))
            });

        protected override byte[] ToBytes()
            => ToBytesHelper(s => s.BigEndianWriteUShort((ushort)Value.Algorithm).WriteBytes(Value.Data));
    }
}