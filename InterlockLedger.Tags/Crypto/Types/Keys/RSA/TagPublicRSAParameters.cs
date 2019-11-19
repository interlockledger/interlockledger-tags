/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.IO;
using System.Security.Cryptography;

namespace InterlockLedger.Tags
{
    public class TagPublicRSAParameters : ILTagExplicit<RSAParameters>
    {
        public TagPublicRSAParameters(RSAParameters parameters) : base(ILTagId.PublicRSAParameters, parameters) {
        }

        internal TagPublicRSAParameters(Stream s) : base(ILTagId.PublicRSAParameters, s) {
        }

        protected override RSAParameters FromBytes(byte[] bytes) => FromBytesHelper(bytes, s => new RSAParameters {
            Modulus = s.DecodeByteArray(),
            Exponent = s.DecodeByteArray()
        });

        protected override byte[] ToBytes() => ToBytesHelper(s => s.EncodeByteArray(Value.Modulus).EncodeByteArray(Value.Exponent));
    }
}