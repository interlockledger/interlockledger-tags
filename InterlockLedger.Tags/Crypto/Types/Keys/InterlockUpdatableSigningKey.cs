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

namespace InterlockLedger.Tags;



public abstract class InterlockUpdatableSigningKey<TP, TK> : InterlockSigningKeyOf<InterlockUpdatableSigningKeyData>, IUpdatableSigningKey where TP : ILTagOfExplicit<TK>, IKeyParameters  {
    public BaseKeyId Identity => KeyData.Identity;
    public DateTimeOffset LastSignatureTimeStamp => KeyData.LastSignatureTimeStamp;
    public TagPubKey? NextPublicKey => (NextKeyParameters ?? KeyParameters)?.PublicKey;
    public Algorithm SignAlgorithm => KeyData.PublicKey.Algorithm;
    public ulong SignaturesWithCurrentKey => KeyData.SignaturesWithCurrentKey;
    public override string ToShortString() => $"UpdatableSigningKey '{Name}' {SignAlgorithm} [{Purposes.ToStringAsList()}]";
    public async Task SaveToAsync(Stream store) {
        using var s = await KeyData.OpenReadingStreamAsync().ConfigureAwait(false);
        await s.CopyToAsync(store).ConfigureAwait(false);
        await store.FlushAsync().ConfigureAwait(false);
    }
    private readonly ITimeStamper _timeStamper;
    protected InterlockUpdatableSigningKey(InterlockUpdatableSigningKeyData tag, ITimeStamper timeStamper) : base(tag) {
        _timeStamper = timeStamper.Required();
        KeyData.LastSignatureTimeStamp = _timeStamper.Now;
    }
    private bool _destroyKeysAfterSigning;
    protected TP? KeyParameters;
    protected TP? NextKeyParameters;
    protected TK Parameters => KeyParameters.Required().Value!;
    public void DestroyKeys() => _destroyKeysAfterSigning = true;
    public void GenerateNextKeys() => NextKeyParameters = CreateNewParameters();
    protected abstract TP CreateNewParameters();
    public TagSignature SignAndUpdate<TD>(TD data, Func<byte[], byte[]>? encrypt = null) where TD : Signable<TD>, new()  => Update(encrypt, HashAndSignStream(data.OpenReadingStreamAsync().WaitResult()));
    public TagSignature SignAndUpdate(Stream dataStream, Func<byte[], byte[]>? encrypt = null) => Update(encrypt, HashAndSignStream(dataStream));
    private TagSignature Update(Func<byte[], byte[]>? encrypt, byte[] signatureData) {
        _ = KeyData.Value.Required();
        if (_destroyKeysAfterSigning) {
            KeyParameters = null;
            NextKeyParameters = null;
            KeyData.Value.Encrypted = null!;
            KeyData.Value.PublicKey = null!;
        } else {
            if (NextKeyParameters is not null) {
                KeyParameters = NextKeyParameters;
                KeyData.Value.Encrypted = encrypt.Required()(KeyParameters!.EncodedBytes());
                KeyData.Value.PublicKey = KeyParameters!.PublicKey;
                NextKeyParameters = null;
                KeyData.SignaturesWithCurrentKey = 0uL;
            } else {
                KeyData.SignaturesWithCurrentKey++;
            }
            KeyData.LastSignatureTimeStamp = _timeStamper.Now;
        }
        return new TagSignature(SignAlgorithm, signatureData);
    }
}
