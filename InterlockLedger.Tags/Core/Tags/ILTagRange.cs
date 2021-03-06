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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.Json.Serialization;

namespace InterlockLedger.Tags
{
    [TypeConverter(typeof(TypeCustomConverter<ILTagRange>))]
    [JsonConverter(typeof(JsonCustomConverter<ILTagRange>))]
    public class ILTagRange : ILTagExplicit<LimitedRange>, ITextual<ILTagRange>, IEquatable<ILTagRange>
    {
        public ILTagRange() : this(new LimitedRange()) {
        }

        public ILTagRange(LimitedRange range) : base(ILTagId.Range, range) {
        }

        public ILTagRange(string textualRepresentation) : base(ILTagId.Range, new LimitedRange(textualRepresentation)) {
        }

        public override object AsJson => this;
        public override string Formatted => TextualRepresentation;
        public bool IsEmpty => Value.IsEmpty;
        public bool IsInvalid => Value.IsInvalid;
        public string TextualRepresentation => Value.TextualRepresentation;

        public static bool operator !=(ILTagRange left, ILTagRange right) => !(left == right);

        public static bool operator ==(ILTagRange left, ILTagRange right) => EqualityComparer<ILTagRange>.Default.Equals(left, right);

        public override bool Equals(object obj) => Equals(obj as ILTagRange);

        public bool Equals(ILTagRange other) => other != null && TextualRepresentation == other.TextualRepresentation;

        public override int GetHashCode() => HashCode.Combine(TextualRepresentation);

        internal ILTagRange(Stream s) : base(ILTagId.Range, s) {
        }

        protected override LimitedRange FromBytes(byte[] bytes)
            => FromBytesHelper(bytes, s => new LimitedRange(s.ILIntDecode(), s.BigEndianReadUShort()));

        protected override byte[] ToBytes(LimitedRange value)
            => TagHelpers.ToBytesHelper(s => s.ILIntEncode(Value.Start).BigEndianWriteUShort(Value.Count));
    }
}