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

namespace InterlockLedger.Tags;

public class ILTagArrayOfILTag<T> : ILTagOfExplicit<T[]> where T : ILTag
{
    public ILTagArrayOfILTag(IEnumerable<T> value) : this(ILTagId.ILTagArray, value.ToArray()) { }

    public override object AsJson => new JsonRepresentation(Value);
    public T this[int i] => Value[i];

    public IEnumerable<TV> GetValues<TV>() => Value.Select(t => t is ILTagOf<TV> tv ? tv.Value : default).SkipDefaults()!;
    protected override bool AreEquivalent(ILTagOf<T[]> other) => Value.EquivalentTo(other.Value);
    internal ILTagArrayOfILTag(Stream s) : base(ILTagId.ILTagArray, s) {
    }

    internal ILTagArrayOfILTag(Stream s, Func<Stream, T?> decoder) :
        base(ILTagId.ILTagArray, s, (it) => { ((ILTagArrayOfILTag<T>)it)._decoder = decoder; }) { }

    internal static ILTagArrayOfILTag<T> FromJson(object json) {
        return new(json switch {
            Dictionary<string, object> dictionary => FromDictionary(dictionary),
            JsonRepresentation jr => jr.ToArray(),
            _ => Array.Empty<T>(),
        });

        static T[] FromDictionary(Dictionary<string, object> dictionary) {
            var elementTagId = Convert.ToUInt64(dictionary[nameof(JsonRepresentation.ElementTagId)], CultureInfo.InvariantCulture);
            if (dictionary[nameof(JsonRepresentation.Elements)] is IEnumerable<object> elements) {
                var list = new List<T>();
                foreach (var item in elements) {
                    if (item is ILTag tag) {
                        if (tag.TagId != elementTagId || tag is not T)
                            throw new InvalidCastException($"Value in Array is a tag {tag.TagId} != {elementTagId}");
                        list.Add((T)tag);
                    } else {
                        list.Add((T)TagProvider.DeserializeFromJson(elementTagId, item) as T);
                    }
                }
                return list.ToArray();
            }
            return Array.Empty<T>();
        }
    }

    private protected ILTagArrayOfILTag(ulong tagId, T[] Value) : base(tagId, Value) { }

    private protected ILTagArrayOfILTag(ulong tagId, Stream s) : base(tagId, s, null) {
    }

    protected override T[] ValueFromStream(StreamSpan s) {
        if (s.Length > s.Position) {
            try {
                var arrayLength = (int)s.ILIntDecode();
                var list = new List<T>(arrayLength);
                for (var i = 0; i < arrayLength; i++) {
                    try {
                        var item = _decoder(s);
                        if (item is not null)
                            list.Add(item);
                        else
                            break;
                    } catch (Exception e) {
                        Console.WriteLine($"Error decoding ILTagArrayOfILTag<{typeof(T).FullName}>, expecting {arrayLength} items but failed at item {i} with: {e}");
                        break;
                    }
                }
                return list.ToArray();
            } catch (Exception e) {
                Console.WriteLine($"Error decoding ILTagArrayOfILTag<{typeof(T).FullName}>, failed with: {e}");
            }
        }
        return Array.Empty<T>();
    }

    protected override Task<Stream> ValueToStreamAsync(Stream s) {
        if (Value.Length > 0) {
            s.ILIntEncode((ulong)Value.Length);
            foreach (var tag in Value)
                s.EncodeTag(tag);
        }
        return Task.FromResult(s);

    }

    private Func<Stream, T?> _decoder = s => AllowNull(s.DecodeTag());

    private static T? AllowNull(ILTag tag) => (tag.Traits.IsNull || tag is not T) ? default : (T)tag;

    private class JsonRepresentation
    {
        public JsonRepresentation(IEnumerable<T> values) {
            ElementTagId = values.FirstOrDefault()?.TagId ?? ILTagId.Null;
            Elements = values.Select(e => e.AsJson).ToArray();
        }

        public object?[] Elements { get; set; }

        public ulong ElementTagId { get; set; }

        public T[] ToArray() => Elements.Select(item => item is T ? item : TagProvider.DeserializeFromJson(ElementTagId, item))
                                        .Cast<T>()
                                        .ToArray();
    }
}