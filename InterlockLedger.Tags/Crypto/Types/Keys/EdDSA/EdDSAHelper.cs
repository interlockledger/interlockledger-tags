// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2023 InterlockLedger Network
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

using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace InterlockLedger.Tags;

public static class EdDSAHelper
{
    private const int _maxRetries = 3;

    public static TagEdDSAParameters CreateNewEdDSAParameters() => new(new EdDSAParameters(Array.Empty<byte>()));

    public static byte[] Decrypt(byte[] data, EdDSAParameters Key) {
        int num = _maxRetries;
        while (true) {
            try {
                using var rSACryptoServiceProvider = new EdDSACryptoServiceProvider();
                rSACryptoServiceProvider.ImportParameters(Key);
                return rSACryptoServiceProvider.Decrypt(data);
            } catch (CryptographicException innerException) {
                if (num-- <= 0) {
                    var defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(61, 1);
                    defaultInterpolatedStringHandler.AppendLiteral("Failed to decrypt data with current parameters after ");
                    defaultInterpolatedStringHandler.AppendFormatted(3);
                    defaultInterpolatedStringHandler.AppendLiteral(" retries");
                    throw new InterlockLedgerCryptographicException(defaultInterpolatedStringHandler.ToStringAndClear(), innerException);
                }
            }
        }
    }

    public static byte[] Encrypt(byte[] data, EdDSAParameters Key) {
        int num = _maxRetries;
        while (true) {
            try {
                using var rSACryptoServiceProvider = new EdDSACryptoServiceProvider();
                rSACryptoServiceProvider.ImportParameters(Key);
                return rSACryptoServiceProvider.Encrypt(data);
            } catch (CryptographicException innerException) {
                if (num-- <= 0) {
                    var defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(61, 1);
                    defaultInterpolatedStringHandler.AppendLiteral("Failed to encrypt data with current parameters after ");
                    defaultInterpolatedStringHandler.AppendFormatted(3);
                    defaultInterpolatedStringHandler.AppendLiteral(" retries");
                    throw new InterlockLedgerCryptographicException(defaultInterpolatedStringHandler.ToStringAndClear(), innerException);
                }
            }
        }
    }

    public static byte[] HashAndSign(byte[] data, EdDSAParameters parameters) {
        int num = _maxRetries;
        while (true) {
            try {
                using var rSACryptoServiceProvider = OpenProvider(parameters);
                return rSACryptoServiceProvider.SignData(data, HashAlgorithmName.SHA256);
            } catch (CryptographicException innerException) {
                if (num-- <= 0) {
                    var defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(58, 1);
                    defaultInterpolatedStringHandler.AppendLiteral("Failed to sign data with current parameters after ");
                    defaultInterpolatedStringHandler.AppendFormatted(3);
                    defaultInterpolatedStringHandler.AppendLiteral(" retries");
                    throw new InterlockLedgerCryptographicException(defaultInterpolatedStringHandler.ToStringAndClear(), innerException);
                }
            }
        }
    }

    public static byte[] HashAndSignBytes<T>(T dataToSign, EdDSAParameters parameters) where T : Signable<T>, new() {
        int num = _maxRetries;
        while (true) {
            try {
                using var rSACryptoServiceProvider = OpenProvider(parameters);
                using var data = dataToSign.OpenReadingStreamAsync().Result;
                return rSACryptoServiceProvider.SignData(data, HashAlgorithmName.SHA256);
            } catch (CryptographicException innerException) {
                if (num-- <= 0) {
                    var defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(58, 1);
                    defaultInterpolatedStringHandler.AppendLiteral("Failed to sign data with current parameters after ");
                    defaultInterpolatedStringHandler.AppendFormatted(3);
                    defaultInterpolatedStringHandler.AppendLiteral(" retries");
                    throw new InterlockLedgerCryptographicException(defaultInterpolatedStringHandler.ToStringAndClear(), innerException);
                }
            }
        }
    }

    public static bool Verify<T>(T dataToVerify, TagSignature signature, EdDSAParameters parameters) where T : Signable<T>, new() {
        using var dataStream = dataToVerify.Required("dataToVerify").OpenReadingStreamAsync().Result;
        return VerifyStream(dataStream, signature, parameters);
    }

    public static bool Verify(byte[] dataToVerify, TagSignature signature, EdDSAParameters parameters) {
        using var dataStream = new MemoryStream(dataToVerify, writable: false);
        return VerifyStream(dataStream, signature, parameters);
    }

    private static EdDSACryptoServiceProvider OpenProvider(EdDSAParameters parameters) {
        var rSACryptoServiceProvider = new EdDSACryptoServiceProvider();
        rSACryptoServiceProvider.ImportParameters(parameters);
        return rSACryptoServiceProvider;
    }
    private static bool VerifyStream(Stream dataStream, TagSignature signature, EdDSAParameters parameters) {
        try {
            if (signature.Required("signature").Algorithm != 0) {
                var defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(54, 1);
                defaultInterpolatedStringHandler.AppendLiteral("Signature uses different algorithm ");
                defaultInterpolatedStringHandler.AppendFormatted(signature.Algorithm);
                defaultInterpolatedStringHandler.AppendLiteral(" from this EdDSA key!");
                throw new InvalidDataException(defaultInterpolatedStringHandler.ToStringAndClear());
            }

            if (!parameters.HasPrivatePart) {
                throw new InvalidDataException("This EdDSA key is not properly configured to be able to verify a signature!");
            }

            using var rSACryptoServiceProvider = OpenProvider(parameters);
            return rSACryptoServiceProvider.VerifyData(dataStream, signature.Data, HashAlgorithmName.SHA256);
        } catch (CryptographicException innerException) {
            throw new InterlockLedgerCryptographicException("Failed to verify data with current parameters and signature", innerException);
        }
    }
}
