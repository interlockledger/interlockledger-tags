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


namespace InterlockLedger.Tags;

public class EdDSAInterlockUpdatableSigningKey : InterlockUpdatableSigningKey
{
    private bool _destroyKeysAfterSigning;

    private TagEdDSAParameters? _keyParameters;

    private TagEdDSAParameters? _nextKeyParameters;

    public override TagPubKey? NextPublicKey => (_nextKeyParameters ?? _keyParameters)?.PublicKey;

    public EdDSAInterlockUpdatableSigningKey(InterlockUpdatableSigningKeyData tag, byte[] decrypted, ITimeStamper timeStamper)
        : base(tag, timeStamper) {
        using var s = new MemoryStream(decrypted);
        _keyParameters = s.Decode<TagEdDSAParameters>().Validate();
    }

    public override void DestroyKeys() => _destroyKeysAfterSigning = true;

    public override void GenerateNextKeys() => _nextKeyParameters = EdDSAHelper.CreateNewTagEdDSAParameters();

    public override TagSignature SignAndUpdate(byte[] data, Func<byte[], byte[]>? encrypt = null) => Update(encrypt, EdDSAHelper.HashAndSign(data, _parameters));
    public override TagSignature SignAndUpdate<T>(T data, Func<byte[], byte[]>? encrypt = null) => Update(encrypt, EdDSAHelper.HashAndSignBytes(data, _parameters));
    public override TagSignature SignAndUpdate(Stream dataStream, Func<byte[], byte[]>? encrypt = null) => Update(encrypt, EdDSAHelper.HashAndSign(dataStream.ReadAllBytesAsync().WaitResult(), _parameters));
    private EdDSAParameters _parameters => _keyParameters.Required().Value.Required();

    private TagSignature Update(Func<byte[], byte[]>? encrypt, byte[] signatureData) {
        _ = _data.Value.Required();
        if (_destroyKeysAfterSigning) {
            _keyParameters = null;
            _nextKeyParameters = null;
            _data.Value.Encrypted = null!;
            _data.Value.PublicKey = null!;
        } else {
            var func = encrypt.Required("encrypt");
            if (_nextKeyParameters is not null) {
                _keyParameters = _nextKeyParameters;
                _data.Value.Encrypted = func(_keyParameters!.EncodedBytes());
                _data.Value.PublicKey = _keyParameters!.PublicKey;
                _nextKeyParameters = null;
                _data.SignaturesWithCurrentKey = 0uL;
            } else {
                _data.SignaturesWithCurrentKey++;
            }
            _data.LastSignatureTimeStamp = _timeStamper.Now;
        }
        return new TagSignature(Algorithm.EdDSA, signatureData);
    }

}
