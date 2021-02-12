// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2021 InterlockLedger Network
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met
//
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
//
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// * Neither the name of the copyright holder nor the names of its
//   contributors may be used to endorse or promote products derived from
//   this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES, LOSS OF USE, DATA, OR PROFITS, OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// ******************************************************************************************************************************

using System;
using System.IO;

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
            var signatureData = RSAHelper.HashAndSignBytes(data, _keyParameters.Value.Parameters);
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