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
public abstract partial class VersionedValue<T> where T : notnull, VersionedValue<T>, new()
{
    public sealed class Payload : ILTagOfExplicit<T>
    {
        public Payload(ulong alreadyDeserializedTagId, Stream s)
            : base(alreadyDeserializedTagId, s)
            => Initialize();

        public Payload(ulong alreadyDeserializedTagId, ReadOnlySequence<byte> bytes)
            : base(alreadyDeserializedTagId, new ReadOnlySequenceStream(bytes), it => SetLength(it, (ulong)bytes.Length))
            => Initialize();

        internal Payload(T value) : base(value.Required().TagId, value) {
        }
 
        private void Initialize() {
            if (Value is not null) {
                Traits.ValidateTagId(Value.TagId);
                Value._payload = this;
            }
        }

        protected override async Task<T?> ValueFromStreamAsync(WrappedReadonlyStream s) =>
            await new T().FromStreamAsync(s).ConfigureAwait(false);
        protected override async Task<Stream> ValueToStreamAsync(Stream s) {
            await Value!.ToStreamAsync(s).ConfigureAwait(false);
            return s;
        }
    }
}