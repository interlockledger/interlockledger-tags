// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2025 InterlockLedger Network
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

public static class EcDSAHelper
{
    public static TagEcDSAParameters Validate([NotNull] this TagEcDSAParameters? tagEcDSAParameters)
        => tagEcDSAParameters.Required().Value.D is not null
            ? tagEcDSAParameters
            : throw new InvalidDataException("Key parameters don't have private key");

    public static TagEcDSAParameters CreateNewTagEcDSAParameters() {
        var keyPair = ECDsa.Create();
        return new (keyPair.ExportParameters(includePrivateParameters: true));
    }

    public static byte[] HashAndSignStream(Stream dataStream, ECParameters parameters) {
        int retries = _maxRetries;
        while (true)
            try {
                using var ecdsa = OpenProvider(parameters);
                return ecdsa.SignData(dataStream, HashAlgorithmName.SHA256, DSASignatureFormat.Rfc3279DerSequence);
            } catch (CryptographicException e) {
                if (retries-- <= 0)
                    throw new InterlockLedgerCryptographicException($"Failed to sign data with current parameters after {_maxRetries} retries", e);
            }
    }

    private static ECDsa OpenProvider(ECParameters parameters) => ECDsa.Create(parameters);

    public static bool Verify<T>(T dataToVerify, TagSignature signature, ECParameters parameters) where T : Signable<T>, ICacheableTag, new() {
        using var dataStream = dataToVerify.Required().OpenReadingStreamAsync().WaitResult();
        return VerifyStream(dataStream, signature, parameters);
    }

    public static bool VerifyStream(Stream dataStream, TagSignature signature, ECParameters parameters) {
        if (signature.Required().Algorithm != Algorithm.EcDSA)
            throw new InvalidDataException($"Signature uses different algorithm {signature.Algorithm} from this EcDSA key!");
        using var ecdsa = OpenProvider(parameters);
        return ecdsa.VerifyData(dataStream, signature.Data, HashAlgorithmName.SHA256, DSASignatureFormat.Rfc3279DerSequence);
    }

    private const int _maxRetries = 3;

}
