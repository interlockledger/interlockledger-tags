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
using System.Security.Cryptography;

namespace InterlockLedger.Tags
{
    public static class ECDsaHelper
    {
        public static TagECParameters CreateNewECDsaParameters(KeyStrength strength) {
            var keySize = strength.ECDsaKeySize();
            using var provider = ECDsa.Create();
            return new TagECParameters(provider.ExportParameters(true), strength);
        }

        public static byte[] Decrypt(byte[] data, ECParameters Key) {
            int retries = _maxRetries;
            while (true)
                try {
                    using var ECDsaalg = OpenWith(Key);
                    try {
                        return ECDsaalg.Decrypt(data, true);
                    } catch (CryptographicException) {
                        return ECDsaalg.Decrypt(data, false);
                    }
                } catch (CryptographicException e) {
                    if (retries-- <= 0)
                        throw new InterlockLedgerCryptographicException($"Failed to decrypt data with current parameters after {_maxRetries} retries", e);
                }
        }

        public static byte[] Encrypt(byte[] data, ECParameters Key) {
            int retries = _maxRetries;
            while (true)
                try {
                    var ECDsaalg = OpenWith(Key);
                    return ECDsaalg.Encrypt(data, true);
                } catch (CryptographicException e) {
                    if (retries-- <= 0)
                        throw new InterlockLedgerCryptographicException($"Failed to encrypt data with current parameters after {_maxRetries} retries", e);
                }
        }

        public static byte[] HashAndSign(byte[] data, ECParameters parameters) {
            int retries = _maxRetries;
            while (true)
                try {
                    using var ECDsaalg = OpenWith(parameters);
                    return ECDsaalg.SignData(data, HashAlgorithmName.SHA256);
                } catch (CryptographicException e) {
                    if (retries-- <= 0)
                        throw new InterlockLedgerCryptographicException($"Failed to sign data with current parameters after {_maxRetries} retries", e);
                }
        }

        public static byte[] HashAndSignBytes<T>(T dataToSign, ECParameters parameters) where T : Signable<T>, new() {
            int retries = _maxRetries;
            while (true)
                try {
                    using var ECDsaalg = OpenWith(parameters);
                    using var dataStream = dataToSign.OpenReadingStream();
                    return ECDsaalg.SignData(dataStream, HashAlgorithmName.SHA256);
                } catch (CryptographicException e) {
                    if (retries-- <= 0)
                        throw new InterlockLedgerCryptographicException($"Failed to sign data with current parameters after {_maxRetries} retries", e);
                }
        }

        public static bool Verify<T>(T dataToVerify, TagSignature signature, ECParameters parameters) where T : Signable<T>, new() {
            using var dataStream = dataToVerify.Required(nameof(dataToVerify)).OpenReadingStream();
            return VerifyStream(dataStream, signature, parameters);
        }

        public static bool Verify(byte[] dataToVerify, TagSignature signature, ECParameters parameters) {
            using var dataStream = new MemoryStream(dataToVerify, writable: false);
            return VerifyStream(dataStream, signature, parameters);
        }

        private const int _maxRetries = 3;

        private static ECDsa OpenWith(ECParameters Key) {
            var ecdsa = ECDsa.Create();
            ecdsa.ImportParameters(Key);
            return ecdsa;
        }

        private static bool VerifyStream(Stream dataStream, TagSignature signature, ECParameters parameters) {
            try {
                if (signature.Required(nameof(signature)).Algorithm != Algorithm.EcDSA)
                    throw new InvalidDataException($"Signature uses different algorithm {signature.Algorithm} from this ECDsa key!");
                if (parameters.D == null)
                    throw new InvalidDataException($"This ECDsa key is not properly configured to be able to verify a signature!");
                using var ECDsaalg = OpenWith(parameters);
                return ECDsaalg.VerifyData(dataStream, signature.Data, HashAlgorithmName.SHA256);
            } catch (CryptographicException e) {
                throw new InterlockLedgerCryptographicException("Failed to verify data with current parameters and signature", e);
            }
        }
    }
}