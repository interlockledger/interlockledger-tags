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

using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;

namespace InterlockLedger.Tags;

public static class EdDSAHelper
{
    public static TagEdDSAParameters Validate([NotNull]this TagEdDSAParameters? tagEdDSAParameters)
        => tagEdDSAParameters.Required().Value.Required().HasPrivatePart
            ? tagEdDSAParameters
            : throw new InvalidDataException("Key parameters don't have private key");

    public static TagEdDSAParameters CreateNewTagEdDSAParameters() {
        var keyPair = _keyPairGenerator.GenerateKeyPair();
        var privKey = (Ed25519PrivateKeyParameters)keyPair.Private;
        var pubKey = (Ed25519PublicKeyParameters)keyPair.Public;
        return new(new EdDSAParameters(pubKey, privKey));
    }

    public static byte[] HashAndSignStream(Stream dataStream, EdDSAParameters parameters) {
        var signer = new Ed25519Signer();
        signer.Init(forSigning: true, parameters.PrivateKeyParameters);
        var buffer = new byte[0x4000];
        do {
            int len = dataStream.Read(buffer, 0, buffer.Length);
            if (len == 0)
                return signer.GenerateSignature();
            signer.BlockUpdate(buffer, 0, len);
        } while (true);
    }

    public static bool Verify<T>(T dataToVerify, TagSignature signature, EdDSAParameters parameters) where T : Signable<T>, ICacheableTag, new() {
        using var dataStream = dataToVerify.Required().OpenReadingStreamAsync().WaitResult();
        return VerifyStream(dataStream, signature, parameters);
    }

    public static bool VerifyStream(Stream dataStream, TagSignature signature, EdDSAParameters parameters) {
        if (signature.Required().Algorithm != Algorithm.EdDSA)
            throw new InvalidDataException($"Signature uses different algorithm {signature.Algorithm} from this EdDSA key!");
        var validator = new Ed25519Signer();
        validator.Init(forSigning: false, parameters.PublicKeyParameters);
        var buffer = new byte[0x4000];
        do {
            int len = dataStream.Read(buffer, 0, buffer.Length);
            if (len == 0)
                return validator.VerifySignature(signature.Data);
            validator.BlockUpdate(buffer, 0, len);
        } while (true);
    }

    private static readonly Ed25519KeyPairGenerator _keyPairGenerator = InitGenerator();
    private static Ed25519KeyPairGenerator InitGenerator() {
        var generator = new Ed25519KeyPairGenerator();
        generator.Init(new Ed25519KeyGenerationParameters(new SecureRandom()));
        return generator;
    }
}
