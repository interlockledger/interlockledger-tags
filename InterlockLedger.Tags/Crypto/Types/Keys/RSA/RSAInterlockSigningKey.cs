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

public class RSAInterlockSigningKey : InterlockSigningKey, IDecryptingKey
{
    public RSAInterlockSigningKey(InterlockSigningKeyData data, byte[] decrypted) : this(data, DecodeTagRSAParameters(decrypted)) {
    }

    public byte[] Decrypt(byte[] bytes) => RSAHelper.Decrypt(bytes, _keyParameters);
    private readonly TagRSAParameters _tagRSAParameters;
    private readonly RSAParameters _keyParameters;
    public RSAInterlockSigningKey(InterlockSigningKeyData data, TagRSAParameters tagRSAParameters) : base(data.ValidateIsEncryptedKey()) {
        _tagRSAParameters = tagRSAParameters.Required();
        _keyParameters = _tagRSAParameters.Value.Required().Parameters;
    }

    private static TagRSAParameters DecodeTagRSAParameters(byte[] decrypted) {
        using var ms = new MemoryStream(decrypted);
        return ms.Decode<TagRSAParameters>().Required();
    }

    protected override byte[] HashAndSignStream(Stream dataStream) => RSAHelper.HashAndSignStream(dataStream, _keyParameters);
    protected override bool VerifySignatureOnStream(Stream dataStream, TagSignature signature) => RSAHelper.VerifyStream(dataStream, signature, _keyParameters);
}