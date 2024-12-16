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



using System.Buffers;

namespace InterlockLedger.Tags;
public interface ISignable
{
    ILTag ResolveSigned(ushort version, Stream s);
}

public abstract class Signable<T> : VersionedValue<T>, ISignable, ICacheableTag, IDisposable where T : Signable<T>, new()
{
    private int _cachedLength;
    private IMemoryOwner<byte>? _memoryOwner;
    private bool _disposedValue;

    public Signable() : base(0, 0) {
    }

    public ILTag ResolveSigned(ushort version, Stream s) => new SignedValue<T>(version, (T)this, s).AsPayload;

    public SignedValue<T> SignWith(ISigningContext context)
        => new((T)this, context.Required().Key.Required().SignWithId((T)this).AsSingle());


    async Task ICacheableTag.CacheFromAsync(Stream source) {
        if (_memoryOwner is not null)
            throw new InvalidOperationException("Trying to cache more than once");
        if (source.Length > int.MaxValue)
            throw new OutOfMemoryException("Tag too big to cache");
        _cachedLength = (int)source.Length;
        _memoryOwner = MemoryPool<byte>.Shared.Rent(_cachedLength);
        source.Seek(0, SeekOrigin.Begin);
        var readBytes = await source.ReadAsync(_memoryOwner.Memory).ConfigureAwait(false);
        if (readBytes != _cachedLength)
            throw new InvalidOperationException("Could not read all expected byte from tag to cache");
    }

    async Task<Stream> ICacheableTag.OpenCachedReadingStreamAsync() {
        if (_memoryOwner is null || _cachedLength <= 0) {
            return new MemoryStream([], writable: false);
        }
        var ms = new MemoryStream(_cachedLength);
        await ms.WriteAsync(_memoryOwner.Memory[0.._cachedLength]).ConfigureAwait(false);
        ms.Position = 0;
        return ms;
    }


    protected Signable(ulong tagId, ushort version) : base(tagId, version) {
    }

    protected virtual void Dispose(bool disposing) {
        if (!_disposedValue) {
            if (disposing) {
                _memoryOwner?.Dispose();
                _memoryOwner = null;
                _cachedLength = 0;
            }
            _disposedValue = true;
        }
    }

    public void Dispose() {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}