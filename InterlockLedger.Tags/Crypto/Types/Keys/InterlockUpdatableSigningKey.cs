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

public abstract class InterlockUpdatableSigningKey : InterlockSigningKeyOf<InterlockUpdatableSigningKeyData>, IUpdatableSigningKey
{
    public BaseKeyId Identity => KeyData.Identity;
    public DateTimeOffset LastSignatureTimeStamp => KeyData.LastSignatureTimeStamp;
    public abstract TagPubKey? NextPublicKey { get; }
    public Algorithm SignAlgorithm => KeyData.PublicKey.Algorithm;
    public ulong SignaturesWithCurrentKey => KeyData.SignaturesWithCurrentKey;
    public abstract void DestroyKeys();
    public abstract void GenerateNextKeys();
    public override TagSignature Sign<T>(T data) => throw new InvalidOperationException("Can't sign without possibly updating the key");
    public override TagSignature Sign(Stream dataStream) => throw new InvalidOperationException("Can't sign without possibly updating the key");
    public abstract TagSignature SignAndUpdate<T>(T data, Func<byte[], byte[]>? encrypt = null) where T : Signable<T>, new();
    public abstract TagSignature SignAndUpdate(Stream dataStream, Func<byte[], byte[]>? encrypt = null);
    public override string ToShortString() => $"UpdatableSigningKey '{Name}' [{Purposes.ToStringAsList()}]";
    public async Task SaveToAsync(Stream store) {
        using var s = await KeyData.OpenReadingStreamAsync().ConfigureAwait(false);
        await s.CopyToAsync(store).ConfigureAwait(false);
        await store.FlushAsync().ConfigureAwait(false);
    }

    protected readonly ITimeStamper _timeStamper;

    protected InterlockUpdatableSigningKey(InterlockUpdatableSigningKeyData tag, ITimeStamper timeStamper) : base(tag) {
        _timeStamper = timeStamper.Required();
        KeyData.LastSignatureTimeStamp = _timeStamper.Now;
    }
}
