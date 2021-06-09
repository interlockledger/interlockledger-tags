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
    public abstract class ILTagOfSimple<T> : ILTag
    {
        [JsonIgnore]
        public override object AsJson => Value;

        public override string Formatted => Value is IFormatted v ? v.Formatted : "??";

        public T Value { get; set; }

        public sealed override Stream SerializeInto(Stream s) {
            if (s is null) return s;
            try {
                s.ILIntEncode(TagId);
                SerializeInner(s);
            } finally {
                s.Flush();
            }
            return s;
        }

        public override bool ValueIs<TV>(out TV value) {
            if (Value is TV tvalue) {
                value = tvalue;
                return true;
            }
            value = default;
            return false;
        }

        protected ILTagOfSimple(ulong tagId, T value) : base(tagId) => Value = value;

        protected ILTagOfSimple(ulong alreadyDeserializedTagId, Stream s, Action<ITag> setup) : base(alreadyDeserializedTagId) {
            setup?.Invoke(this);
            Value = DeserializeInner(s);
        }

        protected ILTagOfSimple(ulong tagId, Stream s) : this(tagId, s, setup: t => t.ValidateTagId(s.ILIntDecode())) { }

        protected abstract T DeserializeInner(Stream s);

        protected abstract void SerializeInner(Stream s);
    }
}