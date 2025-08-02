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

public class FakeLargeILTagByteArrayStream : Stream
{
    public FakeLargeILTagByteArrayStream(ulong fakeSize) {
        var _lengthBytes = fakeSize.AsILInt();
        _prefixLength = 1 + _lengthBytes.Length;
        _prefixBytes = new byte[_prefixLength];
        _prefixBytes[0] = (byte)ILTagId.ByteArray;
        Array.Copy(_lengthBytes, 0, _prefixBytes, 1, _lengthBytes.Length);
        Length = _prefixLength + (long)fakeSize;
        _position = 0;
    }
    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => false;
    public override long Length { get; }
    public override long Position { get => _position; set => _position = value; }

    private readonly byte[] _prefixBytes;
    private readonly int _prefixLength;
    private long _position;

    public override void Flush() { }
    public override int Read(byte[] buffer, int offset, int count) {
        buffer.Required();
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(offset, buffer.Length);
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(count, buffer.Length - offset);
        int written;
        for (written = 0; written < count; written++) {
            if (_position >= Length)
                break;
            buffer[offset++] = _position < _prefixLength ? (byte)_prefixBytes[_position] : (byte)0;
            _position++;
        }
        return written;
    }

    public override long Seek(long offset, SeekOrigin origin) =>
        origin switch {
            SeekOrigin.Begin => ValidateWithinBounds(offset),
            SeekOrigin.Current => ValidateWithinBounds(offset + Position),
            SeekOrigin.End => ValidateWithinBounds(Length + offset),
            _ => throw new ArgumentException($"Unknown origin {origin}")
        };

    private long ValidateWithinBounds(long offset) =>
        offset < 0
        ? throw new ArgumentOutOfRangeException(nameof(offset), "Can't position before start")
        : offset > Length
            ? throw new ArgumentOutOfRangeException(nameof(offset), "Can't position after end")
            : offset;
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}