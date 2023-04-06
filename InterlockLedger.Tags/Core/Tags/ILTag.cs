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

public abstract class ILTag : ITag
{
    [JsonIgnore]
    public byte[] EncodedBytes => KeepEncodedBytesInMemory ? (_encodedBytes ??= ToBytes()) : ToBytes();

    public ulong TagId { get; set; }

    public abstract object? Content { get;}

    [JsonIgnore]
    public ITag Traits => this;

    [JsonIgnore]
    public string TextualRepresentation { get; protected set; }

    internal void Changed() {
        _encodedBytes = null;
        OnChanged();
    }

    public virtual async Task<Stream> OpenReadingStreamAsync() {
        if (KeepEncodedBytesInMemory)
            return new MemoryStream(EncodedBytes, writable: false);
        var s = await BuildTempStreamAsync();
        await SerializeIntoAsync(s);
        s.Position = 0;
        return new StreamSpan(s, 0, (ulong)s.Length, closeWrappedStreamOnDispose: true);
    }

    public async Task<Stream> SerializeIntoAsync(Stream s) {
        s.ILIntEncode(TagId);
        await SerializeInnerAsync(s);
        s.Flush();
        return s;
    }

    public override string ToString() => TagPrefix + TextualRepresentation;

    [JsonIgnore]
    public string TagPrefix => GetType().Name + $"[Tag#{TagId}]: ";

    public virtual bool ValueIs<TV>(out TV? value) {
        value = default;
        return false;
    }

    protected ILTag(ulong tagId, string textualRepresentation) {
        TagId = tagId;
        TextualRepresentation = textualRepresentation;
    }

    [JsonIgnore]
    protected virtual bool KeepEncodedBytesInMemory => true;

    protected virtual Task<Stream> BuildTempStreamAsync() => Task.FromResult<Stream>(new MemoryStream());

    protected abstract Task SerializeInnerAsync(Stream s);

    protected virtual void OnChanged() { }

    private byte[]? _encodedBytes;

    private byte[] ToBytes() {
        using var stream = new MemoryStream();
        SerializeIntoAsync(stream).Wait();
        stream.Flush();
        return stream.ToArray();
    }
}