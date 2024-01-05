// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2024 InterlockLedger Network
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

using System.Security.Cryptography;

namespace InterlockLedger.Tags;
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
                try {
                    return RSAalg.Decrypt(data, true);
                } catch (CryptographicException) {
                    return RSAalg.Decrypt(data, false);
                }
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
                return RSAalg.Encrypt(data, true);
            } catch (CryptographicException e) {
                if (retries-- <= 0)
                    throw new InterlockLedgerCryptographicException($"Failed to encrypt data with current parameters after {_maxRetries} retries", e);
            }
    }

    public static byte[] HashAndSign(byte[] data, RSAParameters parameters) {
        int retries = _maxRetries;
        while (true)
            try {
                using var RSAalg = OpenProvider(parameters);
                return RSAalg.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            } catch (CryptographicException e) {
                if (retries-- <= 0)
                    throw new InterlockLedgerCryptographicException($"Failed to sign data with current parameters after {_maxRetries} retries", e);
            }
    }

    public static byte[] HashAndSignBytes<T>(T dataToSign, RSAParameters parameters) where T : Signable<T>, new() {
        int retries = _maxRetries;
        while (true)
            try {
                using var RSAalg = OpenProvider(parameters);
                using var dataStream = dataToSign.OpenReadingStreamAsync().WaitResult();
                return RSAalg.SignData(dataStream, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            } catch (CryptographicException e) {
                if (retries-- <= 0)
                    throw new InterlockLedgerCryptographicException($"Failed to sign data with current parameters after {_maxRetries} retries", e);
            }
    }

    public static bool Verify<T>(T dataToVerify, TagSignature signature, RSAParameters parameters) where T : Signable<T>, new() {
        using var dataStream = dataToVerify.Required().OpenReadingStreamAsync().WaitResult();
        return VerifyStream(dataStream, signature, parameters);
    }

    public static bool Verify(byte[] dataToVerify, TagSignature signature, RSAParameters parameters) {
        using var dataStream = new MemoryStream(dataToVerify, writable: false);
        return VerifyStream(dataStream, signature, parameters);
    }

    private const int _maxRetries = 3;

    private static RSACryptoServiceProvider OpenProvider(RSAParameters parameters) {
        var RSAalg = new RSACryptoServiceProvider();
        RSAalg.ImportParameters(parameters);
        return RSAalg;
    }

    private static bool VerifyStream(Stream dataStream, TagSignature signature, RSAParameters parameters) {
        try {
            if (signature.Required().Algorithm != Algorithm.RSA)
                throw new InvalidDataException($"Signature uses different algorithm {signature.Algorithm} from this RSA key!");
            if (parameters.Exponent == null || parameters.Modulus == null)
                throw new InvalidDataException($"This RSA key is not properly configured to be able to verify a signature!");
            using var RSAalg = OpenProvider(parameters);
            return RSAalg.VerifyData(dataStream, signature.Data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        } catch (CryptographicException e) {
            throw new InterlockLedgerCryptographicException("Failed to verify data with current parameters and signature", e);
        }
    }
}