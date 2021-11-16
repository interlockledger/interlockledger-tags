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
public abstract class ILTagOfExplicit<T> : ILTagOf<T>
{
    public const int MaxEncodedValueLength = int.MaxValue / 16;

    [JsonIgnore]
    public ulong ValueLength => _valueLength ??= CalcValueLength();

    protected ILTagOfExplicit(ulong tagId, T value) : base(tagId, value) {
    }

    protected ILTagOfExplicit(ulong alreadyDeserializedTagId, Stream s, Action<ITag> setup = null)
        : base(alreadyDeserializedTagId, s, setup) {
    }

    protected static void SetLength(ITag it, ulong length) => ((ILTagOfExplicit<T>)it)._valueLength = length;

    protected virtual ulong CalcValueLength() {
        using var stream = new MemoryStream();
        ValueToStreamAsync(stream);
        stream.Flush();
        return (ulong)stream.ToArray().Length;
    }

    protected sealed override T DeserializeInner(Stream s) {
        if ((_valueLength ??= s.ILIntDecode()) > int.MaxValue && KeepEncodedBytesInMemory)
            throw new InvalidDataException("Tag content is TOO BIG to deserialize!");
        using var ss = new StreamSpan(s, _valueLength.Value);
        return ValueFromStream(ss);
    }

    protected sealed override void OnChanged() {
        base.OnChanged();
        _valueLength = null;
    }

    protected sealed override Task<Stream> SerializeInnerAsync(Stream s) {
        s.ILIntEncode(ValueLength);
        if (ValueLength > 0)
            ValueToStreamAsync(s);
        return Task.FromResult(s);
    }

    private ulong? _valueLength;
}