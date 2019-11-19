/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using System.IO;
using InterlockLedger.Tags;

namespace InterlockLedger.Tags
{
    public class RSAInterlockSigningKey : InterlockSigningKey
    {
        public RSAInterlockSigningKey(InterlockSigningKeyData data, byte[] decrypted) : base(data) {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (data.EncryptedContentType != EncryptedContentType.EncryptedKey)
                throw new ArgumentException($"Wrong kind of EncryptedContentType {data.EncryptedContentType}", nameof(data));
            using var ms = new MemoryStream(decrypted);
            _keyParameters = ms.Decode<TagRSAParameters>();
        }

        public override byte[] AsSessionState {
            get {
                using var ms = new MemoryStream();
                ms.EncodeTag(_value);
                ms.EncodeTag(_keyParameters);
                ms.Flush();
                return ms.GetBuffer();
            }
        }

        public static new RSAInterlockSigningKey FromSessionState(byte[] bytes) {
            using var ms = new MemoryStream(bytes);
            var tag = ms.Decode<InterlockSigningKeyData>();
            var parameters = ms.Decode<TagRSAParameters>();
            return new RSAInterlockSigningKey(tag, parameters);
        }

        public override byte[] Decrypt(byte[] bytes) => RSAHelper.Decrypt(bytes, _keyParameters.Value);

        public override TagSignature Sign(byte[] data) => new TagSignature(Algorithm.RSA, RSAHelper.HashAndSignBytes(data, _keyParameters.Value));

        private readonly TagRSAParameters _keyParameters;

        private RSAInterlockSigningKey(InterlockSigningKeyData tag, TagRSAParameters parameters) : base(tag) => _keyParameters = parameters;
    }
}