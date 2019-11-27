/******************************************************************************************************************************

Copyright (c) 2018-2019 InterlockLedger Network
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this
  list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice,
  this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution.

* Neither the name of the copyright holder nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

******************************************************************************************************************************/

using System;
using System.IO;

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
                return ms.ToArray();
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