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

namespace InterlockLedger.Tags;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class WrappedReadonlyStream : Stream
{
    protected readonly long _begin;
    protected readonly Stream _originalStream;

    /// <summary>
    /// Wrap around a stream as a readonly stream with origin at offset and length
    /// </summary>
    /// <param name="s">Original stream to be wrapped</param>
    /// <param name="offset">Origin (Position 0) of this instance in original stream terms</param>
    /// <param name="length">Length to limit reading for, if -1 no limit is imposed defauting to original stream Length</param>
    /// <param name="closeWrappedStreamOnDispose">When this is true wrapped stream will be disposed (closed) when this stream is disposed</param>
    /// <exception cref="ArgumentException">Stream s must be provided and readable and if non-seekable position must be at offset</exception>
    public WrappedReadonlyStream(Stream s, long offset, long length, bool closeWrappedStreamOnDispose) {
        _originalStream = s.Required();
        _closeWrappedStreamOnDispose = closeWrappedStreamOnDispose;
        if (!s.CanRead)
            throw new ArgumentException("original stream needs to be readable");
        if (!s.CanSeek)
            throw new ArgumentException("original stream needs to be seekable");
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
        if (offset != s.Position)
            s.Position = offset;
        _begin = s.Position;
        _length = length >= 0 ? length : s.Length - _begin;
        DebugHelperInitialize();
    }

    public WrappedReadonlyStream(Stream s) : this(s.Required(), s.Position, -1, closeWrappedStreamOnDispose: false) {
    }

    protected sealed override void Dispose(bool disposing) {
        base.Dispose(disposing);
        if (_closeWrappedStreamOnDispose)
            _originalStream.Dispose();
        else
            DisposingButNotClosingWrappedStream();
    }

    protected virtual void DisposingButNotClosingWrappedStream() { }

    public override bool CanRead => true;

    public override bool CanSeek => true;

    public override bool CanWrite => false;

    public override long Length => _length;

    public long PositionOnOriginalStream => _originalStream is WrappedReadonlyStream ss ? ss.PositionOnOriginalStream : _originalStream.Position;

    public override long Position {
        get => _originalStream.Position - _begin;
        set {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Can't position before start");
            if (value > Length)
                throw new ArgumentOutOfRangeException(nameof(value), "Can't position after end");
            _originalStream.Position = value + _begin;
        }
    }

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) => throw new StreamIsReadonlyException();

    public override void EndWrite(IAsyncResult asyncResult) => throw new StreamIsReadonlyException();

    public override void Flush() {
    }

    public override Task FlushAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public override int Read(byte[] buffer, int offset, int count) {
        if (buffer.Required().Length == 0 || count <= 0)
            return 0;
        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), "Value is less than zero");
        if (offset >= buffer.Length)
            throw new ArgumentOutOfRangeException(nameof(offset), "Value is more than the size of the buffer");
        if (offset + count > buffer.Length)
            count = buffer.Length - offset;
        if (Position + count > Length) {
            long newCount = Length - Position;
            if (newCount <= 0)
                return 0;
            count = newCount > int.MaxValue ? int.MaxValue : (int)newCount;
        }
        return _originalStream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin) {
        return _originalStream.Seek(AdjustOffset(offset, origin), SeekOrigin.Begin) - _begin;

        long AdjustOffset(long offset, SeekOrigin origin) =>
             origin switch {
                 SeekOrigin.Begin => ValidateWithinBounds(offset),
                 SeekOrigin.Current => ValidateWithinBounds(offset + Position),
                 SeekOrigin.End => ValidateWithinBounds(Length + offset),
                 _ => throw new ArgumentException($"Unknown origin {origin}")
             };
        long ValidateWithinBounds(long offset) =>
             offset < 0
                ? throw new ArgumentOutOfRangeException(nameof(offset), "Can't position before start")
                : offset > Length
                    ? throw new ArgumentOutOfRangeException(nameof(offset), "Can't position after end")
                    : offset + _begin;
    }

    public override void SetLength(long value) => throw new NotSupportedException("This StreamSpan can't have its length changed");

    public override void Write(byte[] buffer, int offset, int count) => throw new StreamIsReadonlyException();

    public override void Write(ReadOnlySpan<byte> buffer) => throw new StreamIsReadonlyException();

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => throw new StreamIsReadonlyException();

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) => throw new StreamIsReadonlyException();

    public override void WriteByte(byte value) => throw new StreamIsReadonlyException();

    [Conditional("DEBUG")]
    private void DebugHelperInitialize() {
        if (Length == 0)
            _someBytesForDebug = "Empty";
        else
            try {
                Position = 0;
                if (Length > 100) {
                    byte[] buffer = new byte[50];
                    string head = DumpBytes(buffer);
                    Position = Length - 50;
                    string tail = DumpBytes(buffer);
                    _someBytesForDebug = $"[{Length}] {head}... {tail}";
                } else {
                    byte[] buffer = new byte[Length];
                    _someBytesForDebug = $"[{Length}] {DumpBytes(buffer)}";
                }
            } catch (Exception e) {
                _someBytesForDebug = e.Message;
            } finally {
                Position = 0;
            }

        string DumpBytes(byte[] buffer) {
            _ = Read(buffer, 0, buffer.Length);
            var sb = new StringBuilder();
            foreach (byte b in buffer)
                _ = sb.Append(b).Append(' ');
            return sb.ToString();
        }
    }

    private readonly bool _closeWrappedStreamOnDispose;
    private readonly long _length;
    private string _someBytesForDebug = "?";
    public string GetDebuggerDisplay() => $"{GetType().Name} {_someBytesForDebug}";
}
