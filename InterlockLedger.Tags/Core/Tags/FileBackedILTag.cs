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

using System.Diagnostics;

namespace InterlockLedger.Tags;

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class FileBackedILTag<T> : ILTagOfExplicit<T> where T : notnull
{
    public FileBackedILTag(ulong tagId, FileInfo fileInfo, Stream source) : this(tagId) {
        _fileInfo = fileInfo.Required();
        CopyFromAsync(source).Wait();
    }

    public FileBackedILTag(ulong tagId, FileInfo fileInfo, long offset, ulong length) : this(tagId) {
        if (fileInfo is null || !fileInfo.Exists)
            throw new ArgumentNullException(nameof(fileInfo));
        _fileInfo = fileInfo;
        Initialize(offset, length, fileInfo.Length);
    }

    public override object AsJson => null;

    public FileInfo FileInfo => _fileInfo ?? throw new InvalidOperationException($"This instance of {TagTypeName} has not been set with a backing file");

    public ulong Length { get; private set; }
    public long Offset { get; private set; }

    public Stream ReadingStream =>
        Length == 0
            ? throw new InvalidOperationException("Should not try to deserialize a zero-length tag")
            : _contentStream;

    public override Task<Stream> OpenReadingStreamAsync() => Task.FromResult<Stream>(new TagStream(TagId, _contentStream));

    public override bool ValueIs<TV>(out TV value) {
        value = default;
        return false;
    }

    protected FileBackedILTag(ulong tagId, FileInfo fileInfo) : this(tagId) {
        _fileInfo = fileInfo.Required();
        if (fileInfo.Exists)
            Initialize(0, 0, fileInfo.Length);
    }

    protected FileBackedILTag(ulong tagId) : base(tagId, default) {
        Length = 0;
        Offset = 0;
    }

    protected override bool KeepEncodedBytesInMemory => false;

    protected string TagTypeName => $"{GetType().Name}#{TagId}";

    protected override ulong CalcValueLength() => Length;

    protected async Task CopyFromAsync(Stream source, long fileSizeLimit = 0, bool noRemoval = false, CancellationToken cancellationToken = default) {
        source.Required();
        using (var fileStream = FileInfo.OpenWrite()) {
            await source.CopyToAsync(fileStream, fileSizeLimit, _bufferLength, cancellationToken).ConfigureAwait(false);
            fileStream.Flush(flushToDisk: true);
        }
        Refresh();
    }

    protected void Refresh() {
        FileInfo.Refresh();
        Initialize(0, 0, FileInfo.Length);
    }

    protected override T ValueFromStream(StreamSpan s) => default;

    protected override Task<Stream> ValueToStreamAsync(Stream s) {
        using var fileStream = FileInfo.OpenRead();
        using var streamSlice = new StreamSpan(fileStream, Offset, Length);
        streamSlice.CopyTo(s, _bufferLength);
        return Task.FromResult(s);
    }

    private const int _bufferLength = 16 * 1024;

    private readonly FileInfo _fileInfo;

    private StreamSpan _contentStream
        => !FileInfo.Exists || FileInfo.Length == 0
            ? throw new InvalidOperationException("Nothing to read here")
            : new StreamSpan(FileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete)
                , Offset, Length, closeWrappedStreamOnDispose: true);

    private string GetDebuggerDisplay() => $"{TagTypeName}: {_fileInfo?.FullName ?? "?"}[{Offset}:{Length}]";

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

    private class TagStream : Stream
    {
        public TagStream(ulong tagId, StreamSpan stream) {
            _stream = stream.Required();
            _tagId = tagId.AsILInt();
            _contentLength = ((ulong)_stream.Length).AsILInt();
            _length = _tagId.Length + _contentLength.Length + _stream.Length;
            _position = 0;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => _length;

        public override long Position { get => _position; set => throw new NotSupportedException(); }

        public override void Flush() { }

        public override int Read(byte[] buffer, int offset, int count) {
            if (_position >= _length)
                return 0;
            buffer.Required();
            if (offset < 0 || offset >= buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0 || offset + count > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(count));
            int howMany = 0;
            int tagIdLength = _tagId.Length;
            int prefixLength = tagIdLength + _contentLength.Length;
            while (count > 0) {
                if (_position < tagIdLength) {
                    buffer[offset++] = _tagId[_position++];
                    howMany++;
                    count--;
                } else if (_position < prefixLength) {
                    buffer[offset++] = _contentLength[_position++ - tagIdLength];
                    howMany++;
                    count--;
                } else {
                    long start = _position - prefixLength;
                    if (_position >= _length)
                        break;
                    _stream.Seek(start, SeekOrigin.Begin);
                    int bytesRead = _stream.Read(buffer, offset, count);
                    if (bytesRead > 0) {
                        howMany += bytesRead;
                        _position += bytesRead;
                        count -= bytesRead;
                    }
                }
            }
            return howMany;
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            _stream.Dispose();
        }

        private readonly byte[] _contentLength;
        private readonly long _length;
        private readonly StreamSpan _stream;
        private readonly byte[] _tagId;
        private long _position;
    }
}