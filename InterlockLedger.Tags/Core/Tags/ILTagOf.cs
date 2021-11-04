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
#nullable enable

using System;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace InterlockLedger.Tags
{
    public abstract class ILTagOf<T> : ILTag
    {
        [JsonIgnore]
#pragma warning disable CS8603 // Possible null reference return.
        public override object AsJson => Value;
#pragma warning restore CS8603 // Possible null reference return.

        public override string Formatted => Value is IFormatted v ? v.Formatted : Value?.ToString() ?? "??";

        public T Value { get; set; }

        public override bool ValueIs<TV>(out TV value) {
            if (Value is TV tvalue) {
                value = tvalue;
                return true;
            }
#pragma warning disable CS8601 // Possible null reference assignment.
            value = default;
#pragma warning restore CS8601 // Possible null reference assignment.
            return false;
        }

        protected ILTagOf(ulong tagId, T value) : base(tagId) => Value = value;

        protected ILTagOf(ulong alreadyDeserializedTagId, Stream s, Action<ITag>? setup = null) : base(alreadyDeserializedTagId) {
            setup?.Invoke(this);
            Value = DeserializeInner(s);
        }

        protected abstract T DeserializeInner(Stream s);

        protected abstract T ValueFromStream(StreamSpan s);

        protected abstract Task<Stream> ValueToStreamAsync(Stream s);
    }
}