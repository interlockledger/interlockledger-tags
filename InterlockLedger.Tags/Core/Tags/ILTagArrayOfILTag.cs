/******************************************************************************************************************************

Copyright (c) 2018-2019 InterlockLedger Network
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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InterlockLedger.Tags
{
    public class ILTagArrayOfILTag<T> : ILTagExplicit<T[]> where T : ILTag
    {
        public ILTagArrayOfILTag(IEnumerable<T> value) : base(ILTagId.ILTagArray, value?.ToArray()) {
        }

        public ILTagArrayOfILTag(object json) : base(ILTagId.ILTagArray, Elicit(json)) {
        }

        public override object AsJson => new JsonRepresentation(Value).AsNavigable();

        public T this[int i] => Value?[i];

        public IEnumerable<TV> GetValues<TV>() => (Value ?? Enumerable.Empty<T>()).Select(t => t is ILTagImplicit<TV> tv ? tv.Value : default);

        internal ILTagArrayOfILTag(Stream s) : base(ILTagId.ILTagArray, s) {
        }

        internal ILTagArrayOfILTag(Stream s, Func<Stream, T> decoder) : base(ILTagId.ILTagArray, s, (it) => SetDecoder(it, decoder)) {
        }

        protected ILTagArrayOfILTag(ulong tagId, T[] Value) : base(tagId, Value) {
        }

        protected ILTagArrayOfILTag(ulong tagId, Stream s) : base(tagId, s) {
        }

        protected override T[] FromBytes(byte[] bytes) =>
           FromBytesHelper(bytes, s => {
               var length = (int)s.ILIntDecode();
               var result = new T[length];
               for (var i = 0; i < length; i++) {
                   result[i] = _decoder(s);
               }
               return result;
           });

        protected override byte[] ToBytes()
            => ToBytesHelper(s => {
                if (Value != null) {
                    s.ILIntEncode((ulong)Value.Length);
                    foreach (var tag in Value)
                        s.EncodeTag(tag);
                }
            });

        private Func<Stream, T> _decoder = s => (T)s.DecodeTag();

        private static T[] Elicit(object o) {
            if (o is Dictionary<string, object> dictionary) {
                var elementTagId = Convert.ToUInt64(dictionary[nameof(JsonRepresentation.ElementTagId)]);
                if (dictionary[nameof(JsonRepresentation.Elements)] is List<object> elements) {
                    var list = new List<T>();
                    foreach (var item in elements) {
                        list.Add(DeserializeFromJson(elementTagId, item) as T);
                    }
                    return list.ToArray();
                }
            }
            return Array.Empty<T>();
        }

        private static void SetDecoder(ILTag it, Func<Stream, T> decoder) => ((ILTagArrayOfILTag<T>)it)._decoder = decoder;

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