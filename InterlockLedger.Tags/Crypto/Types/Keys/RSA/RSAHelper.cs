/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
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
            return new TagRSAParameters(provider.ExportParameters(true));
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
