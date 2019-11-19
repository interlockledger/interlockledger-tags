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
    public class RSAInterlockUpdatableSigningKey : InterlockUpdatableSigningKey
    {
        public RSAInterlockUpdatableSigningKey(InterlockUpdatableSigningKeyData tag, byte[] decrypted, ITimeStamper timeStamper) : base(tag, timeStamper) {
            using var ms = new MemoryStream(decrypted);
            _keyParameters = ms.Decode<TagRSAParameters>();
        }

        public override TagPubKey NextPublicKey => (_nextKeyParameters ?? _keyParameters)?.PublicKey;

        public override void DestroyKeys() => _destroyKeysAfterSigning = true;

        public override void GenerateNextKeys() => _nextKeyParameters = RSAHelper.CreateNewRSAParameters(_value.Strength);

        public override TagSignature SignAndUpdate(byte[] data, Func<byte[], byte[]> encrypt = null) {
            var signatureData = RSAHelper.HashAndSignBytes(data, _keyParameters.Value);
            if (_destroyKeysAfterSigning) {
                _keyParameters = null;
                _nextKeyParameters = null;
                _value.Value.Encrypted = null;
                _value.Value.PublicKey = null;
            } else {
                var encryptionHandler = encrypt;
                if (encryptionHandler == null)
                    throw new ArgumentNullException(nameof(encrypt));
                if (_nextKeyParameters != null) {
                    _keyParameters = _nextKeyParameters;
                    _value.Value.Encrypted = encryptionHandler(_keyParameters.EncodedBytes); _value.Value.PublicKey = NextPublicKey;
                    _nextKeyParameters = null;
                    _value.SignaturesWithCurrentKey = 0;
                } else {
                    _value.SignaturesWithCurrentKey++;
                }
                _value.LastSignatureTimeStamp = _timeStamper.Now;
            }
            return new TagSignature(Algorithm.RSA, signatureData);
        }

        private bool _destroyKeysAfterSigning;
        private TagRSAParameters _keyParameters;
        private TagRSAParameters _nextKeyParameters;
    }
}