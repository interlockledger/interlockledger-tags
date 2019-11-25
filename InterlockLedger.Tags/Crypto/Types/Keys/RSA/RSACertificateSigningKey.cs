/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using InterlockLedger.Tags;

namespace InterlockLedger.Tags
{
    public class RSACertificateSigningKey : InterlockSigningKey
    {
        public RSACertificateSigningKey(InterlockSigningKeyData data, byte[] certificateBytes, string password) : base(data) {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (data.EncryptedContentType != EncryptedContentType.EmbeddedCertificate)
                throw new ArgumentException($"Wrong kind of EncryptedContentType {data.EncryptedContentType}", nameof(data));
            _password = password ?? throw new ArgumentNullException(nameof(password));
            _certificateBytes = certificateBytes ?? throw new ArgumentNullException(nameof(certificateBytes));
        }

        public override byte[] AsSessionState {
            get {
                using var ms = new MemoryStream();
                ms.EncodeTag(_value);
                ms.EncodeString(_password);
                ms.EncodeByteArray(_certificateBytes);
                return ms.ToArray();
            }
        }

        public static new RSACertificateSigningKey FromSessionState(byte[] bytes) {
            using var ms = new MemoryStream(bytes);
            var tag = ms.Decode<InterlockSigningKeyData>();
            var password = ms.DecodeString();
            var certificateBytes = ms.DecodeByteArray();
            return new RSACertificateSigningKey(tag, certificateBytes, password);
        }

        public override byte[] Decrypt(byte[] bytes) {
            using var rsa = _certificateBytes.OpenCertificate(_password).GetRSAPrivateKey();
            return rsa.Decrypt(bytes, RSAEncryptionPadding.Pkcs1);
        }

        public override TagSignature Sign(byte[] data) => new TagSignature(Algorithm.RSA, HashAndSign(data));

        private readonly byte[] _certificateBytes;
        private readonly string _password;

        private byte[] HashAndSign(byte[] dataToSign) {
            using var rsa = _certificateBytes.OpenCertificate(_password).GetRSAPrivateKey();
            return rsa.SignData(dataToSign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }
}