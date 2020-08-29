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
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace InterlockLedger.Tags
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class FileBackedILTag<T> : ILTagExplicitBase<T>
    {
        public FileBackedILTag(ulong tagId, FileInfo fileInfo, Stream source) : this(tagId) {
            _fileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
            CopyFromAsync(source).Wait();
        }

        public FileBackedILTag(ulong tagId, FileInfo fileInfo, long offset, ulong length) : this(tagId) {
            if (fileInfo is null || !fileInfo.Exists)
                throw new ArgumentNullException(nameof(fileInfo));
            _fileInfo = fileInfo;
            Initialize(offset, length, fileInfo.Length);
            NoRemoval = true;
        }

        public override object AsJson => null;

        public FileInfo FileInfo => _fileInfo ?? throw new InvalidOperationException($"This instance of {TagTypeName} has not been set with a backing file");
        public override string Formatted => TagTypeName;

        public ulong Length { get; private set; }
        public bool NoRemoval { get; private set; }
        public long Offset { get; private set; }

        public Stream ReadingStream =>
            Length == 0
                ? throw new InvalidOperationException("Should not try to deserialize a zero-length tag")
                : !FileInfo.Exists || FileInfo.Length == 0
                    ? throw new InvalidOperationException("Nothing to read here")
                    : new StreamSpan(FileInfo.OpenRead(), Offset, Length, closeWrappedStreamOnDispose: true);

        public void Remove() {
            if (!NoRemoval) {
                FileInfo.Delete();
                Length = 0;
            }
        }

        public override bool ValueIs<TV>(out TV value) {
            value = default;
            return false;
        }

        protected FileBackedILTag(ulong tagId, FileInfo fileInfo, bool noRemoval) : this(tagId) {
            _fileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
            NoRemoval = noRemoval;
            if (fileInfo.Exists)
                Initialize(0, 0, fileInfo.Length);
        }

        protected FileBackedILTag(ulong tagId) : base(tagId, default) {
            NoRemoval = true;
            Length = 0;
            Offset = 0;
        }

        protected string TagTypeName => $"{GetType().Name}#{TagId}";

        protected async Task CopyFromAsync(Stream source, bool noRemoval = false, CancellationToken cancellationToken = default) {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            using (var fileStream = FileInfo.OpenWrite()) {
                await source.CopyToAsync(fileStream, _bufferLength, cancellationToken).ConfigureAwait(false);
                await fileStream.FlushAsync(cancellationToken).ConfigureAwait(false);
            }
            NoRemoval = noRemoval;
            FileInfo.Refresh();
            Initialize(0, 0, FileInfo.Length);
        }

        protected override T DeserializeValueFromStream(Stream s, ulong length) => default;

        protected override ulong GetValueEncodedLength() => Length;

        protected override void SerializeValueToStream(Stream s) {
            using var fileStream = FileInfo.OpenRead();
            using var streamSlice = new StreamSpan(fileStream, Offset, Length);
            streamSlice.CopyTo(s, _bufferLength);
        }

        private const int _bufferLength = 16 * 1024;
        private readonly FileInfo _fileInfo;

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
    }
}