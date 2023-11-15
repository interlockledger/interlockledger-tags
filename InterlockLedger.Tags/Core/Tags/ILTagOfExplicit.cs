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


namespace InterlockLedger.Tags;

public abstract class ILTagOfExplicit<T> : ILTagOf<T?>
{
    public const int MaxEncodedValueLength = int.MaxValue / 16;

    [JsonIgnore]
    public ulong ValueLength => _valueLength ??= CalcValueLength();

    protected ILTagOfExplicit(ulong tagId, T? value) : base(tagId, value) {
    }

    protected ILTagOfExplicit(ulong alreadyDeserializedTagId, Stream s, Action<ITag>? setup = null)
        : base(alreadyDeserializedTagId, s, setup) {
    }

    protected static void SetLength(ITag it, ulong length) => ((ILTagOfExplicit<T>)it)._valueLength = length;

    protected virtual ulong CalcValueLength() {
        if (Value is null)
            return 0;
        using var stream = new MemoryStream();
        ValueToStreamAsync(stream).Wait();
        stream.Flush();
        return (ulong)stream.ToArray().Length;
    }

    private protected sealed async override Task<T?> DeserializeInnerAsync(Stream s) {
        ulong length = _valueLength ??= s.ILIntDecode();
        if (length > int.MaxValue && KeepEncodedBytesInMemory)
            throw new InvalidDataException("Tag content is TOO BIG to deserialize!");
        if (length == 0)
            return ZeroLengthDefault;
        if (s is StreamSpan sp && (ulong)(sp.Length - sp.Position) < length)
            throw new InvalidDataException($"Decoded tag content length ({length}) is larger than total available bytes in stream");
        if (length > 128 * 1024ul) {
            _cache = await CachedTagPart.ExtractTagPartAsync(s, TagId, length);
            return await _cache.PerformReadingOfStreamAsync(ValueFromStreamAsync, justBody: true);
        }
        using var ss = new StreamSpan(s, length);
        return await ValueFromStreamAsync(ss);
    }
    public override async Task<Stream> OpenReadingStreamAsync() {
        if (_cache is not null)
            return await _cache.OpenReadingStreamAsync();
        if (KeepEncodedBytesInMemory)
            return new MemoryStream(EncodedBytes, writable: false);
        var s = await BuildTempStreamAsync();
        await SerializeIntoAsync(s);
        s.Position = 0;
        return new StreamSpan(s, 0, (ulong)s.Length, closeWrappedStreamOnDispose: true);
    }

    [JsonIgnore]
    protected virtual T? ZeroLengthDefault => default;
    protected async override Task<Stream> SerializeInnerAsync(Stream s) {
        if (Value is not null) {
            s.ILIntEncode(ValueLength);
            if (ValueLength > 0)
                await ValueToStreamAsync(s);
        } else
            s.ILIntEncode(0ul);
        return s;
    }

    protected sealed override void OnChanged() {
        base.OnChanged();
        _valueLength = null;
    }

    private ulong? _valueLength;
    private CachedTagPart? _cache;

    protected override void DisposeManagedResources() {
        _cache?.Dispose();
        _cache = null;
    }
}