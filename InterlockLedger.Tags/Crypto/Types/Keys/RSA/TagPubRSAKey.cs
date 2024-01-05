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
public class TagPubRSAKey : TagPubKey
{
    public readonly RSAParameters Parameters;

    public TagPubRSAKey(RSAParameters parameters) : this(EncodeParameters(parameters)) {
    }

    public override KeyStrength Strength {
        get {
            using var RSAalg = new RSACryptoServiceProvider();
            RSAalg.ImportParameters(Parameters);
            return RSAalg.KeyStrengthGuess();
        }
    }

    public override byte[] Encrypt(byte[] bytes) => RSAHelper.Encrypt(bytes, Parameters);

    public override bool Verify<T>(T data, TagSignature signature)
        => RSAHelper.Verify(data, signature, Parameters);

    public override bool Verify(byte[] data, TagSignature signature)
        => RSAHelper.Verify(data, signature, Parameters);

    internal TagPubRSAKey(byte[] data) : base(Algorithm.RSA, data) => Parameters = DecodeParameters(Data);

    private static RSAParameters DecodeParameters(byte[] bytes) {
        if (bytes == null || bytes.Length == 0)
            return default;
        using var s = new MemoryStream(bytes);
        return s.Decode<TagRSAPublicParameters>().Required().Value;
    }

    private static byte[] EncodeParameters(RSAParameters parameters)
        => new TagRSAPublicParameters(parameters).EncodedBytes;
}