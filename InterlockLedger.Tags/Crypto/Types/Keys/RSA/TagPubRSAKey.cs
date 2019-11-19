/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.IO;
using System.Security.Cryptography;
using InterlockLedger.Tags;

namespace InterlockLedger.Tags
{
    public class TagPubRSAKey : TagPubKey
    {
        public readonly RSAParameters Parameters;

        public TagPubRSAKey(RSAParameters parameters) : this(EncodeParameters(parameters)) {
        }

        public override KeyStrength Strength {
            get {
                using var RSAalg = new RSACryptoServiceProvider();
                RSAalg.ImportParameters(Parameters);
                return RSAalg.KeyStrengthGuess();
            }
        }

        public override byte[] Encrypt(byte[] bytes) => RSAHelper.Encrypt(bytes, Parameters);

        public override bool Verify(byte[] dataToVerify, TagSignature signature) => RSAHelper.Verify(dataToVerify, signature, Parameters);

        internal TagPubRSAKey(byte[] data) : base(Algorithm.RSA, data) => Parameters = DecodeParameters(Data);

        private static RSAParameters DecodeParameters(byte[] bytes) {
            if (bytes == null || bytes.Length == 0)
                return default;
            using var s = new MemoryStream(bytes);
            return s.Decode<TagPublicRSAParameters>().Value;
        }

        private static byte[] EncodeParameters(RSAParameters parameters) => new TagPublicRSAParameters(parameters).EncodedBytes;
    }
}