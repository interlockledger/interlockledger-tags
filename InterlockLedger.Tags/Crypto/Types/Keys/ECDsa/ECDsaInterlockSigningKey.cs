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

namespace InterlockLedger.Tags;
public class ECDsaInterlockSigningKey : InterlockSigningKey
{
    public ECDsaInterlockSigningKey(InterlockSigningKeyData data, byte[] decrypted) : base(data) {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        if (data.EncryptedContentType != EncryptedContentType.EncryptedKey)
            throw new ArgumentException($"Wrong kind of EncryptedContentType {data.EncryptedContentType}", nameof(data));
        using var ms = new MemoryStream(decrypted);
        _keyParameters = ms.DecodeAny<ECDsaParameters>();
    }

    public override byte[] AsSessionState {
        get {
            using var ms = new MemoryStream();
            ms.EncodeTag(_value);
            ms.EncodeTag(_keyParameters.AsPayload);
            return ms.ToArray();
        }
    }

    public static new ECDsaInterlockSigningKey FromSessionState(byte[] bytes) {
        using var ms = new MemoryStream(bytes);
        var tag = ms.Decode<InterlockSigningKeyData>();
        var parameters = ms.DecodeAny<ECDsaParameters>();
        return new ECDsaInterlockSigningKey(tag, parameters);
    }

    public override byte[] Decrypt(byte[] bytes) => throw new InvalidOperationException("ECDsa does not permit encryption");
    // ECDsaHelper.Decrypt(bytes, _keyParameters.Parameters);

    public override TagSignature Sign(byte[] data)
        => new(Algorithm.EcDSA, ECDsaHelper.HashAndSign(data, _keyParameters.Parameters, _keyParameters.HashAlgorithm.ToName()));

    public override TagSignature Sign<T>(T data)
        => new(Algorithm.EcDSA, ECDsaHelper.HashAndSignBytes(data, _keyParameters.Parameters, _keyParameters.HashAlgorithm.ToName()));

    private readonly ECDsaParameters _keyParameters;

    private ECDsaInterlockSigningKey(InterlockSigningKeyData tag, ECDsaParameters parameters) : base(tag)
        => _keyParameters = parameters;
}