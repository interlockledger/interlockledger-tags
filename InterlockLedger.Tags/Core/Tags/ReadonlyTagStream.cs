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
public class ReadonlyTagStream : Stream
{
    public ReadonlyTagStream(ulong tagId, byte[] bytes) {
        _bytes = bytes ?? Array.Empty<byte>();
        _tagId = tagId.AsILInt();
        _contentLength = ((ulong)bytes.Length).AsILInt();
        _length = _tagId.Length + _contentLength.Length + _bytes.Length;
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
                int start = _position - prefixLength;
                int bytesToCopy = Math.Min(count, _bytes.Length - start);
                if (bytesToCopy > 0) {
                    _bytes.AsMemory(start, bytesToCopy).CopyTo(buffer.AsMemory(offset, bytesToCopy));
                    howMany += bytesToCopy;
                    _position += bytesToCopy;
                }
                count = 0;
            }
        }
        return howMany;
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    private readonly byte[] _bytes;
    private readonly byte[] _contentLength;
    private readonly long _length;
    private readonly byte[] _tagId;
    private int _position;
}