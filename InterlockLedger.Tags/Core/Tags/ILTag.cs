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

using System.IO;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace InterlockLedger.Tags
{
    public abstract class ILTag : ITag
    {
        [JsonIgnore]
        public abstract object AsJson { get; }

        [JsonIgnore]
        public byte[] EncodedBytes => KeepEncodedBytesInMemory ? (_encodedBytes ??= ToBytes()) : ToBytes();

        [JsonIgnore]
        public virtual string Formatted => "?";

        public ulong TagId { get; set; }

        [JsonIgnore]
        public ITag Traits => this;

        public string AsString() => (TagId == ILTagId.String) ? Formatted : ToString();

        public void Changed() {
            _encodedBytes = null;
            OnChanged();
        }

        protected virtual void OnChanged() { }

        public async virtual Task<Stream> OpenReadingStreamAsync() {
            if (KeepEncodedBytesInMemory)
                return new MemoryStream(EncodedBytes, writable: false);
            var s = await BuildTempStreamAsync();
            s.ILIntEncode(TagId);
            await SerializeInnerAsync(s);
            s.Flush();
            s.Position = 0;
            return new StreamSpan(s, 0, (ulong)s.Length, closeWrappedStreamOnDispose: true);
        }

        public async Task<Stream> SerializeIntoAsync(Stream s) {
            if (s is not null) {
                s.ILIntEncode(TagId);
                await SerializeInnerAsync(s);
                s.Flush();
            }
            return s;
        }

        public override string ToString() => GetType().Name + $"#{TagId}: " + Formatted;

        public virtual bool ValueIs<TV>(out TV value) {
            value = default;
            return false;
        }

        protected ILTag(ulong tagId) => TagId = tagId;

        protected virtual bool KeepEncodedBytesInMemory => true;

        protected virtual Task<Stream> BuildTempStreamAsync() => Task.FromResult<Stream>(new MemoryStream());

        protected abstract Task SerializeInnerAsync(Stream s);

        private byte[] _encodedBytes;

        private byte[] ToBytes() {
            using var stream = new MemoryStream();
            stream.ILIntEncode(TagId);
            SerializeInnerAsync(stream);
            stream.Flush();
            return stream.ToArray();
        }
    }
}