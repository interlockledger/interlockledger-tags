/******************************************************************************************************************************
 
Copyright (c) 2018-2020 InterlockLedger Network
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
using System.Security.Cryptography;

namespace InterlockLedger.Tags
{
    public static class RSAHelper
    {
        public static TagRSAParameters CreateNewRSAParameters(KeyStrength strength) {
            var keySize = strength.RSAKeySize();
            using var provider = new RSACryptoServiceProvider(keySize);
            return new TagRSAParameters(provider.ExportParameters(true), strength);
        }

        public static byte[] Decrypt(byte[] data, RSAParameters Key) {
            int retries = _maxRetries;
            while (true)
                try {
                    using var RSAalg = new RSACryptoServiceProvider();
                    RSAalg.ImportParameters(Key);
                    return RSAalg.Decrypt(data, false);
                } catch (CryptographicException e) {
                    if (retries-- <= 0)
                        throw new InterlockLedgerCryptographicException($"Failed to decrypt data with current parameters after {_maxRetries} retries", e);
                }
        }

        public static byte[] Encrypt(byte[] data, RSAParameters Key) {
            int retries = _maxRetries;
            while (true)
                try {
                    using var RSAalg = new RSACryptoServiceProvider();
                    RSAalg.ImportParameters(Key);
                    return RSAalg.Encrypt(data, false);
                } catch (CryptographicException e) {
                    if (retries-- <= 0)
                        throw new InterlockLedgerCryptographicException($"Failed to encrypt data with current parameters after {_maxRetries} retries", e);
                }
        }

        public static byte[] HashAndSignBytes(byte[] dataToSign, RSAParameters key) {
            int retries = _maxRetries;
            while (true)
                try {
                    using var RSAalg = new RSACryptoServiceProvider();
                    using var hasher = new SHA256CryptoServiceProvider();
                    RSAalg.ImportParameters(key);
                    return RSAalg.SignData(dataToSign, hasher);
                } catch (CryptographicException e) {
                    if (retries-- <= 0)
                        throw new InterlockLedgerCryptographicException($"Failed to sign data with current parameters after {_maxRetries} retries", e);
                }
        }

        public static bool Verify(byte[] dataToVerify, TagSignature signature, RSAParameters parameters) {
            if (signature is null)
                throw new ArgumentNullException(nameof(signature));
            try {
                if (signature.Algorithm != Algorithm.RSA)
                    throw new InvalidDataException($"Signature uses different algorithm {signature.Algorithm} from this RSA key!");
                if (parameters.Exponent == null || parameters.Modulus == null)
                    throw new InvalidDataException($"This RSA key is not properly configured to be able to verify a signature!");
                using var RSAalg = new RSACryptoServiceProvider();
                RSAalg.ImportParameters(parameters);
                using var hasher = new SHA256CryptoServiceProvider();
                return RSAalg.VerifyData(dataToVerify, hasher, signature.Data);
            } catch (CryptographicException e) {
                throw new InterlockLedgerCryptographicException("Failed to verify data with current parameters and signature", e);
            }
        }

        private const int _maxRetries = 3;
    }
}