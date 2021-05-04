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

namespace InterlockLedger.Tags
{
    public class ILTagByteArray : ILTagOfExplicit<byte[]>
    {
        public ILTagByteArray(object opaqueValue) : this(Elicit(opaqueValue)) {
        }

        public ILTagByteArray(byte[] value) : base(ILTagId.ByteArray, value) {
        }

        public ILTagByteArray(Span<byte> value) : this(value.ToArray()) {
        }

        public override Stream OpenReadingStream() => new ReadonlyTagStream(TagId, Value);

        internal ILTagByteArray(Stream s) : base(ILTagId.ByteArray, s) {
        }

        protected override byte[] DeserializeValueFromStream(StreamSpan s) => s.ReadAllBytesAsync().Result;

        protected override void SerializeValueToStream(Stream s, byte[] value) => s.WriteBytes(value);

        protected override ulong ValueEncodedLength(byte[] value) => (ulong)(value?.Length ?? 0);

        private static byte[] Elicit(object opaqueValue)
            => opaqueValue switch {
                byte[] value => value,
                string s => Convert.FromBase64String(s),
                _ => Array.Empty<byte>()
            };
    }
}