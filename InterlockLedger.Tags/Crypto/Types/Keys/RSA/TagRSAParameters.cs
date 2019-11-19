/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.IO;
using System.Security.Cryptography;

namespace InterlockLedger.Tags
{
    public class TagRSAParameters : ILTagExplicit<RSAParameters>, IKeyParameters
    {
        public TagRSAParameters(RSAParameters parameters) : base(ILTagId.RSAParameters, parameters) {
        }

        public TagPubKey PublicKey => new TagPubRSAKey(Value);

        public static TagRSAParameters DecodeFromBytes(byte[] encodedBytes) {
            using var s = new MemoryStream(encodedBytes);
            return s.Decode<TagRSAParameters>();
        }

        internal TagRSAParameters(Stream s) : base(ILTagId.RSAParameters, s) {
        }

        protected override RSAParameters FromBytes(byte[] bytes) => FromBytesHelper(bytes, s => new RSAParameters {
            Modulus = s.DecodeByteArray(),
            Exponent = s.DecodeByteArray(),
            P = s.DecodeByteArray(),
            Q = s.DecodeByteArray(),
            DP = s.DecodeByteArray(),
            DQ = s.DecodeByteArray(),
            InverseQ = s.DecodeByteArray(),
            D = s.DecodeByteArray(),
        });

        protected override byte[] ToBytes()
            => ToBytesHelper(s => s
                .EncodeByteArray(Value.Modulus)
                .EncodeByteArray(Value.Exponent)
                .EncodeByteArray(Value.P)
                .EncodeByteArray(Value.Q)
                .EncodeByteArray(Value.DP)
                .EncodeByteArray(Value.DQ)
                .EncodeByteArray(Value.InverseQ)
                .EncodeByteArray(Value.D)
                );
    }
}