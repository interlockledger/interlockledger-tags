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
using System.Globalization;
using System.IO;
using System.Linq;

namespace InterlockLedger.Tags
{
    public class ILTagArrayOfILTag<T> : ILTagOfExplicit<T[]> where T : ILTag
    {
        public ILTagArrayOfILTag(IEnumerable<T> value) : base(ILTagId.ILTagArray, value?.ToArray()) {
        }

        public override object AsJson => new JsonRepresentation(Value).AsNavigable();

        public T this[int i] => Value?[i];

        public IEnumerable<TV> GetValues<TV>() => (Value ?? Enumerable.Empty<T>()).Select(t => t is ILTagOf<TV> tv ? tv.Value : default);

        // TODO: do better than _innerBytes
        public override Stream OpenReadingStream() => new ReadonlyTagStream(TagId, _innerBytes ??= ToBytes(Value));

        internal ILTagArrayOfILTag(Stream s) : base(ILTagId.ILTagArray, s) {
        }

        internal ILTagArrayOfILTag(Stream s, Func<Stream, T> decoder) : base(ILTagId.ILTagArray, s, (it) => SetDecoder(it, decoder)) {
        }

        internal static ILTagArrayOfILTag<T> FromJson(object json) => new(Elicit(json));

        protected ILTagArrayOfILTag(ulong tagId, T[] Value) : base(tagId, Value) {
        }

        protected ILTagArrayOfILTag(ulong tagId, Stream s) : base(tagId, s) {
        }

        protected override T[] DeserializeValueFromStream(StreamSpan s) {
            if (s.Length == 0)
                return null;
            var length = (int)s.ILIntDecode();
            var result = new T[length];
            for (var i = 0; i < length; i++) {
                result[i] = _decoder(s);
            }
            return result;
        }

        protected override void SerializeValueToStream(Stream s, T[] value) => s.WriteBytes(_innerBytes ??= ToBytes(value));

        protected override ulong ValueEncodedLength(T[] value) => (ulong)((_innerBytes ??= ToBytes(value))?.Length ?? 0);

        private Func<Stream, T> _decoder = s => AllowNull(s.DecodeTag());
        private byte[] _innerBytes;

        private static T AllowNull(ILTag tag) => tag.Traits.IsNull ? default : (T)tag;

        private static T[] Elicit(object o) {
            if (o is Dictionary<string, object> dictionary) {
                var elementTagId = Convert.ToUInt64(dictionary[nameof(JsonRepresentation.ElementTagId)], CultureInfo.InvariantCulture);
                if (dictionary[nameof(JsonRepresentation.Elements)] is IEnumerable<object> elements) {
                    var list = new List<T>();
                    foreach (var item in elements) {
                        if (item is ILTag tag) {
                            if (tag.TagId != elementTagId || !(tag is T))
                                throw new InvalidCastException($"Value in Array is a tag {tag.TagId} != {elementTagId}");
                            list.Add((T)tag);
                        } else {
                            list.Add(TagProvider.DeserializeFromJson(elementTagId, item) as T);
                        }
                    }
                    return list.ToArray();
                }
            }
            return Array.Empty<T>();
        }

        private static void SetDecoder(ITag it, Func<Stream, T> decoder) => ((ILTagArrayOfILTag<T>)it)._decoder = decoder;

        private static byte[] ToBytes(T[] value)
            => TagHelpers.ToBytesHelper(s => {
                if (value != null) {
                    s.ILIntEncode((ulong)value.Length);
                    foreach (var tag in value)
                        s.EncodeTag(tag);
                }
            });

        private class JsonRepresentation
        {
            public JsonRepresentation(IEnumerable<T> values) {
                ElementTagId = values.FirstOrDefault()?.TagId ?? ILTagId.Null;
                Elements = values.Select(e => e.AsJson).ToArray();
            }

            public object[] Elements { get; set; }
            public ulong ElementTagId { get; set; }
        }
    }
}