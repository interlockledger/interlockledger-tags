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

using System;
using System.IO;
using System.Text.Json.Serialization;

namespace InterlockLedger.Tags
{
    public abstract class ILTag : ITag
    {
        [JsonIgnore]
        public abstract object AsJson { get; }

        [JsonIgnore]
        public byte[] EncodedBytes => _encodedBytes.Value;

        [JsonIgnore]
        public virtual string Formatted => "?";

        public ulong TagId { get; set; }
        public ITag Traits => this;

        public string AsString() => (TagId == ILTagId.String) ? Formatted : ToString();

        public abstract Stream SerializeInto(Stream s);

        public override string ToString() => GetType().Name + $"#{TagId}:" + Formatted;

        public virtual bool ValueIs<TV>(out TV value) {
            value = default;
            return false;
        }

        protected ILTag(ulong tagId) {
            TagId = tagId;
            _encodedBytes = NonFullBytes ? _throwIfCalled : new(ToBytes);
        }

        protected virtual bool NonFullBytes { get; }
        private static readonly Lazy<byte[]> _throwIfCalled = new(() => throw new InvalidOperationException("Should never call EncodedBytes for this tag type"));
        private readonly Lazy<byte[]> _encodedBytes;

        private byte[] ToBytes() {
            using var stream = new MemoryStream();
            SerializeInto(stream);
            stream.Flush();
            return stream.ToArray();
        }
    }
}