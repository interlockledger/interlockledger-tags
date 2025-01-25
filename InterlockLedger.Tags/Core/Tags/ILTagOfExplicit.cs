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

public abstract class ILTagOfExplicit<T> : ILTagOf<T?>
{
    [JsonIgnore]
    public ulong ValueLength => _valueLength ??= CalcValueLength();

    protected ILTagOfExplicit(ulong tagId, T? value) : base(tagId, value) {
        if (value is ICacheableTag cacheable) {
            using var source = InternalOpenReadingStreamAsync().WaitResult();
            cacheable.CacheFromAsync(source).WaitResult();
        }
    }

    protected ILTagOfExplicit(ulong alreadyDeserializedTagId, Stream s, Action<ITag>? setup = null) : base(alreadyDeserializedTagId, s, setup) {
    }

    protected static void SetLength(ITag it, ulong length) => ((ILTagOfExplicit<T>)it)._valueLength = length;
    protected virtual ulong CalcValueLength() {
        if (Value is null)
            return 0;
        using var stream = new CountingStream();
        ValueToStreamAsync(stream).WaitResult();
        stream.Flush();
        return (ulong)stream.Length;
    }
    private protected sealed async override Task<T?> DeserializeInnerAsync(Stream s) {
        ulong length = _valueLength ??= s.ILIntDecode();
        if (length == 0)
            return ZeroLengthDefault;
        if ((s.Length - s.Position) < (long)length)
            throw new InvalidDataException($"Decoded tag content length ({length}) is larger than total available bytes in stream");
        using var ss = new StreamSpan(s, length);
        var value = await ValueFromStreamAsync(ss);
        if (value is ICacheableTag cacheable)
            await cacheable.CachePrefixedFromAsync(ss).ConfigureAwait(false);
        return value;
    }
    public override async Task<Stream> OpenReadingStreamAsync() =>
        Value is ICacheableTag cacheable
            ? await cacheable.OpenCachedReadingStreamAsync()
            : await InternalOpenReadingStreamAsync();

    private async Task<Stream> InternalOpenReadingStreamAsync() {
        var s = await BuildTempStreamAsync();
        await SerializeIntoAsync(s);
        s.Position = 0;
        return new StreamSpan(s, 0, (ulong)s.Length, closeWrappedStreamOnDispose: true);
    }

    [JsonIgnore]
    protected virtual T? ZeroLengthDefault => default;
    protected override void DisposeManagedResources() { }

    protected async override Task<Stream> SerializeInnerAsync(Stream s) {
        if (Value is null)
            s.ILIntEncode(0ul);
        else {
            s.ILIntEncode(ValueLength);
            if (ValueLength > 0)
                await ValueToStreamAsync(s);
        }
        return s;
    }

    private ulong? _valueLength;
}