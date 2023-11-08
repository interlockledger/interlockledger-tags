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

using System.Diagnostics;
using System.IO.MemoryMappedFiles;

namespace InterlockLedger.Tags;
[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class MemoryMappedFileBackedILTag<T> : ILTagOfExplicit<T> where T : notnull
{
    public MemoryMappedFileBackedILTag(ulong tagId, MemoryMappedViewStream mmvs, long offset, ulong length) : base(tagId, default!) {
        _mmvs = mmvs.Required();
        Initialize(offset, length, mmvs.Length);
    }

    public ulong Length { get; private set; }
    public long Offset { get; private set; }

    [JsonIgnore]
    public Stream ReadingStream =>
        Length == 0
            ? throw new InvalidOperationException("Should not try to deserialize a zero-length tag")
            : new StreamSpan(_mmvs, Offset, Length, closeWrappedStreamOnDispose: false);

    public override Task<Stream> OpenReadingStreamAsync() => Task.FromResult(ReadingStream);

    public override bool ValueIs<TV>(out TV value) {
        value = default!;
        return false;
    }

    protected string TagTypeName => $"{GetType().Name}#{TagId}";

    protected override T ValueFromStream(Stream s) => default!;

    protected override Stream ValueToStream(Stream s) {
        using var streamSlice = new StreamSpan(_mmvs, Offset, Length);
        streamSlice.CopyTo(s, _bufferLength);
        return s;
    }

    protected override ulong CalcValueLength() => Length;

    private const int _bufferLength = 16 * 1024;
    private readonly MemoryMappedViewStream _mmvs;

    private string GetDebuggerDisplay() => $"{TagTypeName}: [{Offset}:{Length}]";

    private void Initialize(long offset, ulong length, long fileLength) {
        if (offset < 0 || offset > fileLength)
            throw new ArgumentOutOfRangeException(nameof(offset));
        Offset = offset;
        Length = length == 0
            ? (ulong)(fileLength - offset)
            : length >= long.MaxValue || (offset + (long)length) > fileLength
                ? throw new ArgumentOutOfRangeException(nameof(length))
                : length;
    }
}