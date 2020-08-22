/******************************************************************************************************************************

Copyright (c) 2018-2020 InterlockLedger Network
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this
  list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice,
  this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution.

* Neither the name of the copyright holder nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

******************************************************************************************************************************/

using System;
using System.IO;

namespace InterlockLedger.Tags
{
    public class FileBackedByteArray : ILTag
    {
        private const int _bufferLength = 16 * 1024;
        private readonly FileInfo _fileInfo;

        public FileBackedByteArray(FileInfo fileInfo, Stream source) : base(ILTagId.ByteArray) {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            _fileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
            using (var fileStream = fileInfo.OpenWrite())
                source.CopyTo(fileStream, _bufferLength);
            Initialize(0, 0, fileInfo.Length);
            NoRemoval = false;
        }

        public FileBackedByteArray(FileInfo fileInfo) : this(fileInfo, 0, 0) => NoRemoval = false;

        public FileBackedByteArray(FileInfo fileInfo, long offset, ulong length) : base(ILTagId.ByteArray) {
            if (fileInfo is null || !fileInfo.Exists)
                throw new ArgumentNullException(nameof(fileInfo));
            _fileInfo = fileInfo;
            Initialize(offset, length, fileInfo.Length);
            NoRemoval = true;
        }

        public override object AsJson => null;

        public override string Formatted => $"{_fileInfo.FullName}[{Offset}:{Length}]";

        public ulong Length { get; private set; }

        public long Offset { get; private set; }

        public bool NoRemoval { get; }

        public void Remove() {
            if (!NoRemoval)
                _fileInfo.Delete();
        }

        public override bool ValueIs<TV>(out TV value) {
            value = default;
            return false;
        }

        protected override void SerializeInner(Stream s) {
            s.ILIntEncode(Length);
            if (Length > 0) {
                using var fileStream = _fileInfo.OpenRead();
                using var streamSlice = new StreamSpan(fileStream, Offset, Length);
                streamSlice.CopyTo(s, _bufferLength);
            }
        }

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

    public class ILTagByteArray : ILTagExplicit<byte[]>
    {
        public ILTagByteArray(object opaqueValue) : this(Elicit(opaqueValue)) {
        }

        public ILTagByteArray(byte[] value) : base(ILTagId.ByteArray, value) {
        }

        public ILTagByteArray(Span<byte> value) : this(value.ToArray()) {
        }

        internal ILTagByteArray(Stream s) : base(ILTagId.ByteArray, s) {
        }

        protected override byte[] FromBytes(byte[] bytes) => bytes;

        protected override byte[] ToBytes() => Value;

        private static byte[] Elicit(object opaqueValue)
            => opaqueValue switch
            {
                byte[] value => value,
                string s => Convert.FromBase64String(s),
                _ => Array.Empty<byte>()
            };
    }
}